using System;
using System.Collections.Generic;

using GgmlDotNet;

using uint8_t = System.Byte;
using int32_t = System.Int32;
using int64_t = System.Int64;
using whisper_token = System.Int32;

namespace Whisper
{

    internal sealed class whisper_context 
    {

        #region Constructors

        public whisper_context()
        {
            this.t_load_us = 0;
            this.t_mel_us = 0;
            this.t_sample_us = 0;
            this.t_encode_us = 0;
            this.t_decode_us = 0;
            this.t_start_us = 0;

            this.buf_model = new Buffer<uint8_t>();
            this.buf_memory = new Buffer<uint8_t>();
            this.buf_compute = new Buffer<uint8_t>();
            this.buf_compute_layer = new Buffer<uint8_t>();

            this.wtype = GgmlType.F16; // weight type (FP32 or FP16)

            this.model = new whisper_model();
            this.vocab = new whisper_vocab();

            this.mel = new whisper_mel();

            this.probs = new Buffer<float>();
            this.logits = new Buffer<float>();

            this.result_all = new List<whisper_segment>();
            this.prompt_past = new List<whisper_token>();

            this.t_beg = 0;
            this.t_last = 0;
            this.tid_last = 0;
            this.energy = null;
            
            this.exp_n_audio_ctx = 0;
        }

        #endregion

        #region Properties

        public int64_t t_load_us { get; set; }

        public int64_t t_mel_us { get; set; }

        public int64_t t_sample_us { get; set; }

        public int64_t t_encode_us { get; set; }

        public int64_t t_decode_us { get; set; }

        public int64_t t_start_us { get; set; }

        public Buffer<uint8_t> buf_model { get; }

        public Buffer<uint8_t> buf_memory { get; }

        public Buffer<uint8_t> buf_compute { get; }

        public Buffer<uint8_t> buf_compute_layer { get; }

        public GgmlType wtype { get; set; }

        public whisper_model model { get; }

        public whisper_vocab vocab { get; }

        public whisper_mel mel { get; }

        public Buffer<float> probs { get; set; }

        public Buffer<float> logits { get; set; }

        public IList<whisper_segment> result_all { get; }

        public IList<whisper_token> prompt_past { get; }

        public int64_t t_beg { get; set; }

        public int64_t t_last { get; set; }

        public whisper_token tid_last { get; set; }

        public float[] energy { get; set; }

        public int32_t exp_n_audio_ctx { get; set; }

        #endregion

    }

}