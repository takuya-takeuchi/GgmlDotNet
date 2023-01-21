using System;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class whisper_mel
    {

        #region Constructors

        public whisper_mel()
        {
            this.data = new Buffer<float>();
        }

        #endregion

        #region Properties

        public int32_t n_len { get; set; }

        public int32_t n_mel { get; set; }

        public Buffer<float> data { get; }

        #endregion

    }

}