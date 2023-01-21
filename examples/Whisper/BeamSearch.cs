using System;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class beam_search
    {

        #region Properties

        public int32_t n_past { get; set; }

        public int32_t beam_width { get; set; }

        public int32_t n_best { get; set; }

        #endregion

    }

}