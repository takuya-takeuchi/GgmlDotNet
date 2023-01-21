using System;
using System.Collections.Generic;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class whisper_hparams
    {

        #region Constructors

        public whisper_hparams()
        {
            this.n_vocab = 51864;
            this.n_audio_ctx  = 1500;
            this.n_audio_state = 384;
            this.n_audio_head = 6;
            this.n_audio_layer = 4;
            this.n_text_ctx  = 448;
            this.n_text_state = 384;
            this.n_text_head = 6;
            this.n_text_layer = 4;
            this.n_mels = 80;
            this.f16 = 1;
        }

        #endregion

        #region Properties

        public int32_t n_vocab;

        public int32_t n_audio_ctx;

        public int32_t n_audio_state;

        public int32_t n_audio_head;

        public int32_t n_audio_layer;

        public int32_t n_text_ctx;

        public int32_t n_text_state;

        public int32_t n_text_head;

        public int32_t n_text_layer;

        public int32_t n_mels;

        public int32_t f16;

        #endregion

    }

}