/*
 * This sample program is ported by C# from https://github.com/ggerganov/ggml/tree/master/examples/whisper.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using DrWavDotNet;
using GgmlDotNet;

using int64_t = System.Int64;
using whisper_token = System.Int32;

namespace Whisper
{

    internal sealed partial class Program
    {

        #region Fields

        private static readonly string[] k_colors = new []
        {
            "\033[38;5;196m", "\033[38;5;202m", "\033[38;5;208m", "\033[38;5;214m", "\033[38;5;220m",
            "\033[38;5;226m", "\033[38;5;190m", "\033[38;5;154m", "\033[38;5;118m", "\033[38;5;82m",
        };

        #endregion

        #region Methods

        private static int Main(string[] args)
        {
            string __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var @params = new WhisperParams();
            if (!whisper_params_parse(args, @params))
            {
                return 1;
            }

            if (!@params.fname_inp.Any())
            {
                Console.WriteLine("error: no input files specified");
                whisper_print_usage(args, @params);
                return 2;
            }

            if (@params.language != "auto" && whisper_lang_id(@params.language) == -1)
            {
                Console.WriteLine("error: unknown language '{0}'", @params.language);
                whisper_print_usage(args, @params);
                return 0;
            }

            // whisper init
            var ctx = whisper_init(@params.model);
            if (ctx == null)
            {
                Console.WriteLine("error: failed to initialize whisper context");
                return 3;
            }

            // initial prompt
            var prompt_tokens = new List<whisper_token>();

            if (!string.IsNullOrEmpty(@params.prompt))
            {
                // prompt_tokens.resize(1024);
                // prompt_tokens.resize(whisper_tokenize(ctx, @params.prompt, prompt_tokens, prompt_tokens.size()));
                prompt_tokens.Capacity = 1024;
                whisper_tokenize(ctx, @params.prompt, prompt_tokens, 1024);

                Console.WriteLine("");
                Console.WriteLine("initial prompt: '{0}'", @params.prompt);
                Console.WriteLine("initial tokens: [ ");
                for (var i = 0; i < (int) prompt_tokens.Count; ++i)
                {
                    Console.WriteLine("{0} ", prompt_tokens[i]);
                }

                Console.WriteLine("]");
            }
            
            for (var f = 0; f < (int)@params.fname_inp.Count; ++f)
            {
                var fname_inp = @params.fname_inp[f];

                var pcmf32 = new Buffer<float>(); // mono-channel F32 PCM
                var pcmf32s = new List<Buffer<float>>(); // stereo-channel F32 PCM

                // WAV input
                unsafe
                {
                    using var wav = new DrWav();
                    var wav_data = new List<byte>(); // used for pipe input from stdin

                    if (fname_inp == "-")
                    {
                        // ToDo
                        // {
                        //     var buf = stackalloc byte[1024];
                        //     while (true)
                        //     {
                        //         var n = fread(buf, 1, sizeof(buf), stdin);
                        //         if (n == 0)
                        //         {
                        //             break;
                        //         }
                        //         wav_data.AddRange(buf);
                        //     }
                        // }

                        // if (!wav.InitMemory(wav_data.ToArray(), wav_data.Count))
                        // {
                        //     Console.WriteLine("error: failed to open WAV file from stdin");
                        //     return 4;
                        // }

                        // Console.WriteLine($"{0}: read %zu bytes from stdin", __func__, wav_data.Count);
                    }
                    else if (!wav.InitFile(fname_inp))
                    {                        
                        Console.WriteLine($"error: failed to open '{0}' as WAV file", fname_inp);
                        return 5;
                    }

                    if (wav.Channels != 1 && wav.Channels != 2)
                    {
                        Console.WriteLine($"{0}: WAV file '{1}' must be mono or stereo", nameof(Whisper), fname_inp);
                        return 6;
                    }

                    if (@params.diarize && wav.Channels != 2 && @params.no_timestamps == false)
                    {
                        Console.WriteLine($"{0}: WAV file '{1}' must be stereo for diarization and timestamps have to be enabled", nameof(Whisper), fname_inp);
                        return 6;
                    }

                    if (wav.SampleRate != WHISPER_SAMPLE_RATE)
                    {
                        Console.WriteLine($"{0}: WAV file '{1}' must be {2} kHz", nameof(Whisper), fname_inp, WHISPER_SAMPLE_RATE / 1000);
                        return 8;
                    }

                    if (wav.BitsPerSample != 16)
                    {
                        Console.WriteLine($"{0}: WAV file '{1}' must be 16-bit", nameof(Whisper), fname_inp);
                        return 9;
                    }

                    var n = !wav_data.Any() ? (uint)wav.TotalPCMFrameCount : (uint)(wav_data.Count / (wav.Channels * wav.BitsPerSample / 8));

                    var pcm16 = new Buffer<short>();
                    pcm16.Resize(n * wav.Channels);
                    wav.ReadPcmFramesS16(n, pcm16.Data);
                    wav.Uninit();

                    // convert to mono, float
                    pcmf32.Resize(n);
                    if (wav.Channels == 1)
                    {
                        for (var i = 0; i < n; i++)
                        {
                            pcmf32[i] = (float)(pcm16[i]) / 32768.0f;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < n; i++)
                        {
                            pcmf32[i] = (float)(pcm16[2 * i] + pcm16[2 * i + 1]) / 65536.0f;
                        }
                    }

                    if (@params.diarize)
                    {
                        // convert to stereo, float
                        pcmf32s.Capacity = 2;
                        pcmf32s.Add(new Buffer<float>());
                        pcmf32s.Add(new Buffer<float>());

                        pcmf32s[0].Resize(n);
                        pcmf32s[1].Resize(n);

                        for (var i = 0; i < n; i++)
                        {
                            pcmf32s[0][i] = (float)(pcm16[2 * i]) / 32768.0f;
                            pcmf32s[1][i] = (float)(pcm16[2 * i + 1]) / 32768.0f;
                        }
                    }

                    pcm16.Free();
                }

                // print system information
                {
                    Console.WriteLine("");
                    Console.WriteLine("system_info: n_threads = {0} / {1} | {2}",
                                      @params.n_threads * @params.n_processors, System.Environment.ProcessorCount, whisper_print_system_info());
                }

                // print some info about the processing
                {
                    Console.WriteLine("");
                    if (whisper_is_multilingual(ctx) == 0)
                    {
                        if (@params.language != "en" || @params.translate)
                        {
                            @params.language = "en";
                            @params.translate = false;
                            Console.WriteLine("{0}: WARNING: model is not multilingual, ignoring language and translation options", __func__);
                        }
                    }

                    Console.WriteLine("{0}: processing '{1}' ({2} samples, {3:F1} sec), {4} threads, {5} processors, lang = {6}, task = {7}, timestamps = {8} ...",
                                      __func__, fname_inp, (int)(pcmf32.Size), (float)(pcmf32.Size) / WHISPER_SAMPLE_RATE,
                                      @params.n_threads, @params.n_processors,
                                      @params.language,
                                      @params.translate ? "translate" : "transcribe",
                                      @params.no_timestamps ? 0 : 1);

                    Console.WriteLine("");
                }

                // run the inference
                {
                    var wparams = whisper_full_default_params(whisper_sampling_strategy.WHISPER_SAMPLING_GREEDY);

                    wparams.print_realtime   = false;
                    wparams.print_progress   = @params.print_progress;
                    wparams.print_timestamps = !@params.no_timestamps;
                    wparams.print_special    = @params.print_special;
                    wparams.translate        = @params.translate;
                    wparams.language         = @params.language;
                    wparams.n_threads        = @params.n_threads;
                    wparams.n_max_text_ctx   = @params.max_context >= 0 ? @params.max_context : wparams.n_max_text_ctx;
                    wparams.offset_ms        = @params.offset_t_ms;
                    wparams.duration_ms      = @params.duration_ms;

                    wparams.token_timestamps = @params.output_wts || @params.max_len > 0;
                    wparams.thold_pt         = @params.word_thold;
                    wparams.max_len          = @params.output_wts && @params.max_len == 0 ? 60 : @params.max_len;

                    wparams.speed_up         = @params.speed_up;

                    wparams.prompt_tokens    = !prompt_tokens.Any() ? null : prompt_tokens;
                    wparams.prompt_n_tokens  = !prompt_tokens.Any() ? 0    : prompt_tokens.Count;

                    var user_data = new whisper_print_user_data
                    {
                        @params = @params,
                        pcmf32s = pcmf32s
                    };

                    // this callback is called on each new segment
                    if (!wparams.print_realtime)
                    {
                        wparams.new_segment_callback           = whisper_print_segment_callback;
                        wparams.new_segment_callback_user_data = user_data;
                    }

                    // example for abort mechanism
                    // in this example, we do not abort the processing, but we could if the flag is set to true
                    // the callback is called before every encoder run - if it returns false, the processing is aborted
                    {
                        // ToDo: wrap bool object by class
                        // static bool is_aborted = false; // NOTE: this should be atomic to avoid data race
                        bool is_aborted = false; // NOTE: this should be atomic to avoid data race

                        wparams.encoder_begin_callback = new whisper_encoder_begin_callback((ctx, user_data) =>
                        {
                            var is_aborted = (bool)user_data;
                            return !is_aborted;
                        });
                        wparams.encoder_begin_callback_user_data = is_aborted;
                    }

                    if (whisper_full_parallel(ctx, wparams, pcmf32, (int)pcmf32.Size, @params.n_processors) != 0)
                    {
                        Console.WriteLine("{0}: failed to process audio", nameof(Whisper));
                        return 10;
                    }
                }

                // output stuff
                {
                    Console.WriteLine("");

                    // output to text file
                    if (@params.output_txt)
                    {
                        var fname_txt = fname_inp + ".txt";
                        output_txt(ctx, fname_txt);
                    }

                    // output to VTT file
                    if (@params.output_vtt)
                    {
                        var fname_vtt = fname_inp + ".vtt";
                        output_vtt(ctx, fname_vtt);
                    }

                    // output to SRT file
                    if (@params.output_srt)
                    {
                        var fname_srt = fname_inp + ".srt";
                        output_srt(ctx, fname_srt, @params);
                    }

                    // output to WTS file
                    if (@params.output_wts)
                    {
                        var fname_wts = fname_inp + ".wts";
                        output_wts(ctx, fname_wts, fname_inp, @params, (float)(pcmf32.Size + 1000) / WHISPER_SAMPLE_RATE);
                    }

                    // output to CSV file
                    if (@params.output_csv)
                    {
                        var fname_csv = fname_inp + ".csv";
                        output_csv(ctx, fname_csv);
                    }
                }
            }

            whisper_print_timings(ctx);
            whisper_free(ctx);

            return 0;
        }

        #region Helpers

        private static bool whisper_params_parse(string[] args, WhisperParams @params)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg[0] != '-')
                {
                    @params.fname_inp.Add(arg);
                    continue;
                }

                if (arg == "-h" || arg == "--help")
                {
                    whisper_print_usage(args, @params);
                    Environment.Exit(0);
                }
                else if (arg == "-t"    || arg == "--threads")        { @params.n_threads      = int.Parse(args[++i]); }
                else if (arg == "-p"    || arg == "--processors")     { @params.n_processors   = int.Parse(args[++i]); }
                else if (arg == "-ot"   || arg == "--offset-t")       { @params.offset_t_ms    = int.Parse(args[++i]); }
                else if (arg == "-on"   || arg == "--offset-n")       { @params.offset_n       = int.Parse(args[++i]); }
                else if (arg == "-d"    || arg == "--duration")       { @params.duration_ms    = int.Parse(args[++i]); }
                else if (arg == "-mc"   || arg == "--max-context")    { @params.max_context    = int.Parse(args[++i]); }
                else if (arg == "-ml"   || arg == "--max-len")        { @params.max_len        = int.Parse(args[++i]); }
                else if (arg == "-wt"   || arg == "--word-thold")     { @params.word_thold     = float.Parse(args[++i]); }
                else if (arg == "-su"   || arg == "--speed-up")       { @params.speed_up       = true; }
                else if (arg == "-tr"   || arg == "--translate")      { @params.translate      = true; }
                else if (arg == "-di"   || arg == "--diarize")        { @params.diarize        = true; }
                else if (arg == "-otxt" || arg == "--output-txt")     { @params.output_txt     = true; }
                else if (arg == "-ovtt" || arg == "--output-vtt")     { @params.output_vtt     = true; }
                else if (arg == "-osrt" || arg == "--output-srt")     { @params.output_srt     = true; }
                else if (arg == "-owts" || arg == "--output-words")   { @params.output_wts     = true; }
                else if (arg == "-ocsv" || arg == "--output-csv")     { @params.output_csv     = true; }
                else if (arg == "-ps"   || arg == "--print-special")  { @params.print_special  = true; }
                else if (arg == "-pc"   || arg == "--print-colors")   { @params.print_colors   = true; }
                else if (arg == "-pp"   || arg == "--print-progress") { @params.print_progress = true; }
                else if (arg == "-nt"   || arg == "--no-timestamps")  { @params.no_timestamps  = true; }
                else if (arg == "-l"    || arg == "--language")       { @params.language       = args[++i]; }
                else if (                  arg == "--prompt")         { @params.prompt         = args[++i]; }
                else if (arg == "-m"    || arg == "--model")          { @params.model          = args[++i]; }
                else if (arg == "-f"    || arg == "--file")           { @params.fname_inp.Add(args[++i]); }
                else {
                    Console.WriteLine($"error: unknown argument: {arg}");
                    whisper_print_usage(args, @params);
                    Environment.Exit(0);
                }
            }

            return true;
        }

        private static int whisper_full_n_segments(whisper_context ctx)
        {
            return ctx.result_all.Count;
        }

        private static int64_t whisper_full_get_segment_t0(whisper_context ctx, int i_segment)
        {
            return ctx.result_all[i_segment].t0;
        }

        private static int64_t whisper_full_get_segment_t1(whisper_context ctx, int i_segment)
        {
            return ctx.result_all[i_segment].t1;
        }

        private static string whisper_full_get_segment_text(whisper_context ctx, int i_segment)
        {
            return ctx.result_all[i_segment].text;
        }

        private static int whisper_full_n_tokens(whisper_context ctx, int i_segment)
        {
            return ctx.result_all[i_segment].tokens.Count;
        }

        private static string whisper_full_get_token_text(whisper_context ctx, int i_segment, int i_token)
        {
            return ctx.vocab.id_to_token[ctx.result_all[i_segment].tokens[i_token].id];
        }

        private static whisper_token whisper_full_get_token_id(whisper_context ctx, int i_segment, int i_token)
        {
            return ctx.result_all[i_segment].tokens[i_token].id;
        }

        private static whisper_token_data whisper_full_get_token_data(whisper_context ctx, int i_segment, int i_token)
        {
            return ctx.result_all[i_segment].tokens[i_token];
        }

        private static float whisper_full_get_token_p(whisper_context ctx, int i_segment, int i_token)
        {
            return ctx.result_all[i_segment].tokens[i_token].p;
        }

        private static void whisper_print_usage(string[] args, WhisperParams @params)
        {
            Console.WriteLine("");
            Console.WriteLine(string.Format("usage: {0} [options] file0.wav file1.wav ...", args[0]));
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("  -h,       --help           [default] show this help message and exit");
            Console.WriteLine(string.Format("  -t N,     --threads N      [{0,-7}] number of threads to use during computation",    @params.n_threads));
            Console.WriteLine(string.Format("  -p N,     --processors N   [{0,-7}] number of processors to use during computation", @params.n_processors));
            Console.WriteLine(string.Format("  -ot N,    --offset-t N     [{0,-7}] time offset in milliseconds",                    @params.offset_t_ms));
            Console.WriteLine(string.Format("  -on N,    --offset-n N     [{0,-7}] segment index offset",                           @params.offset_n));
            Console.WriteLine(string.Format("  -d  N,    --duration N     [{0,-7}] duration of audio to process in milliseconds",   @params.duration_ms));
            Console.WriteLine(string.Format("  -mc N,    --max-context N  [{0,-7}] maximum number of text context tokens to store", @params.max_context));
            Console.WriteLine(string.Format("  -ml N,    --max-len N      [{0,-7}] maximum segment length in characters",           @params.max_len));
            Console.WriteLine(string.Format("  -wt N,    --word-thold N   [{0,-7:F2}] word timestamp probability threshold",        @params.word_thold));
            Console.WriteLine(string.Format("  -su,      --speed-up       [{0,-7}] speed up audio by x2 (reduced accuracy)",        @params.speed_up ? "true" : "false"));
            Console.WriteLine(string.Format("  -tr,      --translate      [{0,-7}] translate from source language to english",      @params.translate ? "true" : "false"));
            Console.WriteLine(string.Format("  -di,      --diarize        [{0,-7}] stereo audio diarization",                       @params.diarize ? "true" : "false"));
            Console.WriteLine(string.Format("  -otxt,    --output-txt     [{0,-7}] output result in a text file",                   @params.output_txt ? "true" : "false"));
            Console.WriteLine(string.Format("  -ovtt,    --output-vtt     [{0,-7}] output result in a vtt file",                    @params.output_vtt ? "true" : "false"));
            Console.WriteLine(string.Format("  -osrt,    --output-srt     [{0,-7}] output result in a srt file",                    @params.output_srt ? "true" : "false"));
            Console.WriteLine(string.Format("  -owts,    --output-words   [{0,-7}] output script for generating karaoke video",     @params.output_wts ? "true" : "false"));
            Console.WriteLine(string.Format("  -ocsv,    --output-csv     [{0,-7}] output result in a CSV file",                    @params.output_csv ? "true" : "false"));
            Console.WriteLine(string.Format("  -ps,      --print-special  [{0,-7}] print special tokens",                           @params.print_special ? "true" : "false"));
            Console.WriteLine(string.Format("  -pc,      --print-colors   [{0,-7}] print colors",                                   @params.print_colors ? "true" : "false"));
            Console.WriteLine(string.Format("  -pp,      --print-progress [{0,-7}] print progress",                                 @params.print_progress ? "true" : "false"));
            Console.WriteLine(string.Format("  -nt,      --no-timestamps  [{0,-7}] do not print timestamps",                        @params.no_timestamps ? "false" : "true"));
            Console.WriteLine(string.Format("  -l LANG,  --language LANG  [{0,-7}] spoken language ('auto' for auto-detect)",       @params.language));
            Console.WriteLine(string.Format("            --prompt PROMPT  [{0,-7}] initial prompt",                                 @params.prompt));
            Console.WriteLine(string.Format("  -m FNAME, --model FNAME    [{0,-7}] model path",                                     @params.model));
            Console.WriteLine(string.Format("  -f FNAME, --file FNAME     [{0,-7}] input WAV file path",                            ""));
            Console.WriteLine("");
        }

        private unsafe static void whisper_print_segment_callback(whisper_context ctx,
                                                                  int n_new,
                                                                  object user_data)
        {
            var @params  = ((whisper_print_user_data)user_data).@params;
            var pcmf32s =  ((whisper_print_user_data)user_data).pcmf32s;

            var n_segments = whisper_full_n_segments(ctx);

            var speaker = "";

            int64_t t0 = 0;
            int64_t t1 = 0;

            // print the last n_new segments
            var s0 = n_segments - n_new;

            if (s0 == 0)
            {
                Console.WriteLine("");
            }

            for (var i = s0; i < n_segments; i++)
            {
                if (!@params.no_timestamps || @params.diarize)
                {
                    t0 = whisper_full_get_segment_t0(ctx, i);
                    t1 = whisper_full_get_segment_t1(ctx, i);
                }

                if (!@params.no_timestamps)
                {
                    Console.Write("[{0} --> {1}]  ", to_timestamp(t0), to_timestamp(t1));
                }

                if (@params.diarize && pcmf32s.Count == 2)
                {
                    var n_samples = (int)pcmf32s[0].Size;

                    var is0 = timestamp_to_sample(t0, n_samples);
                    var is1 = timestamp_to_sample(t1, n_samples);

                    var energy0 = 0.0d;
                    var energy1 = 0.0d;

                    var p32_1 = (float*)pcmf32s[0].Data;
                    var p32_2 = (float*)pcmf32s[1].Data;
                    for (var j = is0; j < is1; j++)
                    {
                        energy0 += Math.Abs(p32_1[j]);
                        energy1 += Math.Abs(p32_2[j]);
                    }

                    if (energy0 > 1.1 * energy1)
                    {
                        speaker = "(speaker 0)";
                    }
                    else if (energy1 > 1.1 * energy0)
                    {
                        speaker = "(speaker 1)";
                    }
                    else
                    {
                        speaker = "(speaker ?)";
                    }

                    //printf("is0 = %lld, is1 = %lld, energy0 = %f, energy1 = %f, %s\n", is0, is1, energy0, energy1, speaker);
                }

                if (@params.print_colors)
                {
                    for (int j = 0; j < whisper_full_n_tokens(ctx, i); ++j)
                    {
                        if (@params.print_special == false)
                        {
                            var id = whisper_full_get_token_id(ctx, i, j);
                            if (id >= whisper_token_eot(ctx))
                            {
                                continue;
                            }
                        }

                        string text = whisper_full_get_token_text(ctx, i, j);
                        var    p    = whisper_full_get_token_p   (ctx, i, j);

                        var col = Math.Max(0, Math.Min((int)k_colors.Length, (int)(Math.Pow(p, 3) * (float)(k_colors.Length))));

                        Console.Write("{0}{1}{2}{3}", speaker, k_colors[col], text, "\033[0m");
                    }
                }
                else
                {
                    string text = whisper_full_get_segment_text(ctx, i);

                    Console.Write("{0}{1}", speaker, text);
                }

                // with timestamps or speakers: each segment on new line
                if (!@params.no_timestamps || @params.diarize)
                {
                    Console.WriteLine("");
                }

                // fflush(stdout);
            }
        }

        private static bool output_txt(whisper_context ctx, string fname)
        {
            // std::ofstream fout(fname);
            // if (!fout.is_open()) {
            //     fprintf(stderr, "%s: failed to open '%s' for writing\n", __func__, fname);
            //     return false;
            // }

            // fprintf(stderr, "%s: saving output to '%s'\n", __func__, fname);

            // const int n_segments = whisper_full_n_segments(ctx);
            // for (int i = 0; i < n_segments; ++i) {
            //     string text = whisper_full_get_segment_text(ctx, i);
            //     fout << text << "\n";
            // }

            return true;
        }

        private static bool output_vtt(whisper_context ctx, string fname)
        {
            // std::ofstream fout(fname);
            // if (!fout.is_open()) {
            //     fprintf(stderr, "%s: failed to open '%s' for writing\n", __func__, fname);
            //     return false;
            // }

            // fprintf(stderr, "%s: saving output to '%s'\n", __func__, fname);

            // fout << "WEBVTT\n\n";

            // const int n_segments = whisper_full_n_segments(ctx);
            // for (int i = 0; i < n_segments; ++i) {
            //     string text = whisper_full_get_segment_text(ctx, i);
            //     var t0 = whisper_full_get_segment_t0(ctx, i);
            //     var t1 = whisper_full_get_segment_t1(ctx, i);

            //     fout << to_timestamp(t0) << " --> " << to_timestamp(t1) << "\n";
            //     fout << text << "\n\n";
            // }

            return true;
        }

        private static bool output_srt(whisper_context ctx, string fname, WhisperParams @params)
        {
            // std::ofstream fout(fname);
            // if (!fout.is_open()) {
            //     fprintf(stderr, "%s: failed to open '%s' for writing\n", __func__, fname);
            //     return false;
            // }

            // fprintf(stderr, "%s: saving output to '%s'\n", __func__, fname);

            // const int n_segments = whisper_full_n_segments(ctx);
            // for (int i = 0; i < n_segments; ++i) {
            //     string text = whisper_full_get_segment_text(ctx, i);
            //     var t0 = whisper_full_get_segment_t0(ctx, i);
            //     var t1 = whisper_full_get_segment_t1(ctx, i);

            //     fout << i + 1 + params.offset_n << "\n";
            //     fout << to_timestamp(t0, true) << " --> " << to_timestamp(t1, true) << "\n";
            //     fout << text << "\n\n";
            // }

            return true;
        }

        private static bool output_csv(whisper_context ctx, string fname)
        {
            // std::ofstream fout(fname);
            // if (!fout.is_open()) {
            //     fprintf(stderr, "%s: failed to open '%s' for writing\n", __func__, fname);
            //     return false;
            // }

            // fprintf(stderr, "%s: saving output to '%s'\n", __func__, fname);

            // const int n_segments = whisper_full_n_segments(ctx);
            // for (int i = 0; i < n_segments; ++i) {
            //     string text = whisper_full_get_segment_text(ctx, i);
            // if (text[0] == ' ')
            // text = text + sizeof(char); //whisper_full_get_segment_text() returns a string with leading space, point to the next character.
            //     var t0 = whisper_full_get_segment_t0(ctx, i);
            //     var t1 = whisper_full_get_segment_t1(ctx, i);
            // //need to multiply times returned from whisper_full_get_segment_t{0,1}() by 10 to get milliseconds.
            //     fout << 10 * t0 << ", " 
            //     << 10 * t1 << ", \"" 
            //     << text    << "\"\n";
            // }

            return true;
        }

        private static bool output_wts(whisper_context ctx, string fname, string fname_inp, WhisperParams @params, float t_sec)
        {
            // std::ofstream fout(fname);

            // fprintf(stderr, "%s: saving output to '%s'\n", __func__, fname);

            // // TODO: become parameter
            // static string font = "/System/Library/Fonts/Supplemental/Courier New Bold.ttf";

            // fout << "#!/bin/bash" << "\n";
            // fout << "\n";

            // fout << "ffmpeg -i " << fname_inp << " -f lavfi -i color=size=1200x120:duration=" << t_sec << ":rate=25:color=black -vf \"";

            // for (int i = 0; i < whisper_full_n_segments(ctx); i++) {
            //     var t0 = whisper_full_get_segment_t0(ctx, i);
            //     var t1 = whisper_full_get_segment_t1(ctx, i);

            //     const int n = whisper_full_n_tokens(ctx, i);

            //     std::vector<whisper_token_data> tokens(n);
            //     for (int j = 0; j < n; ++j) {
            //         tokens[j] = whisper_full_get_token_data(ctx, i, j);
            //     }

            //     if (i > 0) {
            //         fout << ",";
            //     }

            //     // background text
            //     fout << "drawtext=fontfile='" << font << "':fontsize=24:fontcolor=gray:x=(w-text_w)/2:y=h/2:text='':enable='between(t," << t0/100.0 << "," << t0/100.0 << ")'";

            //     bool is_first = true;

            //     for (int j = 0; j < n; ++j) {
            //         const auto & token = tokens[j];

            //         if (tokens[j].id >= whisper_token_eot(ctx)) {
            //             continue;
            //         }

            //         std::string txt_bg;
            //         std::string txt_fg; // highlight token
            //         std::string txt_ul; // underline

            //         txt_bg = "> ";
            //         txt_fg = "> ";
            //         txt_ul = "\\ \\ ";

            //         {
            //             for (int k = 0; k < n; ++k) {
            //                 const auto & token2 = tokens[k];

            //                 if (tokens[k].id >= whisper_token_eot(ctx)) {
            //                     continue;
            //                 }

            //                 const std::string txt = whisper_token_to_str(ctx, token2.id);

            //                 txt_bg += txt;

            //                 if (k == j) {
            //                     for (int l = 0; l < (int) txt.size(); ++l) {
            //                         txt_fg += txt[l];
            //                         txt_ul += "_";
            //                     }
            //                     txt_fg += "|";
            //                 } else {
            //                     for (int l = 0; l < (int) txt.size(); ++l) {
            //                         txt_fg += "\\ ";
            //                         txt_ul += "\\ ";
            //                     }
            //                 }
            //             }

            //             ::replace_all(txt_bg, "'", "\u2019");
            //             ::replace_all(txt_bg, "\"", "\\\"");
            //             ::replace_all(txt_fg, "'", "\u2019");
            //             ::replace_all(txt_fg, "\"", "\\\"");
            //         }

            //         if (is_first) {
            //             // background text
            //             fout << ",drawtext=fontfile='" << font << "':fontsize=24:fontcolor=gray:x=(w-text_w)/2:y=h/2:text='" << txt_bg << "':enable='between(t," << t0/100.0 << "," << t1/100.0 << ")'";
            //             is_first = false;
            //         }

            //         // foreground text
            //         fout << ",drawtext=fontfile='" << font << "':fontsize=24:fontcolor=lightgreen:x=(w-text_w)/2+8:y=h/2:text='" << txt_fg << "':enable='between(t," << token.t0/100.0 << "," << token.t1/100.0 << ")'";

            //         // underline
            //         fout << ",drawtext=fontfile='" << font << "':fontsize=24:fontcolor=lightgreen:x=(w-text_w)/2+8:y=h/2+16:text='" << txt_ul << "':enable='between(t," << token.t0/100.0 << "," << token.t1/100.0 << ")'";
            //     }
            // }

            // fout << "\" -c:v libx264 -pix_fmt yuv420p -y " << fname_inp << ".mp4" << "\n";

            // fout << "\n\n";
            // fout << "echo \"Your video has been saved to " << fname_inp << ".mp4\"" << "\n";
            // fout << "\n";
            // fout << "echo \"  ffplay " << fname_inp << ".mp4\"\n";
            // fout << "\n";

            // fout.close();

            // fprintf(stderr, "%s: run 'source %s' to generate karaoke video\n", __func__, fname);

            return true;
        }

        #endregion

        #endregion

    }

}