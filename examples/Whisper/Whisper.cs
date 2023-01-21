using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using GgmlDotNet;

using int32_t = System.Int32;
using int64_t = System.Int64;
using uint32_t = System.UInt32;
using size_t = System.Int64;
using id  = System.Int32;
using whisper_token = System.Int32;

namespace Whisper
{

    internal sealed partial class Program
    {

        #region Fields
        
        private static int iter = 0;

        private const int WHISPER_SAMPLE_RATE = 16000;

        private const int WHISPER_N_FFT = 400;

        private const int WHISPER_N_MEL = 80;

        private const int WHISPER_HOP_LENGTH = 160;

        private const int WHISPER_CHUNK_SIZE = 30;

        private const size_t MB = 1024 * 1024;

        private static readonly IDictionary<string, KeyValuePair<int, string>> g_lang = new Dictionary<string, KeyValuePair<int, string>>()
        {
            { "en",  new KeyValuePair<int, string>( 0,  "english"         ) },
            { "zh",  new KeyValuePair<int, string>( 1,  "chinese"         ) },
            { "de",  new KeyValuePair<int, string>( 2,  "german"          ) },
            { "es",  new KeyValuePair<int, string>( 3,  "spanish"         ) },
            { "ru",  new KeyValuePair<int, string>( 4,  "russian"         ) },
            { "ko",  new KeyValuePair<int, string>( 5,  "korean"          ) },
            { "fr",  new KeyValuePair<int, string>( 6,  "french"          ) },
            { "ja",  new KeyValuePair<int, string>( 7,  "japanese"        ) },
            { "pt",  new KeyValuePair<int, string>( 8,  "portuguese"      ) },
            { "tr",  new KeyValuePair<int, string>( 9,  "turkish"         ) },
            { "pl",  new KeyValuePair<int, string>( 10, "polish"          ) },
            { "ca",  new KeyValuePair<int, string>( 11,  "catalan"        ) },
            { "nl",  new KeyValuePair<int, string>( 12,  "dutch"          ) },
            { "ar",  new KeyValuePair<int, string>( 13,  "arabic"         ) },
            { "sv",  new KeyValuePair<int, string>( 14,  "swedish"        ) },
            { "it",  new KeyValuePair<int, string>( 15,  "italian"        ) },
            { "id",  new KeyValuePair<int, string>( 16,  "indonesian"     ) },
            { "hi",  new KeyValuePair<int, string>( 17,  "hindi"          ) },
            { "fi",  new KeyValuePair<int, string>( 18,  "finnish"        ) },
            { "vi",  new KeyValuePair<int, string>( 19,  "vietnamese"     ) },
            { "iw",  new KeyValuePair<int, string>( 20,  "hebrew"         ) },
            { "uk",  new KeyValuePair<int, string>( 21,  "ukrainian"      ) },
            { "el",  new KeyValuePair<int, string>( 22,  "greek"          ) },
            { "ms",  new KeyValuePair<int, string>( 23,  "malay"          ) },
            { "cs",  new KeyValuePair<int, string>( 24,  "czech"          ) },
            { "ro",  new KeyValuePair<int, string>( 25,  "romanian"       ) },
            { "da",  new KeyValuePair<int, string>( 26,  "danish"         ) },
            { "hu",  new KeyValuePair<int, string>( 27,  "hungarian"      ) },
            { "ta",  new KeyValuePair<int, string>( 28,  "tamil"          ) },
            { "no",  new KeyValuePair<int, string>( 29,  "norwegian"      ) },
            { "th",  new KeyValuePair<int, string>( 30,  "thai"           ) },
            { "ur",  new KeyValuePair<int, string>( 31,  "urdu"           ) },
            { "hr",  new KeyValuePair<int, string>( 32,  "croatian"       ) },
            { "bg",  new KeyValuePair<int, string>( 33,  "bulgarian"      ) },
            { "lt",  new KeyValuePair<int, string>( 34,  "lithuanian"     ) },
            { "la",  new KeyValuePair<int, string>( 35,  "latin"          ) },
            { "mi",  new KeyValuePair<int, string>( 36,  "maori"          ) },
            { "ml",  new KeyValuePair<int, string>( 37,  "malayalam"      ) },
            { "cy",  new KeyValuePair<int, string>( 38,  "welsh"          ) },
            { "sk",  new KeyValuePair<int, string>( 39,  "slovak"         ) },
            { "te",  new KeyValuePair<int, string>( 40,  "telugu"         ) },
            { "fa",  new KeyValuePair<int, string>( 41,  "persian"        ) },
            { "lv",  new KeyValuePair<int, string>( 42,  "latvian"        ) },
            { "bn",  new KeyValuePair<int, string>( 43,  "bengali"        ) },
            { "sr",  new KeyValuePair<int, string>( 44,  "serbian"        ) },
            { "az",  new KeyValuePair<int, string>( 45,  "azerbaijani"    ) },
            { "sl",  new KeyValuePair<int, string>( 46,  "slovenian"      ) },
            { "kn",  new KeyValuePair<int, string>( 47,  "kannada"        ) },
            { "et",  new KeyValuePair<int, string>( 48,  "estonian"       ) },
            { "mk",  new KeyValuePair<int, string>( 49,  "macedonian"     ) },
            { "fin",  new KeyValuePair<int, string>( 50,  "breton"         ) },
            { "eu",  new KeyValuePair<int, string>( 51,  "basque"         ) },
            { "is",  new KeyValuePair<int, string>( 52,  "icelandic"      ) },
            { "hy",  new KeyValuePair<int, string>( 53,  "armenian"       ) },
            { "ne",  new KeyValuePair<int, string>( 54,  "nepali"         ) },
            { "mn",  new KeyValuePair<int, string>( 55,  "mongolian"      ) },
            { "bs",  new KeyValuePair<int, string>( 56,  "bosnian"        ) },
            { "kk",  new KeyValuePair<int, string>( 57,  "kazakh"         ) },
            { "sq",  new KeyValuePair<int, string>( 58,  "albanian"       ) },
            { "sw",  new KeyValuePair<int, string>( 59,  "swahili"        ) },
            { "gl",  new KeyValuePair<int, string>( 60,  "galician"       ) },
            { "mr",  new KeyValuePair<int, string>( 61,  "marathi"        ) },
            { "pa",  new KeyValuePair<int, string>( 62,  "punjabi"        ) },
            { "si",  new KeyValuePair<int, string>( 63,  "sinhala"        ) },
            { "km",  new KeyValuePair<int, string>( 64,  "khmer"          ) },
            { "sn",  new KeyValuePair<int, string>( 65,  "shona"          ) },
            { "yo",  new KeyValuePair<int, string>( 66,  "yoruba"         ) },
            { "so",  new KeyValuePair<int, string>( 67,  "somali"         ) },
            { "af",  new KeyValuePair<int, string>( 68,  "afrikaans"      ) },
            { "oc",  new KeyValuePair<int, string>( 69,  "occitan"        ) },
            { "ka",  new KeyValuePair<int, string>( 70,  "georgian"       ) },
            { "be",  new KeyValuePair<int, string>( 71,  "belarusian"     ) },
            { "tg",  new KeyValuePair<int, string>( 72,  "tajik"          ) },
            { "sd",  new KeyValuePair<int, string>( 73,  "sindhi"         ) },
            { "gu",  new KeyValuePair<int, string>( 74,  "gujarati"       ) },
            { "am",  new KeyValuePair<int, string>( 75,  "amharic"        ) },
            { "yi",  new KeyValuePair<int, string>( 76,  "yiddish"        ) },
            { "lo",  new KeyValuePair<int, string>( 77,  "lao"            ) },
            { "uz",  new KeyValuePair<int, string>( 78,  "uzbek"          ) },
            { "fo",  new KeyValuePair<int, string>( 79,  "faroese"        ) },
            { "ht",  new KeyValuePair<int, string>( 80,  "haitian creole" ) },
            { "ps",  new KeyValuePair<int, string>( 81,  "pashto"         ) },
            { "tk",  new KeyValuePair<int, string>( 82,  "turkmen"        ) },
            { "nn",  new KeyValuePair<int, string>( 83,  "nynorsk"        ) },
            { "mt",  new KeyValuePair<int, string>( 84,  "maltese"        ) },
            { "sa",  new KeyValuePair<int, string>( 85,  "sanskrit"       ) },
            { "lb",  new KeyValuePair<int, string>( 86,  "luxembourgish"  ) },
            { "my",  new KeyValuePair<int, string>( 87,  "myanmar"        ) },
            { "bo",  new KeyValuePair<int, string>( 88,  "tibetan"        ) },
            { "tl",  new KeyValuePair<int, string>( 89,  "tagalog"        ) },
            { "mg",  new KeyValuePair<int, string>( 90,  "malagasy"       ) },
            { "as",  new KeyValuePair<int, string>( 91,  "assamese"       ) },
            { "tt",  new KeyValuePair<int, string>( 92,  "tatar"          ) },
            { "haw", new KeyValuePair<int, string>( 93,  "hawaiian"       ) },
            { "ln",  new KeyValuePair<int, string>( 94,  "lingala"        ) },
            { "ha",  new KeyValuePair<int, string>( 95,  "hausa"          ) },
            { "ba",  new KeyValuePair<int, string>( 96,  "bashkir"        ) },
            { "jw",  new KeyValuePair<int, string>( 97,  "javanese"       ) },
            { "su",  new KeyValuePair<int, string>( 98,  "sundanese"      ) }
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_MODEL = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,     74 * MB },
            { e_model.MODEL_BASE,    142 * MB },
            { e_model.MODEL_SMALL,   466 * MB },
            { e_model.MODEL_MEDIUM, 1464 * MB },
            { e_model.MODEL_LARGE,  2952 * MB },
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_MEMORY = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,     12 * MB },
            { e_model.MODEL_BASE,     24 * MB },
            { e_model.MODEL_SMALL,    70 * MB },
            { e_model.MODEL_MEDIUM,  184 * MB },
            { e_model.MODEL_LARGE,   306 * MB },
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_ENCODE = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,     80 * MB },
            { e_model.MODEL_BASE,    128 * MB },
            { e_model.MODEL_SMALL,   300 * MB },
            { e_model.MODEL_MEDIUM,  680 * MB },
            { e_model.MODEL_LARGE,  1100 * MB },
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_ENCODE_LAYER = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,    104 * MB },
            { e_model.MODEL_BASE,    138 * MB },
            { e_model.MODEL_SMALL,   208 * MB },
            { e_model.MODEL_MEDIUM,  280 * MB },
            { e_model.MODEL_LARGE,   354 * MB },
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_DECODE = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,    200 * MB },
            { e_model.MODEL_BASE,    202 * MB },
            { e_model.MODEL_SMALL,   204 * MB },
            { e_model.MODEL_MEDIUM,  206 * MB },
            { e_model.MODEL_LARGE,   208 * MB },
        };

        private static readonly IDictionary<e_model, size_t> MEM_REQ_DECODE_LAYER = new Dictionary<e_model, size_t>()
        {
            { e_model.MODEL_TINY,     32 * MB },
            { e_model.MODEL_BASE,     44 * MB },
            { e_model.MODEL_SMALL,    64 * MB },
            { e_model.MODEL_MEDIUM,   84 * MB },
            { e_model.MODEL_LARGE,   110 * MB },
        };

        #endregion

        #region Methods

        private static void whisper_free(whisper_context ctx)
        {
            if (ctx != null)
            {
                if (ctx.model.ctx != null)
                {
                    Ggml.Free(ctx.model.ctx);
                }

                if (ctx.model.ctx_mem != null)
                {
                    Ggml.Free(ctx.model.ctx_mem);
                }

                if (ctx.buf_model != null)
                {
                    ctx.buf_model.Free();
                }

                // delete ctx;
            }
        }

        private static int whisper_lang_id(string lang)
        {
            if (!g_lang.ContainsKey(lang))
            {
                foreach (var kv in g_lang)
                {
                    if (kv.Value.Value == lang)
                    {
                        return kv.Value.Key;
                    }
                }

                Console.WriteLine("{0}: unknown language '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, lang);
                return -1;
            }

            return g_lang[lang].Key;
        }

        private unsafe static bool whisper_model_load(string fname, whisper_context wctx)
        {
            string __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Console.WriteLine("{0}: loading model from '{1}'", __func__, fname);

            var model = wctx.model;
            var vocab = wctx.vocab;

            FileStream fileStream;
            BinaryReader fin;
            try
            {
                fileStream = File.Open(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
                fin = new BinaryReader(fileStream);
            }
            catch
            {
                Console.WriteLine("{0}: failed to open '{1}'", __func__, fname);
                return false;
            }

            // verify magic
            {
                read_safe(fin, out uint32_t magic);
                if (magic != 0x67676d6c)
                {
                    Console.WriteLine("{0}: invalid model file '{1}'", System.Reflection.MethodBase.GetCurrentMethod().Name, fname);
                    return false;
                }
            }

            //load hparams
            {
                var hparams = model.hparams;

                read_safe(fin, out hparams.n_vocab);
                read_safe(fin, out hparams.n_audio_ctx);
                read_safe(fin, out hparams.n_audio_state);
                read_safe(fin, out hparams.n_audio_head);
                read_safe(fin, out hparams.n_audio_layer);
                read_safe(fin, out hparams.n_text_ctx);
                read_safe(fin, out hparams.n_text_state);
                read_safe(fin, out hparams.n_text_head);
                read_safe(fin, out hparams.n_text_layer);
                read_safe(fin, out hparams.n_mels);
                read_safe(fin, out hparams.f16);

                Debug.Assert(hparams.n_text_state == hparams.n_audio_state);

                if (hparams.n_audio_layer == 4)
                {
                    model.type = e_model.MODEL_TINY;
                }

                if (hparams.n_audio_layer == 6)
                {
                    model.type = e_model.MODEL_BASE;
                }

                if (hparams.n_audio_layer == 12)
                {
                    model.type = e_model.MODEL_SMALL;
                }

                if (hparams.n_audio_layer == 24)
                {
                    model.type = e_model.MODEL_MEDIUM;
                }

                if (hparams.n_audio_layer == 32)
                {
                    model.type = e_model.MODEL_LARGE;
                }

                Console.WriteLine("{0}: n_vocab       = {1}", __func__, hparams.n_vocab);
                Console.WriteLine("{0}: n_audio_ctx   = {1}", __func__, hparams.n_audio_ctx);
                Console.WriteLine("{0}: n_audio_state = {1}", __func__, hparams.n_audio_state);
                Console.WriteLine("{0}: n_audio_head  = {1}", __func__, hparams.n_audio_head);
                Console.WriteLine("{0}: n_audio_layer = {1}", __func__, hparams.n_audio_layer);
                Console.WriteLine("{0}: n_text_ctx    = {1}", __func__, hparams.n_text_ctx);
                Console.WriteLine("{0}: n_text_state  = {1}", __func__, hparams.n_text_state);
                Console.WriteLine("{0}: n_text_head   = {1}", __func__, hparams.n_text_head);
                Console.WriteLine("{0}: n_text_layer  = {1}", __func__, hparams.n_text_layer);
                Console.WriteLine("{0}: n_mels        = {1}", __func__, hparams.n_mels);
                Console.WriteLine("{0}: f16           = {1}", __func__, hparams.f16);
                Console.WriteLine("{0}: type          = {1}", __func__, (int)model.type);

                wctx.buf_model.Resize(MEM_REQ_MODEL[model.type]);
                wctx.buf_memory.Resize(MEM_REQ_MEMORY[model.type]);
                wctx.buf_compute.Resize(Math.Max(MEM_REQ_ENCODE[model.type], MEM_REQ_DECODE[model.type]));
                wctx.buf_compute_layer.Resize(Math.Max(MEM_REQ_ENCODE_LAYER[model.type], MEM_REQ_DECODE_LAYER[model.type]));
            }

            // load mel filters
            {
                var filters = wctx.model.filters;

                read_safe(fin, out filters.n_mel);
                read_safe(fin, out filters.n_fft);

                filters.data.Resize(filters.n_mel * filters.n_fft);
                var span = new Span<byte>((byte*)filters.data.Data.ToPointer(), (int)filters.data.Size * sizeof(float));
                fin.Read(span);
            }

            // load vocab
            {
                int32_t n_vocab = 0;
                read_safe(fin, out n_vocab);

                //if (n_vocab != model.hparams.n_vocab) {
                //    fprintf(stderr, "%s: invalid model file '%s' (bad vocab size %d != %d)\n",
                //            __func__, fname.c_str(), n_vocab, model.hparams.n_vocab);
                //    return false;
                //}

                string word;

                for (var i = 0; i < n_vocab; i++)
                {
                    uint32_t len;
                    read_safe(fin, out len);

                    if (len > 0)
                    {
                        Span<byte> tmp = stackalloc byte[(int)len];
                        fin.Read(tmp); // read to buffer
                        word = Encoding.ASCII.GetString(tmp);
                    }
                    else
                    {
                        // seems like we have an empty-string token in multi-language models (i = 50256)
                        //fprintf(stderr, "%s: warning: empty-string token in vocab, i = %d\n", __func__, i);
                        word = "";
                    }

                    vocab.token_to_id[word] = i;
                    vocab.id_to_token[i] = word;

                    //printf("%s: vocab[%d] = '%s'\n", __func__, i, word.c_str());
                }

                vocab.n_vocab = model.hparams.n_vocab;
                if (vocab.is_multilingual())
                {
                    vocab.token_eot++;
                    vocab.token_sot++;
                    vocab.token_prev++;
                    vocab.token_solm++;
                    vocab.token_not++;
                    vocab.token_beg++;
                }

                if (n_vocab < model.hparams.n_vocab)
                {
                    Console.WriteLine("{0}: adding {1} extra tokens", System.Reflection.MethodBase.GetCurrentMethod().Name, model.hparams.n_vocab - n_vocab);
                    for (var i = n_vocab; i < model.hparams.n_vocab; i++)
                    {
                        if (i > vocab.token_beg)
                        {
                            word = $"[_TT_{i - vocab.token_beg}]";
                        }
                        else if (i == vocab.token_eot)
                        {
                            word = "[_EOT_]";
                        }
                        else if (i == vocab.token_sot)
                        {
                            word = "[_SOT_]";
                        }
                        else if (i == vocab.token_prev)
                        {
                            word = "[_PREV_]";
                        }
                        else if (i == vocab.token_not)
                        {
                            word = "[_NOT_]";
                        }
                        else if (i == vocab.token_beg)
                        {
                            word = "[_BEG_]";
                        }
                        else
                        {
                            word = $"[_extra_token_{i}]";
                        }

                        vocab.token_to_id[word] = i;
                        vocab.id_to_token[i] = word;
                    }
                }

                wctx.logits.Reserve(vocab.n_vocab*model.hparams.n_text_ctx);
                wctx.probs.Reserve(vocab.n_vocab*model.hparams.n_text_ctx);

                // vocab.probs_id.reserve(n_vocab);
                vocab.probs_id.Clear();
                vocab.probs_id.AddRange(new Pair<double, id>[n_vocab]);
            }

            {
                // this is the total memory required to run the inference
                size_t mem_required =
                        wctx.buf_model.Size +
                        wctx.buf_memory.Size +
                        wctx.buf_compute.Size +
                        wctx.buf_compute_layer.Size;

                Console.WriteLine("{0}: mem_required {1,7:F2} MB", __func__, mem_required / 1024.0 / 1024.0);
            }

            // for the big tensors, we have the option to store the data in 16-bit floats
            // in order to save memory and also to speed up the computation
            wctx.wtype = model.hparams.f16 > 0 ? GgmlType.F16 : GgmlType.F32;

            var wtype = wctx.wtype;

            size_t ctx_size = 0;

            {
                var hparams = model.hparams;

                var n_vocab = hparams.n_vocab;

                var n_audio_ctx   = hparams.n_audio_ctx;
                var n_audio_state = hparams.n_audio_state;
                var n_audio_layer = hparams.n_audio_layer;

                var n_text_ctx   = hparams.n_text_ctx;
                var n_text_state = hparams.n_text_state;
                var n_text_layer = hparams.n_text_layer;

                var n_mels = hparams.n_mels;

                // encoder
                {
                    ctx_size += n_audio_ctx * n_audio_state * Ggml.TypeSize(GgmlType.F32); // e_pe;

                    ctx_size += 3 * n_mels * n_audio_state * Ggml.TypeSize(wtype);        // e_conv_1_w
                    ctx_size +=              n_audio_state * Ggml.TypeSize(GgmlType.F32); // e_conv_1_b

                    ctx_size += 3 * n_audio_state * n_audio_state * Ggml.TypeSize(wtype);        // e_conv_2_w
                    ctx_size +=                     n_audio_state * Ggml.TypeSize(GgmlType.F32); // e_conv_2_b

                    ctx_size += n_audio_state * Ggml.TypeSize(GgmlType.F32); // e_ln_w;
                    ctx_size += n_audio_state * Ggml.TypeSize(GgmlType.F32); // e_ln_b;
                }

                // decoder
                {
                    ctx_size += n_text_ctx * n_text_state * Ggml.TypeSize(GgmlType.F32); // d_pe;

                    ctx_size += n_vocab * n_text_state * Ggml.TypeSize(wtype); // d_te;

                    ctx_size += n_text_state * Ggml.TypeSize(GgmlType.F32); // d_ln_w;
                    ctx_size += n_text_state * Ggml.TypeSize(GgmlType.F32); // d_ln_b;
                }

                // encoder layers
                {
                    ctx_size += n_audio_layer * (n_audio_state * Ggml.TypeSize(GgmlType.F32)); // mlp_ln_w
                    ctx_size += n_audio_layer * (n_audio_state * Ggml.TypeSize(GgmlType.F32)); // mlp_ln_b

                    ctx_size += n_audio_layer * (4 * n_audio_state * n_audio_state * Ggml.TypeSize(wtype));        // mlp_0_w
                    ctx_size += n_audio_layer * (                4 * n_audio_state * Ggml.TypeSize(GgmlType.F32)); // mlp_0_b

                    ctx_size += n_audio_layer * (4 * n_audio_state * n_audio_state * Ggml.TypeSize(wtype));        // mlp_1_w
                    ctx_size += n_audio_layer * (                    n_audio_state * Ggml.TypeSize(GgmlType.F32)); // mlp_1_b

                    ctx_size += n_audio_layer * (n_audio_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_0_w
                    ctx_size += n_audio_layer * (n_audio_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_0_b

                    ctx_size += n_audio_layer * (n_audio_state * n_audio_state * Ggml.TypeSize(wtype));        // attn_q_w
                    ctx_size += n_audio_layer * (                n_audio_state * Ggml.TypeSize(GgmlType.F32)); // attn_q_b

                    ctx_size += n_audio_layer * (n_audio_state * n_audio_state * Ggml.TypeSize(wtype)); // attn_k_w

                    ctx_size += n_audio_layer * (n_audio_state * n_audio_state * Ggml.TypeSize(wtype));        // attn_v_w
                    ctx_size += n_audio_layer * (                n_audio_state * Ggml.TypeSize(GgmlType.F32)); // attn_v_b

                    ctx_size += n_audio_layer * (n_audio_state * n_audio_state * Ggml.TypeSize(wtype));        // attn_ln_1_w
                    ctx_size += n_audio_layer * (                n_audio_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_1_b
                }

                // decoder layers
                {
                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // mlp_ln_w
                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // mlp_ln_b

                    ctx_size += n_text_layer * (4 * n_text_state * n_text_state * Ggml.TypeSize(wtype));        // mlp_0_w
                    ctx_size += n_text_layer * (               4 * n_text_state * Ggml.TypeSize(GgmlType.F32)); // mlp_0_b

                    ctx_size += n_text_layer * (4 * n_text_state * n_text_state * Ggml.TypeSize(wtype));        // mlp_1_w
                    ctx_size += n_text_layer * (                   n_text_state * Ggml.TypeSize(GgmlType.F32)); // mlp_1_b

                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_0_w
                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_0_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // attn_q_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // attn_q_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype)); // attn_k_w

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // attn_v_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // attn_v_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // attn_ln_1_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // attn_ln_1_b
                                                                                                        //
                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // cross_attn_ln_0_w
                    ctx_size += n_text_layer * (n_text_state * Ggml.TypeSize(GgmlType.F32)); // cross_attn_ln_0_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // cross_attn_q_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // cross_attn_q_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype)); // cross_attn_k_w

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // cross_attn_v_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // cross_attn_v_b

                    ctx_size += n_text_layer * (n_text_state * n_text_state * Ggml.TypeSize(wtype));        // cross_attn_ln_1_w
                    ctx_size += n_text_layer * (               n_text_state * Ggml.TypeSize(GgmlType.F32)); // cross_attn_ln_1_b
                }

                ctx_size += (15 + 15 * n_audio_layer + 24 * n_text_layer) * 256; // object overhead

                Console.WriteLine("{0}: ggml ctx size = {1,7:F2} MB", __func__, ctx_size / (1024.0 * 1024.0));
            }

            // create the ggml context
            {
                using var @params = new GgmlInitParams();
                @params.MemSize   = wctx.buf_model.Size;
                @params.MemBuffer = wctx.buf_model.Data;

                model.ctx = Ggml.Init(@params);
                if (model.ctx == null)
                {
                    Console.WriteLine("{0}: Ggml.Init() failed", __func__);
                    return false;
                }
            }

            // prepare memory for the weights
            {
                var ctx = model.ctx;

                var hparams = model.hparams;

                var n_vocab = hparams.n_vocab;

                var n_audio_ctx   = hparams.n_audio_ctx;
                var n_audio_state = hparams.n_audio_state;
                var n_audio_layer = hparams.n_audio_layer;

                var n_text_ctx   = hparams.n_text_ctx;
                var n_text_state = hparams.n_text_state;
                var n_text_layer = hparams.n_text_layer;

                var n_mels = hparams.n_mels;

                // model.layers_encoder.resize(n_audio_layer);
                model.layers_encoder.Clear();
                model.layers_encoder.AddRange(Enumerable.Range(0, n_audio_layer).Select(_ => new whisper_layer_encoder()));
                // model.layers_decoder.resize(n_text_layer);
                model.layers_decoder.Clear();
                model.layers_decoder.AddRange(Enumerable.Range(0, n_text_layer).Select(_ => new whisper_layer_decoder()));

                // encoder
                {
                    model.e_pe = Ggml.NewTensor2D(ctx, GgmlType.F32, n_audio_state, n_audio_ctx);

                    model.e_conv_1_w = Ggml.NewTensor3D(ctx, wtype,         3, n_mels, n_audio_state);
                    model.e_conv_1_b = Ggml.NewTensor2D(ctx, GgmlType.F32, 1, n_audio_state);

                    model.e_conv_2_w = Ggml.NewTensor3D(ctx, wtype,         3, n_audio_state, n_audio_state);
                    model.e_conv_2_b = Ggml.NewTensor2D(ctx, GgmlType.F32, 1, n_audio_state);

                    model.e_ln_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);
                    model.e_ln_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                    // map by name
                    model.tensors["encoder.positional_embedding"] = model.e_pe;

                    model.tensors["encoder.conv1.weight"] = model.e_conv_1_w;
                    model.tensors["encoder.conv1.bias"]   = model.e_conv_1_b;

                    model.tensors["encoder.conv2.weight"] = model.e_conv_2_w;
                    model.tensors["encoder.conv2.bias"]   = model.e_conv_2_b;

                    model.tensors["encoder.ln_post.weight"] = model.e_ln_w;
                    model.tensors["encoder.ln_post.bias"]   = model.e_ln_b;

                    for (var i = 0; i < n_audio_layer; ++i)
                    {
                        var layer = model.layers_encoder[i];

                        layer.mlp_ln_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);
                        layer.mlp_ln_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                        layer.mlp_0_w = Ggml.NewTensor2D(ctx, wtype,             n_audio_state, 4 * n_audio_state);
                        layer.mlp_0_b = Ggml.NewTensor1D(ctx, GgmlType.F32, 4 * n_audio_state);

                        layer.mlp_1_w = Ggml.NewTensor2D(ctx, wtype,         4 * n_audio_state, n_audio_state);
                        layer.mlp_1_b = Ggml.NewTensor1D(ctx, GgmlType.F32,     n_audio_state);

                        layer.attn_ln_0_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);
                        layer.attn_ln_0_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                        layer.attn_q_w = Ggml.NewTensor2D(ctx, wtype,         n_audio_state, n_audio_state);
                        layer.attn_q_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                        layer.attn_k_w = Ggml.NewTensor2D(ctx, wtype,         n_audio_state, n_audio_state);

                        layer.attn_v_w = Ggml.NewTensor2D(ctx, wtype,         n_audio_state, n_audio_state);
                        layer.attn_v_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                        layer.attn_ln_1_w = Ggml.NewTensor2D(ctx, wtype,         n_audio_state, n_audio_state);
                        layer.attn_ln_1_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_audio_state);

                        // map by name
                        model.tensors[$"encoder.blocks.{i}.mlp_ln.weight"] = layer.mlp_ln_w;
                        model.tensors[$"encoder.blocks.{i}.mlp_ln.bias"]   = layer.mlp_ln_b;

                        model.tensors[$"encoder.blocks.{i}.mlp.0.weight"] = layer.mlp_0_w;
                        model.tensors[$"encoder.blocks.{i}.mlp.0.bias"]   = layer.mlp_0_b;

                        model.tensors[$"encoder.blocks.{i}.mlp.2.weight"] = layer.mlp_1_w;
                        model.tensors[$"encoder.blocks.{i}.mlp.2.bias"]   = layer.mlp_1_b;

                        model.tensors[$"encoder.blocks.{i}.attn_ln.weight"] = layer.attn_ln_0_w;
                        model.tensors[$"encoder.blocks.{i}.attn_ln.bias"]   = layer.attn_ln_0_b;

                        model.tensors[$"encoder.blocks.{i}.attn.query.weight"] = layer.attn_q_w;
                        model.tensors[$"encoder.blocks.{i}.attn.query.bias"]   = layer.attn_q_b;

                        model.tensors[$"encoder.blocks.{i}.attn.key.weight"] = layer.attn_k_w;

                        model.tensors[$"encoder.blocks.{i}.attn.value.weight"] = layer.attn_v_w;
                        model.tensors[$"encoder.blocks.{i}.attn.value.bias"]   = layer.attn_v_b;

                        model.tensors[$"encoder.blocks.{i}.attn.out.weight"] = layer.attn_ln_1_w;
                        model.tensors[$"encoder.blocks.{i}.attn.out.bias"]   = layer.attn_ln_1_b;
                    }
                }

                // decoder
                {
                    model.d_pe = Ggml.NewTensor2D(ctx, GgmlType.F32, n_text_state, n_text_ctx);

                    model.d_te = Ggml.NewTensor2D(ctx, wtype, n_text_state, n_vocab);

                    model.d_ln_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);
                    model.d_ln_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                    // map by name
                    model.tensors["decoder.positional_embedding"] = model.d_pe;

                    model.tensors["decoder.token_embedding.weight"] = model.d_te;

                    model.tensors["decoder.ln.weight"] = model.d_ln_w;
                    model.tensors["decoder.ln.bias"]   = model.d_ln_b;

                    for (var i = 0; i < n_text_layer; ++i)
                    {
                        var layer = model.layers_decoder[i];

                        layer.mlp_ln_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);
                        layer.mlp_ln_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.mlp_0_w = Ggml.NewTensor2D(ctx, wtype,            n_text_state, 4 * n_text_state);
                        layer.mlp_0_b = Ggml.NewTensor1D(ctx, GgmlType.F32, 4 * n_text_state);

                        layer.mlp_1_w = Ggml.NewTensor2D(ctx, wtype,         4 * n_text_state, n_text_state);
                        layer.mlp_1_b = Ggml.NewTensor1D(ctx, GgmlType.F32,      n_text_state);

                        layer.attn_ln_0_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);
                        layer.attn_ln_0_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.attn_q_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.attn_q_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.attn_k_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);

                        layer.attn_v_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.attn_v_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.attn_ln_1_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.attn_ln_1_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.cross_attn_ln_0_w = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);
                        layer.cross_attn_ln_0_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.cross_attn_q_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.cross_attn_q_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.cross_attn_k_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);

                        layer.cross_attn_v_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.cross_attn_v_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        layer.cross_attn_ln_1_w = Ggml.NewTensor2D(ctx, wtype,         n_text_state, n_text_state);
                        layer.cross_attn_ln_1_b = Ggml.NewTensor1D(ctx, GgmlType.F32, n_text_state);

                        // map by name
                        model.tensors[$"decoder.blocks.{i}.mlp_ln.weight"] = layer.mlp_ln_w;
                        model.tensors[$"decoder.blocks.{i}.mlp_ln.bias"]   = layer.mlp_ln_b;

                        model.tensors[$"decoder.blocks.{i}.mlp.0.weight"] = layer.mlp_0_w;
                        model.tensors[$"decoder.blocks.{i}.mlp.0.bias"]   = layer.mlp_0_b;

                        model.tensors[$"decoder.blocks.{i}.mlp.2.weight"] = layer.mlp_1_w;
                        model.tensors[$"decoder.blocks.{i}.mlp.2.bias"]   = layer.mlp_1_b;

                        model.tensors[$"decoder.blocks.{i}.attn_ln.weight"] = layer.attn_ln_0_w;
                        model.tensors[$"decoder.blocks.{i}.attn_ln.bias"]   = layer.attn_ln_0_b;

                        model.tensors[$"decoder.blocks.{i}.attn.query.weight"] = layer.attn_q_w;
                        model.tensors[$"decoder.blocks.{i}.attn.query.bias"]   = layer.attn_q_b;

                        model.tensors[$"decoder.blocks.{i}.attn.key.weight"] = layer.attn_k_w;

                        model.tensors[$"decoder.blocks.{i}.attn.value.weight"] = layer.attn_v_w;
                        model.tensors[$"decoder.blocks.{i}.attn.value.bias"]   = layer.attn_v_b;

                        model.tensors[$"decoder.blocks.{i}.attn.out.weight"] = layer.attn_ln_1_w;
                        model.tensors[$"decoder.blocks.{i}.attn.out.bias"]   = layer.attn_ln_1_b;

                        model.tensors[$"decoder.blocks.{i}.cross_attn_ln.weight"] = layer.cross_attn_ln_0_w;
                        model.tensors[$"decoder.blocks.{i}.cross_attn_ln.bias"]   = layer.cross_attn_ln_0_b;

                        model.tensors[$"decoder.blocks.{i}.cross_attn.query.weight"] = layer.cross_attn_q_w;
                        model.tensors[$"decoder.blocks.{i}.cross_attn.query.bias"]   = layer.cross_attn_q_b;

                        model.tensors[$"decoder.blocks.{i}.cross_attn.key.weight"] = layer.cross_attn_k_w;

                        model.tensors[$"decoder.blocks.{i}.cross_attn.value.weight"] = layer.cross_attn_v_w;
                        model.tensors[$"decoder.blocks.{i}.cross_attn.value.bias"]   = layer.cross_attn_v_b;

                        model.tensors[$"decoder.blocks.{i}.cross_attn.out.weight"] = layer.cross_attn_ln_1_w;
                        model.tensors[$"decoder.blocks.{i}.cross_attn.out.bias"]   = layer.cross_attn_ln_1_b;
                    }
                }
            }

            // create the ggml memory context
            {
                using var @params = new GgmlInitParams();
                @params.MemSize   = wctx.buf_memory.Size;
                @params.MemBuffer = wctx.buf_memory.Data;

                model.ctx_mem = Ggml.Init(@params);
                if (model.ctx_mem == null)
                {
                    Console.WriteLine("{0}: Ggml.Init() failed", __func__);
                    return false;
                }
            }

            // key + value memory
            {
                var ctx = model.ctx_mem;

                var hparams = model.hparams;

                var n_text_state = hparams.n_text_state;
                var n_text_layer = hparams.n_text_layer;
                var n_text_ctx   = hparams.n_text_ctx;

                // key/value memory for the self-attention layer
                {
                    var n_mem      = n_text_layer * n_text_ctx;
                    var n_elements = n_text_state * n_mem;

                    model.memory_k = Ggml.NewTensor1D(ctx, wtype, n_elements);
                    model.memory_v = Ggml.NewTensor1D(ctx, wtype, n_elements);
                }

                // key/value memory for the cross-attention layer
                {
                    var n_audio_ctx = hparams.n_audio_ctx;

                    var n_mem      = n_text_layer * n_audio_ctx;
                    var n_elements = n_text_state * n_mem;

                    model.memory_cross_k = Ggml.NewTensor1D(ctx, wtype, n_elements);
                    model.memory_cross_v = Ggml.NewTensor1D(ctx, wtype, n_elements);
                }

                var memory_size =
                    Ggml.NBytes(model.memory_k)       + Ggml.NBytes(model.memory_v) +
                    Ggml.NBytes(model.memory_cross_k) + Ggml.NBytes(model.memory_cross_v);

                Console.WriteLine("{0}: memory size   = {1,7:F2} MB", __func__, memory_size / 1024.0 / 1024.0);
            }

            // load weights
            {
                size_t total_size = 0;

                model.n_loaded = 0;

                while (true)
                {
                    if (fin.BaseStream.Position == fin.BaseStream.Length)
                    {
                        break;
                    }

                    read_safe(fin, out int32_t n_dims);
                    read_safe(fin, out int32_t length);
                    read_safe(fin, out int32_t ftype);

                    // if (fin.eof()) {
                    //     break;
                    // }

                    int32_t nelements = 1;
                    var ne = new int32_t[]{ 1, 1, 1 };
                    for (var i = 0; i < n_dims; ++i)
                    {
                        read_safe(fin, out ne[i]);
                        nelements *= ne[i];
                    }

                    string name;
                    Span<byte> tmp = stackalloc byte[(int)length];
                    fin.Read(tmp); // read to buffer
                    name = Encoding.ASCII.GetString(tmp);

                    if (!model.tensors.ContainsKey(name))
                    {
                        Console.WriteLine("{0}: unknown tensor '{1}' in model file", __func__, name);
                        return false;
                    }

                    var tensor = model.tensors[name];
                    if (Ggml.NElements(tensor) != nelements)
                    {
                        Console.WriteLine("{0}: tensor '{1}' has wrong size in model file", __func__, name);
                        return false;
                    }

                    var tensorNe = tensor.NE;
                    if (tensorNe[0] != ne[0] || tensorNe[1] != ne[1] || tensorNe[2] != ne[2])
                    {
                        Console.WriteLine("{0}: tensor '{1}' has wrong shape in model file: got [{2}, {3}, {4}], expected [{5}, {6}, {7}]",
                                          __func__, name, tensorNe[0], tensorNe[1], tensorNe[2], ne[0], ne[1], ne[2]);
                        return false;
                    }

                    // ToDo: sizeof(ggml_fp16_t) should be 2 even though ARM is enabled
                    // var bpe = (ftype == 0) ? sizeof(float) : sizeof(ggml_fp16_t);
                    var bpe = (ftype == 0) ? sizeof(float) : 2;

                    if (nelements * bpe != Ggml.NBytes(tensor))
                    {
                        Console.WriteLine("{0}: tensor '{1}' has wrong size in model file: got {2}, expected {3}\n",
                                          __func__, name, Ggml.NBytes(tensor), nelements * bpe);
                        return false;
                    }

                    // fin.read(reinterpret_cast<char *>(tensor->data), Ggml.NBytes(tensor));
                    Span<byte> bytes = new Span<byte>(tensor.Data.ToPointer(), (int)Ggml.NBytes(tensor));
                    fin.Read(bytes);

                    //printf("%48s - [%5d, %5d, %5d], type = %6s, %6.2f MB\n", name.data(), ne[0], ne[1], ne[2], ftype == 0 ? "float" : "f16", Ggml.NBytes(tensor)/1024.0/1024.0);
                    total_size += Ggml.NBytes(tensor);
                    model.n_loaded++;
                }

                Console.WriteLine("{0}: model size    = {1,7:F2} MB", __func__, total_size / 1024.0 / 1024.0);

                if (model.n_loaded == 0)
                {
                    Console.WriteLine("{0}: WARN no tensors loaded from model file - assuming empty model for testing", __func__);
                }
                else if (model.n_loaded != (int)model.tensors.Count)
                {
                    Console.WriteLine("{0}: ERROR not all tensors loaded from model file - expected {1}, got {2}", __func__, model.tensors.Count, model.n_loaded);
                    return false;
                }
            }

            return true;
        }

        private static whisper_context whisper_init(string path_model)
        {
            Ggml.TimeInit();

            var ctx = new whisper_context();

            var t_start_us = Ggml.TimeMicrosecond();
            ctx.t_start_us = t_start_us;

            if (!whisper_model_load(path_model, ctx))
            {
                Console.WriteLine("{0}: failed to load model from '{1}'\n", System.Reflection.MethodBase.GetCurrentMethod().Name, path_model);
                return null;
            }

            ctx.t_load_us = Ggml.TimeMicrosecond() - t_start_us;

            return ctx;
        }

        private static void whisper_print_timings(whisper_context ctx)
        {
            string __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var t_end_us = Ggml.TimeMicrosecond();

            Console.WriteLine("");
            Console.WriteLine("{0}:     load time = {1,8:F2} ms", __func__, ctx.t_load_us / 1000.0f);
            Console.WriteLine("{0}:      mel time = {1,8:F2} ms", __func__, ctx.t_mel_us / 1000.0f);
            Console.WriteLine("{0}:   sample time = {1,8:F2} ms", __func__, ctx.t_sample_us / 1000.0f);
            Console.WriteLine("{0}:   encode time = {1,8:F2} ms / {2:F2} ms per layer", __func__, ctx.t_encode_us / 1000.0f, ctx.t_encode_us / 1000.0f/ ctx.model.hparams.n_audio_layer);
            Console.WriteLine("{0}:   decode time = {1,8:F2} ms / {2:F2} ms per layer", __func__, ctx.t_decode_us / 1000.0f, ctx.t_decode_us / 1000.0f/ ctx.model.hparams.n_text_layer);
            Console.WriteLine("{0}:    total time = {1,8:F2} ms", __func__, (t_end_us - ctx.t_start_us) / 1000.0f);
        }

        private static string whisper_print_system_info()
        {
            var s = "";

            s += "AVX = "       + (Ggml.CpuHasAVX()      ? "1" : "0")  + " | ";
            s += "AVX2 = "      + (Ggml.CpuHasAVX2()     ? "1" : "0")  + " | ";
            s += "AVX512 = "    + (Ggml.CpuHasAVX512()   ? "1" : "0")  + " | ";
            s += "FMA = "       + (Ggml.CpuHasFMA()      ? "1" : "0")  + " | ";
            s += "NEON = "      + (Ggml.CpuHasNEON()     ? "1" : "0")  + " | ";
            s += "ARM_FMA = "   + (Ggml.CpuHasArmFMA()   ? "1" : "0")  + " | ";
            s += "F16C = "      + (Ggml.CpuHasF16C()     ? "1" : "0")  + " | ";
            s += "FP16_VA = "   + (Ggml.CpuHasFP16VA()   ? "1" : "0")  + " | ";
            s += "WASM_SIMD = " + (Ggml.CpuHasWasmSIMD() ? "1" : "0")  + " | ";
            s += "BLAS = "      + (Ggml.CpuHasBLAS()     ? "1" : "0")  + " | ";

            return s;
        }

        private static int whisper_tokenize(whisper_context ctx,
                                            string text,
                                            IList<whisper_token> tokens,
                                            int n_max_tokens)
        {
            var res = tokenize(ctx.vocab, text);

            if (n_max_tokens < res.Count)
            {
                Console.WriteLine("{0}: too many resulting tokens: {1} (max {2})\n", System.Reflection.MethodBase.GetCurrentMethod(), (int)res.Count, n_max_tokens);
                return -1;
            }

            for (var i = 0; i < res.Count; i++)
            {
                // tokens[i] = res[i];
                tokens.Add(res[i]);
            }

            return res.Count;
        }

        private static int whisper_lang_max_id()
        {
            var max_id = 0;
            foreach (var kv in g_lang)
            {
                max_id = Math.Max(max_id, kv.Value.Key);
            }

            return max_id;
        }

        private static whisper_full_params whisper_full_default_params(whisper_sampling_strategy strategy)
        {
            var result = new whisper_full_params();
            switch (strategy)
            {
                case whisper_sampling_strategy.WHISPER_SAMPLING_GREEDY:
                    {
                        result.strategy         = whisper_sampling_strategy.WHISPER_SAMPLING_GREEDY;

                        result.n_threads        = Math.Min(4, (int32_t)System.Environment.ProcessorCount);
                        result.n_max_text_ctx   = 16384;
                        result.offset_ms        = 0;
                        result.duration_ms      = 0;

                        result.translate        = false;
                        result.no_context       = false;
                        result.single_segment   = false;
                        result.print_special    = false;
                        result.print_progress   = true;
                        result.print_realtime   = false;
                        result.print_timestamps = true;

                        result.token_timestamps = false;
                        result.thold_pt         = 0.01f;
                        result.thold_ptsum      = 0.01f;
                        result.max_len          = 0;
                        result.max_tokens       = 0;

                        result.speed_up         = false;
                        result.audio_ctx        = 0;

                        result.prompt_tokens    = null;
                        result.prompt_n_tokens  = 0;

                        result.language         = "en";

                        result.greedy.n_past = 0;

                        result.beam_search.n_past     = -1;
                        result.beam_search.beam_width = -1;
                        result.beam_search.n_best     = -1;

                        result.new_segment_callback           = null;
                        result.new_segment_callback_user_data = null;

                        result.encoder_begin_callback           = null;
                        result.encoder_begin_callback_user_data = null;
                    }
                    break;
                case whisper_sampling_strategy.WHISPER_SAMPLING_BEAM_SEARCH:
                    {
                        result.strategy         = whisper_sampling_strategy.WHISPER_SAMPLING_BEAM_SEARCH;

                        result.n_threads        = Math.Min(4, (int32_t)System.Environment.ProcessorCount);
                        result.n_max_text_ctx   = 16384;
                        result.offset_ms        = 0;
                        result.duration_ms      = 0;

                        result.translate        = false;
                        result.no_context       = false;
                        result.single_segment   = false;
                        result.print_special    = false;
                        result.print_progress   = true;
                        result.print_realtime   = false;
                        result.print_timestamps = true;

                        result.token_timestamps = false;
                        result.thold_pt         = 0.01f;
                        result.thold_ptsum      = 0.01f;
                        result.max_len          = 0;
                        result.max_tokens       = 0;

                        result.speed_up         = false;
                        result.audio_ctx        = 0;

                        result.prompt_tokens    = null;
                        result.prompt_n_tokens  = 0;

                        result.language         = "en";

                        result.greedy.n_past = -1;

                        result.beam_search.n_past     = 0;
                        result.beam_search.beam_width = 10;
                        result.beam_search.n_best     = 5;

                        result.new_segment_callback           = null;
                        result.new_segment_callback_user_data = null;

                        result.encoder_begin_callback           = null;
                        result.encoder_begin_callback_user_data = null;
                    }
                    break;
            }

            return result;
        }

        private static unsafe int whisper_full_parallel(whisper_context ctx,
                                                        whisper_full_params @params,
                                                        Buffer<float> samples,
                                                        int n_samples,
                                                        int n_processors)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (n_processors == 1)
                return whisper_full(ctx, @params, (float*)samples.Data.ToPointer(), n_samples);

            int ret = 0;

            // prepare separate contexts for each thread
            var ctxs = new whisper_context[n_processors - 1];

            for (var i = 0; i < n_processors - 1; ++i)
            {
                ctxs[i] = ctx;

                var model = ctxs[i].model;

                // create the ggml memory context
                {
                    using var initParams = new GgmlInitParams();
                    initParams.MemSize   = ctxs[i].buf_memory.Size;
                    initParams.MemBuffer = ctxs[i].buf_memory.Data;

                    model.ctx_mem = Ggml.Init(initParams);
                    if (model.ctx_mem == null)
                    {
                        Console.WriteLine($"{0}: Ggml.Init() failed", __func__);
                        // return false;
                        return 0;
                    }
                }

                // separate key + value memory for each processor
                {
                    var mctx = model.ctx_mem;

                    var hparams = model.hparams;

                    var n_text_state = hparams.n_text_state;
                    var n_text_layer = hparams.n_text_layer;
                    var n_text_ctx   = hparams.n_text_ctx;

                    // key/value memory for the self-attention layer
                    {
                        var n_mem      = n_text_layer * n_text_ctx;
                        var n_elements = n_text_state * n_mem;

                        model.memory_k = Ggml.NewTensor1D(mctx, ctx.wtype, n_elements);
                        model.memory_v = Ggml.NewTensor1D(mctx, ctx.wtype, n_elements);
                    }

                    // key/value memory for the cross-attention layer
                    {
                        var n_audio_ctx = hparams.n_audio_ctx;

                        var n_mem      = n_text_layer * n_audio_ctx;
                        var n_elements = n_text_state * n_mem;

                        model.memory_cross_k = Ggml.NewTensor1D(mctx, ctx.wtype, n_elements);
                        model.memory_cross_v = Ggml.NewTensor1D(mctx, ctx.wtype, n_elements);
                    }
                }
            }

            var offset_samples = (WHISPER_SAMPLE_RATE * @params.offset_ms) / 1000;
            var n_samples_per_processor = (n_samples - offset_samples) / n_processors;

            // the calling thread will process the first chunk
            // while the other threads will process the remaining chunks

            var workers = new Thread[n_processors - 1];
            for (int i = 0; i < n_processors - 1; ++i)
            {
                var start_samples = offset_samples + (i + 1) * n_samples_per_processor;
                var n_samples_cur = (i == n_processors - 2) ? n_samples - start_samples : n_samples_per_processor;

                var params_cur = @params;

                params_cur.offset_ms = 0;
                params_cur.print_progress = false;
                params_cur.print_realtime = false;

                params_cur.new_segment_callback = null;
                params_cur.new_segment_callback_user_data = null;

                var start = new ParameterizedThreadStart(whisper_full);
                workers[i] = new Thread(start);
                workers[i].Start(new Tuple<whisper_context, whisper_full_params, IntPtr, int>(ctxs[i], params_cur, samples.Data + start_samples, n_samples_cur));
            }

            {
                var params_cur = @params;

                ret = whisper_full(ctx, params_cur, (float*)samples.Data.ToPointer(), offset_samples + n_samples_per_processor);
            }

            for (var i = 0; i < n_processors - 1; ++i)
            {
                workers[i].Join();
            }

            var offset_t = (int64_t)(@params.offset_ms / 10.0);

            // combine results into ctx.result_all
            for (var i = 0; i < n_processors - 1; ++i)
            {
                var results_i = ctxs[i].result_all;

                foreach (var result in results_i)
                {
                    // correct the segment timestamp taking into account the offset
                    result.t0 += 100 * ((i + 1) * n_samples_per_processor) / WHISPER_SAMPLE_RATE + offset_t;
                    result.t1 += 100 * ((i + 1) * n_samples_per_processor) / WHISPER_SAMPLE_RATE + offset_t;

                    // make sure that segments are not overlapping
                    if (ctx.result_all.Any())
                    {
                        result.t0 = Math.Max(result.t0, ctx.result_all.Last().t1);
                    }

                    ctx.result_all.Add(result);

                    // call the new_segment_callback for each segment
                    if (@params.new_segment_callback != null)
                    {
                        @params.new_segment_callback(ctx, 1, @params.new_segment_callback_user_data);
                    }
                }

                ctx.t_mel_us    += ctxs[i].t_mel_us;
                ctx.t_sample_us += ctxs[i].t_sample_us;
                ctx.t_encode_us += ctxs[i].t_encode_us;
                ctx.t_decode_us += ctxs[i].t_decode_us;
            }

            // average the timings
            ctx.t_mel_us    /= n_processors;
            ctx.t_sample_us /= n_processors;
            ctx.t_encode_us /= n_processors;
            ctx.t_decode_us /= n_processors;

            // print information about the audio boundaries
            Console.WriteLine("");
            Console.WriteLine($"{0}: the audio has been split into {1} chunks at the following times:", __func__, n_processors);
            for (int i = 0; i < n_processors - 1; ++i)
            {
                Console.WriteLine($"{0}: split {1} - {2}", __func__, (i + 1), to_timestamp(100 * ((i + 1) * n_samples_per_processor) / WHISPER_SAMPLE_RATE + offset_t));
            }
            Console.WriteLine($"{0}: the transcription quality may be degraded near these boundaries", __func__);

            return ret;
        }

        private static int whisper_lang_auto_detect(whisper_context ctx,
                                                    int offset_ms,
                                                    int n_threads,
                                                    float[] lang_probs)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var seek = offset_ms / 10;

            if (seek < 0)
            {
                Console.WriteLine($"{0}: offset {1}ms is before the start of the audio", __func__, offset_ms);
                return -1;
            }

            if (seek >= ctx.mel.n_len)
            {
                Console.WriteLine($"{0}: offset {1}ms is past the end of the audio ({2}ms)", __func__, offset_ms, ctx.mel.n_len * 10);
                return -2;
            }

            // run the encoder
            if (whisper_encode(ctx, seek, n_threads) != 0)
            {
                Console.WriteLine($"{0}: failed to encode", __func__);
                return -6;
            }

            var prompt = new whisper_token[] { whisper_token_sot(ctx) };

            if (whisper_decode(ctx, prompt, prompt.Length, 0, n_threads) != 0)
            {
                Console.WriteLine($"{0}: failed to decode", __func__);
                return -7;
            }

            var probs_id = new List<Pair<float, int>>();
            foreach (var kv in g_lang)
            {
                var token_lang = whisper_token_lang(ctx, kv.Value.Key);
                probs_id.Add(new Pair<float, int>(ctx.probs[token_lang], kv.Value.Key));
            }

            // sort descending
            {
                // using pair_type = decltype(probs_id)::value_type;
                // std::sort(probs_id.begin(), probs_id.end(), [](const pair_type & a, const pair_type & b) {
                //     return a.first > b.first;
                // });
                probs_id.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            }

            // softmax
            {
                float sum = 0;
                foreach (var kv in probs_id)
                {
                    sum += (float)Math.Exp(kv.Item1);
                }

                for (var i = 0; i < probs_id.Count; i++)
                {
                    var kv = probs_id[i];
                    kv.Item1 = (float)Math.Exp(kv.Item1) / sum;
                }
            }

            {
                foreach (var prob in probs_id)
                {
                    if (lang_probs != null)
                    {
                        lang_probs[prob.Item2] = prob.Item1;
                    }

                    //printf("%s: lang %2d (%3s): %f\n", __func__, prob.second, whisper_lang_str(prob.second), prob.first);
                }
            }

            return probs_id[0].Item2;
        }

        // wrap the last segment to max_len characters
        // returns the number of new segments
        private static int whisper_wrap_segment(whisper_context ctx, int max_len)
        {
            var segment = ctx.result_all.Last();

            int res = 1;
            int acc = 0;

            var text = "";

            for (var i = 0; i < (int) segment.tokens.Count; i++)
            {
                var token = segment.tokens[i];
                if (token.id >= whisper_token_eot(ctx))
                {
                    continue;
                }

                var txt = whisper_token_to_str(ctx, token.id);

                var cur = txt.Length;

                if (acc + cur > max_len && i > 0)
                {
                    // split here
                    ctx.result_all.Last().text = text;
                    ctx.result_all.Last().t1 = token.t0;
                    ctx.result_all.Last().tokens.RemoveRange(i, ctx.result_all.Last().tokens.Count - i);

                    ctx.result_all.Add(new whisper_segment());
                    ctx.result_all.Last().t0 = token.t0;
                    ctx.result_all.Last().t1 = segment.t1;

                    // add tokens [i, end] to the new segment
                    ctx.result_all.Last().tokens.Insert(
                            ctx.result_all.Last().tokens.Count,
                            segment.tokens[i]);

                    acc = 0;
                    text = "";

                    segment = ctx.result_all.Last();
                    i = -1;

                    res++;
                }
                else
                {
                    acc += cur;
                    text += txt;
                }
            }

            ctx.result_all.Last().text = text;

            return res;
        }

        private static unsafe void whisper_full(object obj)
        {
            var tuple = (Tuple<whisper_context, whisper_full_params, IntPtr, int>)obj;
            whisper_full(tuple.Item1, tuple.Item2, (float*)tuple.Item3.ToPointer(), tuple.Item4);
        }

        private static unsafe int whisper_full(whisper_context ctx,
                                               whisper_full_params @params,
                                               float* samples,
                                               int n_samples)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            // clear old results
            var result_all = ctx.result_all;

            result_all.Clear();

            // compute log mel spectrogram
            if (@params.speed_up)
            {
                if (whisper_pcm_to_mel_phase_vocoder(ctx, samples, n_samples, @params.n_threads) != 0)
                {
                    Console.WriteLine($"{0}: failed to compute log mel spectrogram", __func__);
                    return -1;
                }
            }
            else
            {
                if (whisper_pcm_to_mel(ctx, samples, n_samples, @params.n_threads) != 0)
                {
                    Console.WriteLine($"{0}: failed to compute log mel spectrogram", __func__);
                    return -2;
                }
            }

            // auto-detect language if not specified
            if (@params.language == null || @params.language.Length == 0 || string.Equals(@params.language, "auto"))
            {
                var probs = new float[whisper_lang_max_id() + 1];

                var lang_id = whisper_lang_auto_detect(ctx, 0, @params.n_threads, probs);
                if (lang_id < 0)
                {
                    Console.WriteLine($"{0}: failed to auto-detect language", __func__);
                    return -3;
                }

                @params.language = whisper_lang_str(lang_id);

                Console.WriteLine($"{0}: auto-detected language: {1} (p = {2})", __func__, @params.language, probs[whisper_lang_id(@params.language)]);
            }

            if (@params.token_timestamps)
            {
                ctx.t_beg = 0;
                ctx.t_last = 0;
                ctx.tid_last = 0;
                ctx.energy = get_signal_energy(samples, n_samples, 32);
            }

            var seek_start = @params.offset_ms / 10;
            var seek_end = seek_start + (@params.duration_ms == 0 ? whisper_n_len(ctx) : @params.duration_ms / 10);

            // if length of spectrogram is less than 1s (100 samples), then return
            // basically don't process anything that is less than 1s
            // see issue #39: https://github.com/ggerganov/whisper.cpp/issues/39
            if (seek_end < 100 + seek_start)
            {
                return 0;
            }

            // the accumulated text context so far
            var prompt_past = ctx.prompt_past;
            if (@params.no_context)
            {
                prompt_past.Clear();
            }

            // prepend the prompt tokens to the prompt_past
            if (@params.prompt_tokens != null && @params.prompt_n_tokens > 0)
            {
                // parse tokens from the pointer
                for (var i = 0; i < @params.prompt_n_tokens; i++)
                {
                    prompt_past.Add(@params.prompt_tokens[i]);
                }
                // std::rotate(prompt_past.begin(), prompt_past.end() - @params.prompt_n_tokens, prompt_past.end());
                Rotate(prompt_past, -@params.prompt_n_tokens);
            }

            // overwrite audio_ctx, max allowed is hparams.n_audio_ctx
            if (@params.audio_ctx > whisper_n_audio_ctx(ctx)) {

                Console.WriteLine($"{0}: audio_ctx is larger than the maximum allowed ({1} > {2})", __func__, @params.audio_ctx, whisper_n_audio_ctx(ctx));
                return -4;
            }
            ctx.exp_n_audio_ctx = @params.audio_ctx;

            // these tokens determine the task that will be performed
            var prompt_init = new List<whisper_token>(new whisper_token[]{ whisper_token_sot(ctx) });
            if (whisper_is_multilingual(ctx) > 0)
            {
                var lang_id = whisper_lang_id(@params.language);
                prompt_init.Add(whisper_token_lang(ctx, lang_id));
                if (@params.translate)
                {
                    prompt_init.Add(whisper_token_translate());
                }
                else
                {
                    prompt_init.Add(whisper_token_transcribe());
                }
            }

            int progress_prev = 0;
            int progress_step = 5;

            var tokens_cur = new List<whisper_token_data>();
            tokens_cur.AddRange(Enumerable.Range(0, whisper_n_text_ctx(ctx)).Select(i => new whisper_token_data()));

            var prompt = new List<whisper_token>();
            prompt.AddRange(Enumerable.Range(0, whisper_n_text_ctx(ctx)));

            // main loop
            int seek = seek_start;
            while (true)
            {
                var progress_cur = (100 * (seek - seek_start)) / (seek_end - seek_start);
                while (progress_cur >= progress_prev + progress_step)
                {
                    progress_prev += progress_step;
                    if (@params.print_progress)
                    {
                        Console.WriteLine($"{0}: progress = {1:D3}%", __func__, progress_prev);
                    }
                }

                // of only 1 second left, then stop
                if (seek + 100 >= seek_end)
                {
                    break;
                }

                // if there is a very short audio segment left to process, we remove any past prompt since it tends
                // to confuse the decoder and often make it repeat or hallucinate stuff
                if (seek > seek_start && seek + 500 >= seek_end)
                {
                    prompt_past.Clear();
                }

                if (@params.encoder_begin_callback != null)
                {
                    if (@params.encoder_begin_callback(ctx, @params.encoder_begin_callback_user_data) == false)
                    {
                        Console.WriteLine($"{0}: encoder_begin_callback returned false - aborting", __func__);
                        break;
                    }
                }

                // encode audio features starting at offset seek
                if (whisper_encode(ctx, seek, @params.n_threads) != 0)
                {
                    Console.WriteLine($"{0}: failed to encode", __func__);
                    return -4;
                }

                int n_past = 0;
                prompt.Clear();

                // if we have already generated some text, use it as a prompt to condition the next generation
                if (prompt_past.Any())
                {
                    var n_take = Math.Min(Math.Min(@params.n_max_text_ctx, whisper_n_text_ctx(ctx) / 2), (int)(prompt_past.Count));

                    prompt = new List<whisper_token>(new []{ whisper_token_prev(ctx) });
                    prompt.Insert(1, prompt_past[prompt_past.Count - n_take]);

                    prompt_past.Clear();
                    prompt_past.Insert(prompt_past.Count, prompt[1]);
                }

                prompt.Insert(prompt.Count, prompt_init.First());

                int seek_delta = 100 * WHISPER_CHUNK_SIZE;

                // print the prompt
                //printf("\n\n");
                //for (int i = 0; i < prompt.size(); i++) {
                //    printf("%s: prompt[%d] = %s\n", __func__, i, ctx.vocab.id_to_token[prompt[i]].c_str());
                //}
                //printf("\n\n");

                // the accumulated transcription in the current interation
                var result_len = 0;
                tokens_cur.Clear();

                var failed = false;
                var has_ts = false; // have we already sampled a non-beg timestamp token for the current segment?

                for (int i = 0, n_max = whisper_n_text_ctx(ctx) / 2 - 4; i < n_max; ++i)
                {
                    if (whisper_decode(ctx, prompt, prompt.Count, n_past, @params.n_threads) != 0)
                    {
                        Console.WriteLine($"{0}: failed to decode", __func__);
                        return -5;
                    }

                    n_past += prompt.Count;
                    prompt.Clear();

                    // very basic greedy sampling strategy:
                    //
                    //   - always take the most probable token
                    //
                    // more sophisticated sampling strategies could be implemented here, but we keep it simple
                    // feel free to experiment!
                    //
                    {
                        var token = (i == 0) ? whisper_sample_timestamp(ctx, true) : whisper_sample_best(ctx);

                        // timestamp token - update sliding window
                        if (token.id > whisper_token_beg(ctx))
                        {
                            var seek_delta_new = 2 * (token.id - whisper_token_beg(ctx));

                            // do not allow to go back in time
                            if (has_ts && seek_delta > seek_delta_new && result_len < i)
                            {
                                break;
                            }

                            seek_delta = seek_delta_new;
                            result_len = i + 1;
                            has_ts = true;
                        }

                        // add it to the context
                        prompt.Add(token.id);
                        tokens_cur.Add(token);

                        //{
                        //    const auto tt = token.pt > 0.10 ? ctx.vocab.id_to_token[token.tid] : "[?]";
                        //    printf("%s: %3d %10s %6d %6.3f '%s'\n", __func__, i, tt.c_str(), token.id, token.pt, ctx.vocab.id_to_token[token.id].c_str());
                        //}

                        // end of segment
                        if (token.id == whisper_token_eot(ctx) ||                // end of text token
                            (@params.max_tokens > 0 && i >= @params.max_tokens) || // max tokens per segment reached
                            (has_ts && seek + seek_delta + 100 >= seek_end)      // end of audio reached
                            ) {
                            if (result_len == 0)
                            {
                                if (seek + seek_delta + 100 >= seek_end)
                                {
                                    result_len = i + 1;
                                }
                                else
                                {
                                    failed = true;
                                    break;
                                }
                            }

                            if (@params.single_segment)
                            {
                                result_len = i + 1;
                                seek_delta = 100 * WHISPER_CHUNK_SIZE;
                            }

                            break;
                        }

                        // TESTS: if no tensors are loaded, it means we are running tests
                        if (ctx.model.n_loaded == 0)
                        {
                            seek_delta = 100 * WHISPER_CHUNK_SIZE;
                            break;
                        }
                    }

                    // sometimes, the decoding can get stuck in a repetition loop
                    // this is a simple strategy to avoid such cases - we simply flag the decoding as failed and advance
                    // the sliding window by 1 second
                    if (i == n_max - 1 && (result_len == 0 || seek_delta < 100 * WHISPER_CHUNK_SIZE/2))
                    {
                        failed = true;
                        break;
                    }
                }

                if (failed) {
                    // when we fail to sample timestamp token, retry by clearing the past prompt
                    // if it fails again, then we advance the window by 1 second
                    if (prompt_past.Any())
                    {
                        prompt_past.Clear();
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine($"{0}: failed to generate timestamp token - skipping one second", __func__);
                        Console.WriteLine("");
                        seek += 100;
                    }
                    continue;
                }

                // shrink down to result_len
                // tokens_cur.resize(result_len);
                tokens_cur.RemoveRange(result_len, tokens_cur.Count - result_len);

                foreach (var r in tokens_cur)
                {
                    prompt_past.Add(r.id);
                }

                // store the text from this iteration
                if (tokens_cur.Any())
                {
                    int i0 = 0;
                    var t0 = seek + 2 * (tokens_cur.First().tid - whisper_token_beg(ctx));

                    var text = "";

                    for (int i = 0; i < (int) tokens_cur.Count; i++)
                    {
                        //printf("%s: %18s %6.3f %18s %6.3f\n", __func__,
                        //        ctx.vocab.id_to_token[tokens_cur[i].id].c_str(), tokens_cur[i].p,
                        //        ctx.vocab.id_to_token[tokens_cur[i].tid].c_str(), tokens_cur[i].pt);

                        if (@params.print_special == false && tokens_cur[i].id >= whisper_token_eot(ctx))
                        {
                        }
                        else
                        {
                            text += whisper_token_to_str(ctx, tokens_cur[i].id);
                        }

                        if (tokens_cur[i].id > whisper_token_beg(ctx) && !@params.single_segment)
                        {
                            var t1 = seek + 2 * (tokens_cur[i].tid - whisper_token_beg(ctx));
                            if (!string.IsNullOrEmpty(text))
                            {
                                var tt0 = @params.speed_up ? 2*t0 : t0;
                                var tt1 = @params.speed_up ? 2*t1 : t1;

                                if (@params.print_realtime)
                                {
                                    if (@params.print_timestamps)
                                    {
                                        Console.WriteLine($"[{0} --> {1}]  {2}", to_timestamp(tt0), to_timestamp(tt1), text);
                                    }
                                    else
                                    {
                                        Console.Write($"{0}", text);
                                        // fflush(stdout);
                                    }
                                }

                                result_all.Add(new whisper_segment
                                {
                                     t0 = tt0,
                                     t1 = tt1,
                                     text = text
                                });
                                for (var j = i0; j <= i; j++)
                                {
                                    result_all.Last().tokens.Add(tokens_cur[j]);
                                }

                                int n_new = 1;

                                if (@params.token_timestamps)
                                {
                                    whisper_exp_compute_token_level_timestamps(
                                            ctx, result_all.Count - 1, @params.thold_pt, @params.thold_ptsum);

                                    if (@params.max_len > 0)
                                    {
                                        n_new = whisper_wrap_segment(ctx, @params.max_len);
                                    }
                                }
                                if (@params.new_segment_callback != null)
                                {
                                    @params.new_segment_callback(ctx, n_new, @params.new_segment_callback_user_data);
                                }
                            }
                            text = "";
                            while (i < (int)tokens_cur.Count && tokens_cur[i].id > whisper_token_beg(ctx))
                            {
                                i++;
                            }
                            i--;
                            t0 = t1;
                            i0 = i + 1;
                        }
                    }

                    if (!string.IsNullOrEmpty(text))
                    {
                        var t1 = seek + seek_delta;

                        var tt0 = @params.speed_up ? 2 * t0 : t0;
                        var tt1 = @params.speed_up ? 2 * t1 : t1;

                        if (@params.print_realtime)
                        {
                            if (@params.print_timestamps)
                            {
                                Console.WriteLine($"[{0} --> {1}]  {2}", to_timestamp(tt0), to_timestamp(tt1), text);
                            }
                            else
                            {
                                Console.Write("{0}", text);
                                // fflush(stdout);
                            }
                        }

                        result_all.Add(new whisper_segment
                        {
                            t0 = tt0,
                            t1 = tt1,
                            text = text
                        });
                        for (var j = i0; j < (int) tokens_cur.Count; j++)
                        {
                            result_all.Last().tokens.Add(tokens_cur[j]);
                        }

                        int n_new = 1;

                        if (@params.token_timestamps)
                        {
                            whisper_exp_compute_token_level_timestamps(
                                    ctx, result_all.Count - 1, @params.thold_pt, @params.thold_ptsum);

                            if (@params.max_len > 0)
                            {
                                n_new = whisper_wrap_segment(ctx, @params.max_len);
                            }
                        }
                        if (@params.new_segment_callback != null)
                        {
                            @params.new_segment_callback(ctx, n_new, @params.new_segment_callback_user_data);
                        }
                    }
                }

                seek += seek_delta;
            }

            return 0;
        }

        private static string whisper_lang_str(int id)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            foreach (var kv in g_lang)
            {
                if (kv.Value.Key == id)
                {
                    return kv.Key;
                }
            }

            Console.WriteLine($"{0}: unknown language id {1}", __func__, id);

            return null;
        }

        private static unsafe int whisper_pcm_to_mel(whisper_context ctx, float* samples, int n_samples, int n_threads)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var t_start_us = Ggml.TimeMicrosecond();

            if (!log_mel_spectrogram(samples, n_samples, WHISPER_SAMPLE_RATE, WHISPER_N_FFT, WHISPER_HOP_LENGTH, WHISPER_N_MEL, n_threads, ctx.model.filters, false, ctx.mel))
            {
                Console.WriteLine($"{0}: failed to compute mel spectrogram", __func__);
                return -1;
            }

            ctx.t_mel_us = Ggml.TimeMicrosecond() - t_start_us;

            return 0;
        }

        // same as whisper_pcm_to_mel, but applies a Phase Vocoder to speed up the audio x2
        private static unsafe int whisper_pcm_to_mel_phase_vocoder(whisper_context ctx, float* samples, int n_samples, int n_threads)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var t_start_us = Ggml.TimeMicrosecond();

            if (!log_mel_spectrogram(samples, n_samples, WHISPER_SAMPLE_RATE, 2 * WHISPER_N_FFT, 2 * WHISPER_HOP_LENGTH, WHISPER_N_MEL, n_threads, ctx.model.filters, true, ctx.mel))
            {
                Console.WriteLine($"{0}: failed to compute mel spectrogram", __func__);
                return -1;
            }

            ctx.t_mel_us = Ggml.TimeMicrosecond() - t_start_us;

            return 0;
        }

        private static int whisper_set_mel(whisper_context ctx,
                                           float[] data,
                                           int n_len,
                                           int n_mel)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (n_mel != WHISPER_N_MEL)
            {
                Console.WriteLine($"{0}: invalid number of mel bands: {1} (expected {2})", __func__, n_mel, WHISPER_N_MEL);
                return -1;
            }

            ctx.mel.n_len = n_len;
            ctx.mel.n_mel = n_mel;

            ctx.mel.data.Resize(n_len * n_mel);
            Marshal.Copy(data, 0, ctx.mel.data.Data, n_len * n_mel);
            // memcpy(ctx.mel.data.Data, data, n_len * n_mel * sizeof(float));

            return 0;
        }

        
        // evaluate the encoder
        //
        // given audio recording (more specifically, its log mel spectrogram), runs forward pass of the encoder
        // part of the transformer model and returns the encoded features
        //
        //   - model:      the model
        //   - n_threads:  number of threads to use
        //   - mel_offset: offset in the mel spectrogram (i.e. audio offset)
        //
        private static unsafe bool whisper_encode2(whisper_context wctx,
                                                   int n_threads,
                                                   int mel_offset)
        {
            var model   = wctx.model;
            var mel_inp = wctx.mel;
            var hparams = model.hparams;

            var n_ctx   = wctx.exp_n_audio_ctx > 0 ? wctx.exp_n_audio_ctx : hparams.n_audio_ctx;
            var n_state = hparams.n_audio_state;
            var n_head  = hparams.n_audio_head;
            var n_layer = hparams.n_audio_layer;

            var n_mels = hparams.n_mels;
            Debug.Assert(mel_inp.n_mel == n_mels);

            using var @params = new GgmlInitParams();
            @params.MemSize   = wctx.buf_compute.Size;
            @params.MemBuffer = wctx.buf_compute.Data;

            var ctx0 = Ggml.Init(@params);

            var mel = Ggml.NewTensor2D(ctx0, GgmlType.F32, 2 * n_ctx, n_mels);
            Debug.Assert(mel.Type == GgmlType.F32);
            {
                var dst = (float*)mel.Data.ToPointer();
                Std.MemSet(mel.Data, 0, Ggml.NBytes(mel));

                var i0 = Math.Min(mel_offset, mel_inp.n_len);
                var i1 = Math.Min(mel_offset + 2 * n_ctx, mel_inp.n_len);

                for (var j = 0; j < mel_inp.n_mel; ++j)
                {
                    for (var i = i0; i < i1; ++i)
                    {
                        dst[j*2*n_ctx + (i - i0)] = mel_inp.data[j * mel_inp.n_len + i];
                    }
                }
            }

            GgmlTensor cur = null;

            // convolution + gelu
            {
                cur = Ggml.Conv1D1S(ctx0, model.e_conv_1_w, mel);
                cur = Ggml.Add(ctx0,
                        Ggml.Repeat(ctx0,
                            model.e_conv_1_b,
                            cur),
                        cur);

                cur = Ggml.GELU(ctx0, cur);

                cur = Ggml.Conv1D2S(ctx0, model.e_conv_2_w, cur);
                cur = Ggml.Add(ctx0,
                        Ggml.Repeat(ctx0,
                            model.e_conv_2_b,
                            cur),
                        cur);

                cur = Ggml.GELU(ctx0, cur);
            }

            // ===================================================================
            // NOTE: experimenting with partial evaluation of the encoder (ignore)
            //static int iter = -1;
            //const int n_iter = 1500/n_ctx;

            //iter = (iter + 1) % n_iter;

            //if (iter == 0) {
            //    memset(model.memory_cross_k->data, 0, Ggml.NBytes(model.memory_cross_k));
            //    memset(model.memory_cross_v->data, 0, Ggml.NBytes(model.memory_cross_v));
            //}

            // static int iter = 0;

            var e_pe_stride = model.e_pe.NE[0] * Ggml.ElementSize(model.e_pe);
            var e_pe_offset = model.e_pe.NE[0] * Ggml.ElementSize(model.e_pe) * n_ctx * iter;

            var e_pe = Ggml.View2D(ctx0, model.e_pe, model.e_pe.NE[0], n_ctx, e_pe_stride, e_pe_offset);

            cur = Ggml.Add(ctx0, e_pe, Ggml.Transpose(ctx0, cur));
            // ===================================================================

            // original:
            //cur = Ggml.Add(ctx0, model.e_pe, Ggml.Transpose(ctx0, cur));

            var inpL = cur;

            for (var il = 0; il < n_layer; ++il)
            {
                var layer = model.layers_encoder[il];

                // create separate context for each layer to reduce memory usage

                using var paramsL = new GgmlInitParams();
                paramsL.MemSize   = wctx.buf_compute_layer.Size;
                paramsL.MemBuffer = wctx.buf_compute_layer.Data;

                var ctxL = Ggml.Init(paramsL);

                // norm
                {
                    cur = Ggml.Norm(ctxL, inpL);

                    // cur = ln_0_w*cur + ln_0_b
                    cur = Ggml.Add(ctxL,
                            Ggml.Mul(ctxL,
                                Ggml.Repeat(ctxL, layer.attn_ln_0_w, cur),
                                cur),
                            Ggml.Repeat(ctxL, layer.attn_ln_0_b, cur));
                }

                // self-attention
                {
                    var Qcur = Ggml.MulMat(ctxL,
                            layer.attn_q_w,
                            cur);

                    Qcur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL,
                                layer.attn_q_b,
                                Qcur),
                            Qcur);

                    //Qcur = Ggml.Scale(ctxL, Qcur, Ggml.NewF32(ctxL, pow(float(n_state)/n_head, -0.25)));

                    // note: no bias for Key
                    var Kcur = Ggml.MulMat(ctxL,
                            layer.attn_k_w,
                            cur);

                    //Kcur = Ggml.Scale(ctxL, Kcur, Ggml.NewF32(ctxL, pow(float(n_state)/n_head, -0.25)));

                    var Vcur = Ggml.MulMat(ctxL,
                            layer.attn_v_w,
                            cur);

                    Vcur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL,
                                layer.attn_v_b,
                                Vcur),
                            Vcur);

                    // ------

#if USE_FLASH_ATTN
                    var Q =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Qcur,
                                    Ggml.NewTensor3D(ctxL, wctx.wtype, n_state / n_head, n_head, n_ctx)),
                                0, 2, 1, 3);

                    var K =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Kcur,
                                    Ggml.NewTensor3D(ctxL, wctx.wtype, n_state / n_head, n_head, n_ctx)),
                                0, 2, 1, 3);

                    var V =
                        Ggml.Cpy(ctxL,
                                Ggml.Permute(ctxL,
                                    Ggml.Reshape3D(ctxL,
                                        Vcur,
                                        n_state / n_head, n_head, n_ctx),
                                    1, 2, 0, 3),
                                Ggml.NewTensor3D(ctxL, wctx.wtype, n_ctx, n_state / n_head, n_head)
                                );

                    var KQV = Ggml.FlashAttn(ctxL, Q, K, V, false);
#else
                    var Q =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Qcur,
                                    Ggml.NewTensor3D(ctxL, GgmlType.F32, n_state / n_head, n_head, n_ctx)),
                                0, 2, 1, 3);

                    var K =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Kcur,
                                    Ggml.NewTensor3D(ctxL, wctx.wtype, n_state / n_head, n_head, n_ctx)),
                                0, 2, 1, 3);

                    // K * Q
                    var KQ = Ggml.MulMat(ctxL, K, Q);

                    var KQ_scaled =
                        Ggml.Scale(ctxL,
                                KQ,
                                Ggml.NewF32(ctxL, 1.0f / (float)Math.Sqrt((n_state)/n_head))
                                );

                    var KQ_soft_max = Ggml.SoftMax(ctxL, KQ_scaled);

                    //var V_trans =
                    //    Ggml.Permute(ctxL,
                    //            Ggml.Cpy(ctxL,
                    //                Vcur,
                    //                Ggml.NewTensor3D(ctxL, wctx.wtype, n_state/n_head, n_head, n_ctx)),
                    //            1, 2, 0, 3);

                    //var KQV = Ggml.MulMat(ctxL, V_trans, KQ_soft_max);

                    var V =
                        Ggml.Cpy(ctxL,
                                Ggml.Permute(ctxL,
                                    Ggml.Reshape3D(ctxL,
                                        Vcur,
                                        n_state / n_head, n_head, n_ctx),
                                    0, 2, 1, 3),
                                Ggml.NewTensor3D(ctxL, wctx.wtype, n_state / n_head, n_ctx, n_head)
                                );

                    var KQV = Ggml.MulMat(ctxL, Ggml.Transpose(ctxL, V), KQ_soft_max);
#endif

                    var KQV_merged = Ggml.Permute(ctxL, KQV, 0, 2, 1, 3);

                    cur = Ggml.Cpy(ctxL,
                            KQV_merged,
                            Ggml.NewTensor2D(ctxL, GgmlType.F32, n_state, n_ctx));
                }

                // projection
                {
                    cur = Ggml.MulMat(ctxL,
                            layer.attn_ln_1_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.attn_ln_1_b, cur),
                            cur);
                }

                // add the input
                cur = Ggml.Add(ctxL, cur, inpL);

                var inpFF = cur;

                // feed-forward network
                {
                    // norm
                    {
                        cur = Ggml.Norm(ctxL, inpFF);

                        // cur = mlp_ln_w*cur + mlp_ln_b
                        cur = Ggml.Add(ctxL,
                                Ggml.Mul(ctxL,
                                    Ggml.Repeat(ctxL, layer.mlp_ln_w, cur),
                                    cur),
                                Ggml.Repeat(ctxL, layer.mlp_ln_b, cur));
                    }

#if USE_FLASH_FF
                    cur = Ggml.FlashFF(ctxL,
                            Ggml.Cpy(ctxL, cur, Ggml.NewTensor2D(ctxL, wctx.wtype, n_state, N)),
                            layer.mlp_0_w, layer.mlp_0_b, layer.mlp_1_w, layer.mlp_1_b);
#else
                    // fully connected
                    cur = Ggml.MulMat(ctxL,
                            layer.mlp_0_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.mlp_0_b, cur),
                            cur);

                    // GELU activation
                    cur = Ggml.GELU(ctxL, cur);

                    // projection
                    cur = Ggml.MulMat(ctxL,
                            layer.mlp_1_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.mlp_1_b, cur),
                            cur);
#endif
                }

                // output from this layer
                var inpO = Ggml.Add(ctxL, cur, inpFF);

                {
                    using var gf = new GgmlCGraph();
                    gf.NThreads = n_threads;

                    Ggml.BuildForwardExpand(gf, inpO);
                    Ggml.GraphCompute(ctxL, gf);

                    //ggml_graph_print(&gf);
                }

                // TODO: this is a hack to have per-layer computation graphs - need to come up with something better
                // input for next layer (inpO -> inpL)
                Std.MemCpy(inpL.Data, inpO.Data, Ggml.NBytes(inpL));
                inpL.Op = GgmlOp.None;
                inpL.Src0 = null;
                inpL.Src1 = null;

                //printf("%s: - used_mem(%d) = %f MB\n", __func__, il, ggml_used_mem(ctxL)/1024.0/1024.0);

                Ggml.Free(ctxL);
            }

            cur = inpL;

            // norm
            {
                cur = Ggml.Norm(ctx0, cur);

                // cur = ln_f_g*cur + ln_f_b
                cur = Ggml.Add(ctx0,
                        Ggml.Mul(ctx0,
                            Ggml.Repeat(ctx0, model.e_ln_w, cur),
                            cur),
                        Ggml.Repeat(ctx0, model.e_ln_b, cur));
            }

            // run the computation
            {
                using var gf = new GgmlCGraph();
                gf.NThreads = n_threads;

                Ggml.BuildForwardExpand(gf, cur);
                Ggml.GraphCompute(ctx0, gf);

                //ggml_graph_print(&gf);
            }

            // cur
            //{
            //    printf("ne0 = %d\n", cur.ne[0]);
            //    printf("ne1 = %d\n", cur.ne[1]);
            //    for (int i = 0; i < 10; ++i) {
            //        printf("%8.4f ", ((float *)(cur.data))[i]);
            //    }
            //    printf("... ");
            //    for (int i = cur.ne[0] - 10; i < cur.ne[0]; ++i) {
            //        printf("%8.4f ", ((float *)(cur.data))[i]);
            //    }
            //    printf("\n");
            //}

            // pre-compute cross-attention memory
            {
                using var gf = new GgmlCGraph();
                gf.NThreads = n_threads;

                // TODO: hack to disconnect the encoded features from the previous graph
                cur.Op = GgmlOp.None;
                cur.Src0 = null;
                cur.Src1 = null;

                for (var il = 0; il < model.hparams.n_text_layer; ++il)
                {
                    var layer = model.layers_decoder[il];

                    var Kcross = Ggml.MulMat(ctx0,
                            layer.cross_attn_k_w,
                            cur);

                    Kcross = Ggml.Scale(ctx0, Kcross, Ggml.NewF32(ctx0, (float)Math.Pow((n_state) / n_head, -0.25)));

                    var Vcross = Ggml.MulMat(ctx0,
                            layer.cross_attn_v_w,
                            cur);

                    Vcross = Ggml.Add(ctx0,
                            Ggml.Repeat(ctx0,
                                layer.cross_attn_v_b,
                                Vcross),
                            Vcross);

                    //var k = Ggml.View1D(ctx0, model.memory_cross_k, n_state*n_ctx, (Ggml.ElementSize(model.memory_cross_k)*n_state)*(il*hparams.n_audio_ctx + iter*n_ctx));
                    //var v = Ggml.View1D(ctx0, model.memory_cross_v, n_state*n_ctx, (Ggml.ElementSize(model.memory_cross_v)*n_state)*(il*hparams.n_audio_ctx + iter*n_ctx));
                    var k = Ggml.View1D(ctx0, model.memory_cross_k, n_state * n_ctx, (Ggml.ElementSize(model.memory_cross_k) * n_state) * (il * n_ctx));
                    var v = Ggml.View1D(ctx0, model.memory_cross_v, n_state * n_ctx, (Ggml.ElementSize(model.memory_cross_v) * n_state) * (il * n_ctx));

                    Ggml.BuildForwardExpand(gf, Ggml.Cpy(ctx0, Kcross, k));
                    Ggml.BuildForwardExpand(gf, Ggml.Cpy(ctx0, Vcross, v));
                }

                Ggml.GraphCompute(ctx0, gf);
                //ggml_graph_print(&gf);
            }

            ////////////////////////////////////////////////////////////////////////////

            //printf("%s: used_mem = %f MB\n", __func__, ggml_used_mem(ctx0)/1024.0/1024.0);

            Ggml.Free(ctx0);

            return true;
        }

        private static unsafe bool whisper_decode(whisper_context wctx,
                                                  int n_threads,
                                                  IList<whisper_token> tokens,
                                                  int n_tokens,
                                                  int n_past)
        {
            var model   = wctx.model;
            var hparams = model.hparams;

            var logits_out = wctx.logits;
            var probs_out  = wctx.probs;

            var n_vocab = hparams.n_vocab;

            var n_ctx   = hparams.n_text_ctx;
            var n_state = hparams.n_text_state;
            var n_head  = hparams.n_text_head;
            var n_layer = hparams.n_text_layer;

            var N = n_tokens;
            var M = wctx.exp_n_audio_ctx > 0 ? wctx.exp_n_audio_ctx : hparams.n_audio_ctx;

            using var @params = new GgmlInitParams();
            @params.MemSize   = wctx.buf_compute.Size;
            @params.MemBuffer = wctx.buf_compute.Data;

            var ctx0 = Ggml.Init(@params);

            var embd = Ggml.NewTensor1D(ctx0, GgmlType.I32, N);
            Marshal.Copy(tokens.ToArray(), 0, embd.Data, (int)(N * Ggml.ElementSize(embd)));

            var position = Ggml.NewTensor1D(ctx0, GgmlType.I32, N);
            var p = (int32_t*)position.Data.ToPointer();
            for (var i = 0; i < N; ++i)
            {
                p[i] = n_past + i;
            }

            // token encoding + position encoding
            var cur =
                Ggml.Add(ctx0,
                        Ggml.GetRows(ctx0, model.d_te, embd),
                        Ggml.GetRows(ctx0, model.d_pe, position));

            var inpL = cur;

            for (var il = 0; il < n_layer; ++il)
            {
                var layer = model.layers_decoder[il];

                using var paramsL = new GgmlInitParams();
                paramsL.MemSize   = wctx.buf_compute_layer.Size;
                paramsL.MemBuffer = wctx.buf_compute_layer.Data;

                var ctxL = Ggml.Init(paramsL);
                using var gf = new GgmlCGraph();
                gf.NThreads = n_threads;

                // norm
                {
                    cur = Ggml.Norm(ctxL, inpL);

                    // cur = ln_0_w*cur + ln_0_b
                    cur = Ggml.Add(ctxL,
                            Ggml.Mul(ctxL,
                                Ggml.Repeat(ctxL, layer.attn_ln_0_w, cur),
                                cur),
                            Ggml.Repeat(ctxL, layer.attn_ln_0_b, cur));
                }

                // self-attention
                {
                    var Qcur = Ggml.MulMat(ctxL,
                            layer.attn_q_w,
                            cur);

                    Qcur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL,
                                layer.attn_q_b,
                                Qcur),
                            Qcur);

                    Qcur = Ggml.Scale(ctxL, Qcur, Ggml.NewF32(ctxL, (float)Math.Pow((n_state) / n_head, -0.25)));

                    // note: no bias for Key
                    var Kcur = Ggml.MulMat(ctxL,
                            layer.attn_k_w,
                            cur);

                    Kcur = Ggml.Scale(ctxL, Kcur, Ggml.NewF32(ctxL, (float)Math.Pow((n_state) / n_head, -0.25)));

                    var Vcur = Ggml.MulMat(ctxL,
                            layer.attn_v_w,
                            cur);

                    Vcur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL,
                                layer.attn_v_b,
                                Vcur),
                            Vcur);

                    // store key and value to memory
                    {
                        var k = Ggml.View1D(ctxL, model.memory_k, N * n_state, (Ggml.ElementSize(model.memory_k) * n_state) * (il * n_ctx + n_past));
                        var v = Ggml.View1D(ctxL, model.memory_v, N * n_state, (Ggml.ElementSize(model.memory_v) * n_state) * (il * n_ctx + n_past));

                        Ggml.BuildForwardExpand(gf, Ggml.Cpy(ctxL, Kcur, k));
                        Ggml.BuildForwardExpand(gf, Ggml.Cpy(ctxL, Vcur, v));
                    }

                    // ------

                    var Q =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Qcur,
                                    Ggml.NewTensor3D(ctxL, GgmlType.F32, n_state / n_head, n_head, N)),
                                0, 2, 1, 3);

                    var K =
                        Ggml.Permute(ctxL,
                                Ggml.Reshape3D(ctxL,
                                    Ggml.View1D(ctxL, model.memory_k, (n_past + N) * n_state, il * n_ctx * Ggml.ElementSize(model.memory_k) * n_state),
                                    n_state / n_head, n_head, n_past + N),
                                0, 2, 1, 3);

                    // K * Q
                    var KQ = Ggml.MulMat(ctxL, K, Q);

                    //var KQ_scaled =
                    //    Ggml.Scale(ctxL,
                    //            KQ,
                    //            Ggml.NewF32(ctxL, 1.0f/sqrt(float(n_state)/n_head))
                    //            );

                    var KQ_masked = Ggml.DiagMaskInf(ctxL, KQ, n_past);

                    var KQ_soft_max = Ggml.SoftMax(ctxL, KQ_masked);

                    var V_trans =
                        Ggml.Permute(ctxL,
                                Ggml.Reshape3D(ctxL,
                                    Ggml.View1D(ctxL, model.memory_v, (n_past + N)*n_state, il*n_ctx*Ggml.ElementSize(model.memory_v)*n_state),
                                    n_state/n_head, n_head, n_past + N),
                                1, 2, 0, 3);

                    var KQV = Ggml.MulMat(ctxL, V_trans, KQ_soft_max);

                    var KQV_merged = Ggml.Permute(ctxL, KQV, 0, 2, 1, 3);

                    cur = Ggml.Cpy(ctxL,
                            KQV_merged,
                            Ggml.NewTensor2D(ctxL, GgmlType.F32, n_state, N));
                }

                {
                    cur = Ggml.MulMat(ctxL,
                            layer.attn_ln_1_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.attn_ln_1_b, cur),
                            cur);
                }

                // add the input
                var inpCA = Ggml.Add(ctxL, cur, inpL);

                // norm
                {
                    cur = Ggml.Norm(ctxL, inpCA); // note: we use inpCA here

                    // cur = ln_0_w*cur + ln_0_b
                    cur = Ggml.Add(ctxL,
                            Ggml.Mul(ctxL,
                                Ggml.Repeat(ctxL, layer.cross_attn_ln_0_w, cur),
                                cur),
                            Ggml.Repeat(ctxL, layer.cross_attn_ln_0_b, cur));
                }

                // cross-attention
                {
                    var Qcur = Ggml.MulMat(ctxL,
                            layer.cross_attn_q_w,
                            cur);

                    Qcur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL,
                                layer.cross_attn_q_b,
                                Qcur),
                            Qcur);

                    Qcur = Ggml.Scale(ctxL, Qcur, Ggml.NewF32(ctxL, (float)Math.Pow((n_state) / n_head, -0.25)));

                    // Kcross is already scaled
                    var Kcross =
                        Ggml.Reshape3D(ctxL,
                                Ggml.View1D(ctxL, model.memory_cross_k, M * n_state, il * M * Ggml.ElementSize(model.memory_cross_k) * n_state),
                                n_state / n_head, n_head, M);

                    var Vcross =
                        Ggml.Reshape3D(ctxL,
                                Ggml.View1D(ctxL, model.memory_cross_v, M * n_state, il * M * Ggml.ElementSize(model.memory_cross_v) * n_state),
                                n_state / n_head, n_head, M);

                    // ------

                    var Q =
                        Ggml.Permute(ctxL,
                                Ggml.Cpy(ctxL,
                                    Qcur,
                                    Ggml.NewTensor3D(ctxL, GgmlType.F32, n_state/n_head, n_head, N)),
                                0, 2, 1, 3);

                    var K = Ggml.Permute(ctxL, Kcross, 0, 2, 1, 3);

                    // K * Q
                    var KQ = Ggml.MulMat(ctxL, K, Q);

                    //var KQ_scaled =
                    //    Ggml.Scale(ctxL,
                    //            KQ,
                    //            Ggml.NewF32(ctxL, 1.0f/sqrt(float(n_state)/n_head))
                    //            );

                    // no masking for cross-attention
                    //var KQ_masked = Ggml.DiagMaskInf(ctxL, KQ_scaled, n_past);

                    var KQ_soft_max = Ggml.SoftMax(ctxL, KQ);

                    var V_trans = Ggml.Permute(ctxL, Vcross, 1, 2, 0, 3);

                    var KQV = Ggml.MulMat(ctxL, V_trans, KQ_soft_max);

                    var KQV_merged = Ggml.Permute(ctxL, KQV, 0, 2, 1, 3);

                    // cur = KQV_merged.contiguous().view(n_state, N)
                    cur = Ggml.Cpy(ctxL,
                            KQV_merged,
                            Ggml.NewTensor2D(ctxL, GgmlType.F32, n_state, N));
                }

                // projection
                {
                    cur = Ggml.MulMat(ctxL,
                            layer.cross_attn_ln_1_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.cross_attn_ln_1_b, cur),
                            cur);
                }

                // add the input
                cur = Ggml.Add(ctxL, cur, inpCA);

                var inpFF = cur;

                // feed-forward network
                {
                    // norm
                    {
                        cur = Ggml.Norm(ctxL, inpFF);

                        // cur = mlp_ln_w*cur + mlp_ln_b
                        cur = Ggml.Add(ctxL,
                                Ggml.Mul(ctxL,
                                    Ggml.Repeat(ctxL, layer.mlp_ln_w, cur),
                                    cur),
                                Ggml.Repeat(ctxL, layer.mlp_ln_b, cur));
                    }

                    // fully connected
                    cur = Ggml.MulMat(ctxL,
                            layer.mlp_0_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.mlp_0_b, cur),
                            cur);

                    // GELU activation
                    cur = Ggml.GELU(ctxL, cur);

                    // projection
                    cur = Ggml.MulMat(ctxL,
                            layer.mlp_1_w,
                            cur);

                    cur = Ggml.Add(ctxL,
                            Ggml.Repeat(ctxL, layer.mlp_1_b, cur),
                            cur);
                }

                // output from this layer
                var inpO = Ggml.Add(ctxL, cur, inpFF);

                {
                    Ggml.BuildForwardExpand(gf, inpO);
                    Ggml.GraphCompute(ctxL, gf);

                    //ggml_graph_print(&gf);
                }

                // TODO: this is a hack to have per-layer computation graphs - need to come up with something better
                // input for next layer (inpO -> inpL)
                Std.MemCpy(inpL.Data, inpO.Data, Ggml.NBytes(inpL));
                inpL.Op = GgmlOp.None;
                inpL.Src0 = null;
                inpL.Src1 = null;

                if (N > 1)
                {
                    //printf("%s: - used_mem(%d) = %f MB\n", __func__, il, ggml_used_mem(ctxL)/1024.0/1024.0);
                }

                Ggml.Free(ctxL);
            }

            cur = inpL;

            // norm
            {
                cur = Ggml.Norm(ctx0, cur);

                cur = Ggml.Add(ctx0,
                        Ggml.Mul(ctx0,
                            Ggml.Repeat(ctx0, model.d_ln_w, cur),
                            cur),
                        Ggml.Repeat(ctx0, model.d_ln_b, cur));
            }

            var logits = Ggml.MulMat(ctx0, model.d_te, cur);

            // logits -> probs
            cur = Ggml.Dup(ctx0, logits);
            cur = Ggml.SoftMax(ctx0, cur); // in-place

            // run the computation
            {
                using var gf2 = new GgmlCGraph();
                gf2.NThreads = n_threads;

                Ggml.BuildForwardExpand(gf2, cur);
                Ggml.GraphCompute(ctx0, gf2);
            }

            logits_out.Resize(N * n_vocab);
            Std.MemCpy(logits_out.Data, Ggml.GetData(logits), sizeof(float) * N * n_vocab);

            probs_out.Resize(N * n_vocab);
            Std.MemCpy(probs_out.Data, Ggml.GetData(cur), sizeof(float) * N * n_vocab);

            if (N > 1)
            {
                //const float mem_per_token = ggml_used_mem(ctx0)/1024.0/1024.0/N;
                //printf("%s: used_mem = %f MB / %f per token\n", __func__, ggml_used_mem(ctx0)/1024.0/1024.0, mem_per_token);
                //printf("%s: max mem = %f MB\n", __func__, mem_per_token*model.hparams.n_text_ctx);
            }

            Ggml.Free(ctx0);

            return true;
        }

        // the most basic sampling scheme - select the top token
        private static unsafe whisper_token_data whisper_sample_best(whisper_vocab vocab,
                                                                     float * probs,
                                                                     bool force_timestamp,
                                                                     bool is_initial)
        {
            var result = new whisper_token_data
            {                
                id = 0,
                tid = 0,
                p = 0.0f,
                pt = 0.0f,
                ptsum = 0.0f,
                t0 = -1,
                t1 = -1,
                vlen = 0.0f
            };

            var n_logits = vocab.n_vocab;

            var probs_id = vocab.probs_id;

            probs_id.Clear();
            for (int i = 0; i < n_logits; i++)
            {
                probs_id.Add(new Pair<double, int32_t>(probs[i], i));
            }

            {
                double sum_ts =  0.0;
                double max_ts = -1.0;
                double max_tx = -1.0;

                for (var i = 0; i < vocab.token_beg; i++)
                {
                    max_tx = Math.Max(max_tx, probs_id[i].Item1);
                }

                var i0 = is_initial ? vocab.token_beg + 101 : vocab.token_beg;
                var i1 = is_initial ? vocab.token_beg + 101 : n_logits;

                // the initial timestamp cannot be larger than 100
                // ref: https://github.com/openai/whisper/blob/0b1ba3d46ebf7fe6f953acfd8cad62a4f851b49f/whisper/decoding.py#L426-L429
                if (is_initial)
                {
                    for (var i = i0; i < n_logits; ++ i)
                    {
                        probs_id[i].Item1 = float.NegativeInfinity;
                    }
                }

                for (var i = vocab.token_beg; i < i1; i++)
                {
                    sum_ts += probs_id[i].Item1;
                    if  (probs_id[i].Item1 > max_ts)
                    {
                        max_ts = probs_id[i].Item1;
                        result.tid = probs_id[i].Item2;
                    }
                }

                // if the probability sum of all timestamp tokens is higher than the max probability of the text tokens - sample a
                // timestamp token
                if (sum_ts > max_tx || force_timestamp)
                {
                    // ref: https://github.com/openai/whisper/blob/0b1ba3d46ebf7fe6f953acfd8cad62a4f851b49f/whisper/decoding.py#L430-L438
                    for (var i = 0; i < vocab.token_beg; i++)
                    {
                        probs_id[i].Item1 = float.NegativeInfinity;
                    }
                }

                result.pt = (float)(max_ts / (sum_ts + 1e-10));
                result.ptsum = (float)sum_ts;
            }

            // find the top K tokens
            const int top_k = 4;

            // std::partial_sort(
            //         probs_id.begin(),
            //         probs_id.begin() + top_k, probs_id.end(),
            //         [](const std::pair<double, whisper_vocab::id> & a, const std::pair<double, whisper_vocab::id> & b) {
            //     return a.first > b.first;
            // });

            // probs_id.resize(top_k);
            probs_id.Sort((a, b) => b.Item1.CompareTo(a.Item1));
            probs_id.RemoveRange(top_k, probs_id.Count - top_k);

            //printf("\n");
            //for (int i = 0; i < (int) probs_id.size(); i++) {
            //    printf("%d: '%s' %f, %d\n", i, vocab.id_to_token.at(probs_id[i].second).c_str(), probs_id[i].first, probs_id[i].second);
            //}

            int res = 0;
            while ((probs_id[res].Item2 == vocab.token_sot ||
                    probs_id[res].Item2 == vocab.token_solm ||
                    probs_id[res].Item2 == vocab.token_not) &&
                    res < (int)probs_id.Count - 1) {
                res++;
            }

            result.id = probs_id[res].Item2;
            result.p  = (float)probs_id[res].Item1;

            return result;
        }

        private static unsafe whisper_token_data whisper_sample_best(whisper_context ctx)
        {
            var t_start_sample_us = Ggml.TimeMicrosecond();

            var res = whisper_sample_best(ctx.vocab, (float*)ctx.probs.Data.ToPointer() + (ctx.probs.Size - ctx.vocab.n_vocab), false, false);

            ctx.t_sample_us += Ggml.TimeMicrosecond() - t_start_sample_us;

            return res;
        }

        private static unsafe whisper_token_data whisper_sample_timestamp(whisper_context ctx, bool is_initial)
        {
            var t_start_sample_us = Ggml.TimeMicrosecond();

            var res = whisper_sample_best(ctx.vocab, (float*)ctx.probs.Data.ToPointer() + (ctx.probs.Size - ctx.vocab.n_vocab), true, is_initial);

            ctx.t_sample_us += Ggml.TimeMicrosecond() - t_start_sample_us;

            return res;
        }

        private static int whisper_decode(whisper_context ctx,
                                          IList<whisper_token> tokens,
                                          int n_tokens,
                                          int n_past,
                                          int n_threads)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;
            
            var t_start_us = Ggml.TimeMicrosecond();

            if (!whisper_decode(ctx, n_threads, tokens, n_tokens, n_past))
            {
                Console.WriteLine($"{0}: failed to eval\n", __func__);
                return 1;
            }

            ctx.t_decode_us += Ggml.TimeMicrosecond() - t_start_us;

            return 0;
        }

        private static int whisper_encode(whisper_context ctx, int offset, int n_threads)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;
            
            var t_start_us = Ggml.TimeMicrosecond();

            if (!whisper_encode2(ctx, n_threads, offset))
            {
                Console.WriteLine($"{0}: failed to eval", __func__);
                return -1;
            }

            ctx.t_encode_us += Ggml.TimeMicrosecond() - t_start_us;

            return 0;
        }

        private static void read_safe(BinaryReader binaryReader, out int dest)
        {
            dest = binaryReader.ReadInt32();
        }

        private static void read_safe(BinaryReader binaryReader, out uint dest)
        {
            dest = binaryReader.ReadUInt32();
        }

        private static string to_timestamp(int64_t t, bool comma = false)
        {
            var msec = t * 10;
            var hr = msec / (1000 * 60 * 60);
            msec = msec - hr * (1000 * 60 * 60);
            var min = msec / (1000 * 60);
            msec = msec - min * (1000 * 60);
            var sec = msec / 1000;
            msec = msec - sec * 1000;

            return string.Format("{0:D2}:{1:D2}:{2:D2}{3}{4:3D}", (int) hr, (int) min, (int) sec, comma ? "," : ".", (int) msec);
        }

        private static int timestamp_to_sample(int64_t t, int n_samples)
        {
            return Math.Max(0, Math.Min((int)n_samples - 1, (int)((t * WHISPER_SAMPLE_RATE) / 100)));
        }

        private static int64_t sample_to_timestamp(int i_sample)
        {
            return (100 * i_sample) / WHISPER_SAMPLE_RATE;
        }

        private static void dft(Buffer<float> inBuffer, Buffer<float> outBuffer)
        {
            var N = inBuffer.Size;

            outBuffer.Resize(N * 2);

            for (int k = 0; k < N; k++)
            {
                double re = 0;
                double im = 0;

                for (int n = 0; n < N; n++)
                {
                    var angle = 2 * Math.PI * k * n / N;
                    re += inBuffer[n] * Math.Cos(angle);
                    im -= inBuffer[n] * Math.Sin(angle);
                }

                outBuffer[k * 2 + 0] = (float)re;
                outBuffer[k * 2 + 1] = (float)im;
            }
        }

        // Cooley-Tukey FFT
        // poor man's implementation - use something better
        // input is real-valued
        // output is complex-valued
        private unsafe static void fft(Buffer<float> @in, Buffer<float> @out)
        {
            @out.Resize(@in.Size * 2);

            var N = @in.Size;

            if (N == 1)
            {
                @out[0] = @in[0];
                @out[1] = 0;
                return;
            }

            if (N % 2 == 1)
            {
                dft(@in, @out);
                return;
            }

            var even = new Buffer<float>();
            var odd = new Buffer<float>();

            even.Reserve(N / 2);
            odd.Reserve(N / 2);

            for (int i = 0; i < N; i++)
            {
                if (i % 2 == 0)
                {
                    even[i / 2] = @in[i];
                }
                else
                {
                    odd[i / 2] = @in[i];
                }
            }

            var even_fft = new Buffer<float>();
            var odd_fft = new Buffer<float>();

            fft(even, even_fft);
            fft(odd, odd_fft);

            for (var k = 0; k < N / 2; k++)
            {
                var theta = 2 * Math.PI * k / N;

                var re = (float)Math.Cos(theta);
                var im = -(float)Math.Sin(theta);

                var re_odd = odd_fft[2 * k + 0];
                var im_odd = odd_fft[2 * k + 1];

                @out[2 * k + 0] = even_fft[2 * k + 0] + re * re_odd - im * im_odd;
                @out[2 * k + 1] = even_fft[2 * k + 1] + re * im_odd + im * re_odd;

                @out[2 * (k + N / 2) + 0] = even_fft[2 * k + 0] - re * re_odd + im * im_odd;
                @out[2 * (k + N / 2) + 1] = even_fft[2 * k + 1] - re * im_odd - im * re_odd;
            }

            even_fft.Free();
            odd_fft.Free();
        }

        // ref: https: // github.com / openai / whisper / blob / main / whisper / audio.py#L92-L124
        private static unsafe bool log_mel_spectrogram(float* samples,
                                                       int n_samples,
                                                       int sample_rate,
                                                       int fft_size,
                                                       int fft_step,
                                                       int n_mel,
                                                       int n_threads,
                                                       whisper_filters filters,
                                                       bool speed_up,
                                                       whisper_mel mel)
        {
            // Hanning window
            var hann = new Buffer<float>();
            hann.Resize(fft_size);
            for (int i = 0; i < fft_size; i++)
            {
                hann[i] = (float)(0.5 * (1.0 - Math.Cos((2.0 * Math.PI * i) / (fft_size))));
            }

            mel.n_mel = n_mel;
            mel.n_len = (n_samples) / fft_step;
            mel.data.Resize(mel.n_mel * mel.n_len);

            var n_fft = 1 + (speed_up ? fft_size / 4 : fft_size / 2);

            // printf("%s: n_samples = %d, n_len = %d\n", __func__, n_samples, mel.n_len);
            // printf("%s: recording length: %f s\n", __func__, (float) n_samples / sample_rate);

            var workers = new Thread[n_threads];
            for (int iw = 0; iw < n_threads; ++iw)
            {
                var start = new ParameterizedThreadStart(obj => 
                {
                    var ith = (int)obj;

                    var fft_in = new Buffer<float>();
                    fft_in.Resize(fft_size);
                    for (var i = 0; i < fft_size; i++)
                    {
                        fft_in[i] = 0.0f;
                    }

                    var fft_out = new Buffer<float>();
                    fft_out.Resize(2 * fft_size);

                    for (var i = ith; i < mel.n_len; i += n_threads)
                    {
                        var offset = i * fft_step;

                        // apply Hanning window
                        for (var j = 0; j < fft_size; j++)
                        {
                            if (offset + j < n_samples)
                            {
                                fft_in[j] = hann[j] * samples[offset + j];
                            }
                            else
                            {
                                fft_in[j] = 0.0f;
                            }
                        }

                        // FFT -> mag^2
                        fft(fft_in, fft_out);

                        for (var j = 0; j < fft_size; j++)
                        {
                            fft_out[j] = (fft_out[2 * j + 0] * fft_out[2 * j + 0] + fft_out[2 * j + 1] * fft_out[2 * j + 1]);
                        }
                        for (var j = 1; j < fft_size / 2; j++)
                        {
                            // if (i == 0) {
                            //    printf("%d: %f %f\n", j, fft_out[j], fft_out[fft_size - j]);
                            // }
                            fft_out[j] += fft_out[fft_size - j];
                        }
                        if (i == 0)
                        {
                            // for (int j = 0; j < fft_size; j++) {
                            //    printf("%d: %e\n", j, fft_out[j]);
                            // }
                        }

                        if (speed_up)
                        {
                            // scale down @in the frequency domain results @in a speed up @in the time domain
                            for (int j = 0; j < n_fft; j++)
                            {
                                fft_out[j] = (float)(0.5 * (fft_out[2 * j] + fft_out[2 * j + 1]));
                            }
                        }

                        // mel spectrogram
                        for (var j = 0; j < mel.n_mel; j++)
                        {
                            double sum = 0.0;

                            for (var k = 0; k < n_fft; k++)
                            {
                                sum += fft_out[k] * filters.data[j * n_fft + k];
                            }
                            if (sum < 1e-10)
                            {
                                sum = 1e-10;
                            }

                            sum = Math.Log10(sum);

                            mel.data[j * mel.n_len + i] = (float)sum;
                        }
                    }

                    fft_in.Free();
                    fft_out.Free();
                });
                workers[iw] = new Thread(start);
                workers[iw].Start(iw);
            }

            for (var iw = 0; iw < n_threads; ++iw)
            {
                workers[iw].Join();
            }

            // clamping and normalization
            double mmax = -1e20;
            for (var i = 0; i < mel.n_mel * mel.n_len; i++)
            {
                if (mel.data[i] > mmax)
                {
                    mmax = mel.data[i];
                }
            }
            // printf("%s: max = %f\n", __func__, mmax);

            mmax -= 8.0;

            for (var i = 0; i < mel.n_mel * mel.n_len; i++)
            {
                if (mel.data[i] < mmax)
                {
                    mel.data[i] = (float)mmax;
                }

                mel.data[i] = (float)((mel.data[i] + 4.0) / 4.0);
            }

            hann.Free();

            return true;
        }

        private static int whisper_n_len(whisper_context ctx)
        {
            return ctx.mel.n_len;
        }

        private static int whisper_n_vocab(whisper_context ctx)
        {
            return ctx.vocab.n_vocab;
        }

        private static int whisper_n_text_ctx(whisper_context ctx)
        {
            return ctx.model.hparams.n_text_ctx;
        }

        private static int whisper_n_audio_ctx(whisper_context ctx)
        {
            return ctx.model.hparams.n_audio_ctx;
        }

        private static int whisper_is_multilingual(whisper_context ctx)
        {
            return ctx.vocab.is_multilingual() ? 1 : 0;
        }

        private static unsafe float* whisper_get_probs(whisper_context ctx)
        {
            return (float*)ctx.probs.Data.ToPointer();
        }

        private static string whisper_token_to_str(whisper_context ctx, whisper_token token)
        {
            return ctx.vocab.id_to_token[token];
        }

        private static whisper_token whisper_token_eot(whisper_context ctx)
        {
            return ctx.vocab.token_eot;
        }

        private static whisper_token whisper_token_sot(whisper_context ctx)
        {
            return ctx.vocab.token_sot;
        }

        private static whisper_token whisper_token_prev(whisper_context ctx)
        {
            return ctx.vocab.token_prev;
        }

        private static whisper_token whisper_token_solm(whisper_context ctx)
        {
            return ctx.vocab.token_solm;
        }

        private static whisper_token whisper_token_not(whisper_context ctx)
        {
            return ctx.vocab.token_not;
        }

        private static whisper_token whisper_token_beg(whisper_context ctx)
        {
            return ctx.vocab.token_beg;
        }

        private static whisper_token whisper_token_lang(whisper_context ctx, int lang_id)
        {
            return whisper_token_sot(ctx) + 1 + lang_id;
        }

        private static whisper_token whisper_token_translate()
        {
            return whisper_vocab.token_translate;
        }

        private static whisper_token whisper_token_transcribe()
        {
            return whisper_vocab.token_transcribe;
        }

        private static IList<id> tokenize(whisper_vocab vocab, string text)
        {
            // ToDo: not support for now
            // var words = new List<string>();

            // // first split the text into words
            // {
            //     string str = text;
            //     string pat = R"('s|'t|'re|'ve|'m|'ll|'d| ?[[:alpha:]]+| ?[[:digit:]]+| ?[^\s[:alpha:][:digit:]]+|\s+(?!\S)|\s+)";

            //     std::regex re(pat);
            //     std::smatch m;

            //     while (std::regex_search(str, m, re))
            //     {
            //         foreach (var x in m)
            //         {
            //             words.Add(x);
            //         }

            //         str = m.suffix();
            //     }
            // }

            // find the longest tokens that form the words:
            var tokens = new List<id>();
            // ToDo: not support for now
            // foreach (var word in words)
            // {
            //     if (string.IsNullOrEmpty(word))
            //         continue;

            //     int i = 0;
            //     int n = word.size();
            //     while (i < n)
            //     {
            //         int j = n;
            //         while (j > i)
            //         {
            //             auto it = vocab.token_to_id.find(word.substr(i, j-i));
            //             if (it != vocab.token_to_id.end()) {
            //                 tokens.push_back(it->second);
            //                 i = j;
            //                 break;
            //             }
            //             --j;
            //         }
            //         if (i == n)
            //         {
            //             break;
            //         }

            //         if (j == i)
            //         {
            //             auto sub = word.substr(i, 1);
            //             if (vocab.token_to_id.find(sub) != vocab.token_to_id.end())
            //             {
            //                 tokens.push_back(vocab.token_to_id.at(sub));
            //             }
            //             else
            //             {
            //                 fprintf(stderr, "%s: unknown token '%s'\n", __func__, sub.data());
            //             }

            //             ++i;
            //         }
            //     }
            // }

            return tokens;
        }

        // a cost-function / heuristic that is high for text that takes longer to pronounce
        // obviously, can be improved
        private static float voice_length(string text)
        {
            float res = 0.0f;

            foreach (var c in text)
            {
                if (c == ' ')
                {
                    res += 0.01f;
                }
                else if (c == ',')
                {
                    res += 2.00f;
                }
                else if (c == '.')
                {
                    res += 3.00f;
                }
                else if (c == '!')
                {
                    res += 3.00f;
                }
                else if (c == '?')
                {
                    res += 3.00f;
                }
                else if (c >= '0' && c <= '9')
                {
                    res += 3.00f;
                }
                else
                {
                    res += 1.00f;
                }
            }

            return res;
        }

        // average the fabs of the signal
        private static unsafe float[] get_signal_energy(float* signal, int n_samples, int n_samples_per_half_window)
        {
            var hw = n_samples_per_half_window;

            var result = new float[n_samples];

            for (var i = 0; i < n_samples; i++)
            {
                float sum = 0;
                for (var j = -hw; j <= hw; j++)
                {
                    if (i + j >= 0 && i + j < n_samples)
                    {
                        sum += Math.Abs(signal[i + j]);
                    }
                }

                result[i] = sum / (2 * hw + 1);
            }

            return result;
        }


        private static void whisper_exp_compute_token_level_timestamps(whisper_context ctx,
                                                                       int   i_segment,
                                                                       float thold_pt,
                                                                       float thold_ptsum)
        {
            var __func__ = System.Reflection.MethodBase.GetCurrentMethod().Name;

            var segment = ctx.result_all[i_segment];
            var tokens  = segment.tokens;

            var n_samples = ctx.energy.Length;

            if (n_samples == 0)
            {
                Console.WriteLine("{0}: no signal data available", __func__);
                return;
            }

            var t0 = segment.t0;
            var t1 = segment.t1;

            var n = tokens.Count;

            if (n == 0)
            {
                return;
            }

            if (n == 1)
            {
                tokens[0].t0 = t0;
                tokens[0].t1 = t1;

                return;
            }

            var t_beg    = ctx.t_beg;
            var t_last   = ctx.t_last;
            var tid_last = ctx.tid_last;

            for (var j = 0; j < n; ++j)
            {
                var token = tokens[j];

                if (j == 0)
                {
                    if (token.id == whisper_token_beg(ctx))
                    {
                        tokens[j    ].t0 = t0;
                        tokens[j    ].t1 = t0;
                        tokens[j + 1].t0 = t0;

                        t_beg    = t0;
                        t_last   = t0;
                        tid_last = whisper_token_beg(ctx);
                    }
                    else
                    {
                        tokens[j    ].t0 = t_last;
                    }
                }

                var tt = t_beg + 2*(token.tid - whisper_token_beg(ctx));

                tokens[j].id    = token.id;
                tokens[j].tid   = token.tid;
                tokens[j].p     = token.p;
                tokens[j].pt    = token.pt;
                tokens[j].ptsum = token.ptsum;

                tokens[j].vlen = voice_length(whisper_token_to_str(ctx, token.id));

                if (token.pt > thold_pt && token.ptsum > thold_ptsum && token.tid > tid_last && tt <= t1)
                {
                    if (j > 0)
                    {
                        tokens[j - 1].t1 = tt;
                    }
                    tokens[j].t0 = tt;
                    tid_last = token.tid;
                }
            }

            tokens[n - 2].t1 = t1;
            tokens[n - 1].t0 = t1;
            tokens[n - 1].t1 = t1;

            t_last = t1;

            // find intervals of tokens with unknown timestamps
            // fill the timestamps by proportionally splitting the interval based on the token voice lengths
            {
                int p0 = 0;
                int p1 = 0;

                while (true)
                {
                    while (p1 < n && tokens[p1].t1 < 0)
                    {
                        p1++;
                    }

                    if (p1 >= n)
                    {
                        p1--;
                    }

                    if (p1 > p0)
                    {
                        double psum = 0.0;
                        for (var j = p0; j <= p1; j++)
                        {
                            psum += tokens[j].vlen;
                        }

                        //printf("analyzing %d - %d, psum = %f\n", p0, p1, psum);

                        var dt = tokens[p1].t1 - tokens[p0].t0;

                        // split the time proportionally to the voice length
                        for (int j = p0 + 1; j <= p1; j++)
                        {
                            double ct = tokens[j - 1].t0 + dt * tokens[j - 1].vlen / psum;

                            tokens[j - 1].t1 = (int64_t)ct;
                            tokens[j    ].t0 = (int64_t)ct;
                        }
                    }

                    p1++;
                    p0 = p1;
                    if (p1 >= n)
                    {
                        break;
                    }
                }
            }

            // fix up (just in case)
            for (var j = 0; j < n - 1; j++)
            {
                if (tokens[j].t1 < 0)
                {
                    tokens[j + 1].t0 = tokens[j].t1;
                }

                if (j > 0)
                {
                    if (tokens[j - 1].t1 > tokens[j].t0)
                    {
                        tokens[j].t0 = tokens[j - 1].t1;
                        tokens[j].t1 = Math.Max(tokens[j].t0, tokens[j].t1);
                    }
                }
            }

            // VAD
            // expand or contract tokens based on voice activity
            {
                var hw = WHISPER_SAMPLE_RATE/8;

                for (int j = 0; j < n; j++)
                {
                    if (tokens[j].id >= whisper_token_eot(ctx))
                    {
                        continue;
                    }

                    int s0 = timestamp_to_sample(tokens[j].t0, n_samples);
                    int s1 = timestamp_to_sample(tokens[j].t1, n_samples);

                    var ss0 = Math.Max(s0 - hw, 0);
                    var ss1 = Math.Min(s1 + hw, n_samples);

                    var ns = ss1 - ss0;

                    float sum = 0.0f;

                    for (int k = ss0; k < ss1; k++)
                    {
                        sum += ctx.energy[k];
                    }

                    var thold = 0.5*sum/ns;

                    {
                        int k = s0;
                        if (ctx.energy[k] > thold && j > 0)
                        {
                            while (k > 0 && ctx.energy[k] > thold)
                            {
                                k--;
                            }
                            tokens[j].t0 = sample_to_timestamp(k);
                            if (tokens[j].t0 < tokens[j - 1].t1)
                            {
                                tokens[j].t0 = tokens[j - 1].t1;
                            }
                            else
                            {
                                s0 = k;
                            }
                        }
                        else
                        {
                            while (ctx.energy[k] < thold && k < s1)
                            {
                                k++;
                            }
                            s0 = k;
                            tokens[j].t0 = sample_to_timestamp(k);
                        }
                    }

                    {
                        int k = s1;
                        if (ctx.energy[k] > thold)
                        {
                            while (k < n_samples - 1 && ctx.energy[k] > thold)
                            {
                                k++;
                            }
                            tokens[j].t1 = sample_to_timestamp(k);
                            if (j < ns - 1 && tokens[j].t1 > tokens[j + 1].t0)
                            {
                                tokens[j].t1 = tokens[j + 1].t0;
                            }
                            else
                            {
                                s1 = k;
                            }
                        }
                        else
                        {
                            while (ctx.energy[k] < thold && k > s0)
                            {
                                k--;
                            }
                            s1 = k;
                            tokens[j].t1 = sample_to_timestamp(k);
                        }
                    }
                }
            }

            // fixed token expand (optional)
            //{
            //    const int t_expand = 0;

            //    for (int j = 0; j < n; j++) {
            //        if (j > 0) {
            //            tokens[j].t0 = Math.Max(0, (int) (tokens[j].t0 - t_expand));
            //        }
            //        if (j < n - 1) {
            //            tokens[j].t1 = tokens[j].t1 + t_expand;
            //        }
            //    }
            //}

            // debug info
            //for (int j = 0; j < n; ++j) {
            //    const var token = tokens[j];
            //    const auto tt = token.pt > thold_pt && token.ptsum > 0.01 ? whisper_token_to_str(ctx, token.tid) : "[?]";
            //    printf("%s: %10s %6.3f %6.3f %6.3f %6.3f %5d %5d '%s'\n", __func__,
            //            tt, token.p, token.pt, token.ptsum, token.vlen, (int) token.t0, (int) token.t1, whisper_token_to_str(ctx, token.id));

            //    if (tokens[j].id >= whisper_token_eot(ctx)) {
            //        continue;
            //    }
            //}
        }

        #endregion

    }

}