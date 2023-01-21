using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using whisper_token = System.Int32;

namespace Whisper
{

    internal sealed class whisper_full_params
    {

        #region Constructors

        public whisper_full_params()
        {
            this.greedy = new greedy();
            this.beam_search = new beam_search();
        }

        #endregion

        #region Properties

        public whisper_sampling_strategy strategy { get; set; }

        public int n_threads { get; set; }

        public int n_max_text_ctx { get; set; }

        public int offset_ms { get; set; }

        public int duration_ms { get; set; }

        public bool translate { get; set; }

        public bool no_context { get; set; }

        public bool single_segment { get; set; }

        public bool print_special { get; set; }

        public bool print_progress { get; set; }

        public bool print_realtime { get; set; }

        public bool print_timestamps { get; set; }

        public bool  token_timestamps { get; set; }

        public float thold_pt { get; set; }

        public float thold_ptsum { get; set; }

        public int   max_len { get; set; }

        public int   max_tokens { get; set; }

        public bool speed_up { get; set; }

        public int  audio_ctx { get; set; }

        public List<whisper_token> prompt_tokens { get; set; }

        public int prompt_n_tokens { get; set; }

        // for auto-detection, set to nullptr, "" or "auto"
        public string language { get; set; }

        public greedy greedy { get; }

        public beam_search beam_search { get; }

        public whisper_new_segment_callback new_segment_callback { get; set; }

        public object new_segment_callback_user_data { get; set; }

        public whisper_encoder_begin_callback encoder_begin_callback { get; set; }

        public object encoder_begin_callback_user_data { get; set; }

        #endregion

    }

}