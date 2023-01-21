using System;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class whisper_filters
    {

        #region Constructors

        public whisper_filters()
        {
            this.data = new Buffer<float>();
        }

        #endregion

        #region Properties

        public int32_t n_mel;

        public int32_t n_fft;

        public Buffer<float> data { get; }

        #endregion

    }

}