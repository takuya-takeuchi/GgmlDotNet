using System;
using System.Collections.Generic;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class WhisperParams
    {

        #region Constructors

        public WhisperParams()
        {
            this.n_threads = Math.Min(4, System.Environment.ProcessorCount);
            this.n_processors = 1;
            this.offset_t_ms = 0;
            this.offset_n = 0;
            this.duration_ms = 0;
            this.max_context = -1;
            this.max_len = 0;

            this.word_thold = 0.01f;

            this.speed_up = false;
            this.translate = false;
            this.diarize = false;
            this.output_txt = false;
            this.output_vtt = false;
            this.output_srt = false;
            this.output_wts = false;
            this.output_csv = false;
            this.print_special = false;
            this.print_colors = false;
            this.print_progress = false;
            this.no_timestamps = false;

            this.prompt = "";
            this.language = "en";
            this.model = "models/ggml-base.en.bin";

            this.fname_inp = new List<string>();
        }

        #endregion

        #region Properties

        public int32_t n_threads { get; set; }

        public int32_t n_processors { get; set; }

        public int32_t offset_t_ms { get; set; }

        public int32_t offset_n { get; set; }

        public int32_t duration_ms { get; set; }

        public int32_t max_context { get; set; }

        public int32_t max_len { get; set; }

        public float word_thold { get; set; }

        public bool speed_up { get; set; }

        public bool translate { get; set; }

        public bool diarize { get; set; }

        public bool output_txt { get; set; }

        public bool output_vtt { get; set; }

        public bool output_srt { get; set; }

        public bool output_wts { get; set; }

        public bool output_csv { get; set; }

        public bool print_special { get; set; }

        public bool print_colors { get; set; }

        public bool print_progress { get; set; }

        public bool no_timestamps { get; set; }

        public string language { get; set; }

        public string prompt { get; set; }

        public string model { get; set; }

        public IList<string> fname_inp { get; }

        #endregion

    }

}