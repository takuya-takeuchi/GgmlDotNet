using System;
using System.Collections.Generic;

using int64_t = System.Int64;

namespace Whisper
{

    internal sealed class whisper_segment
    {

        #region Constructors

        public whisper_segment()
        {
            this.tokens = new List<whisper_token_data>();
        }

        #endregion

        #region Properties

        public int64_t t0 { get; set; }

        public int64_t t1 { get; set; }

        public string text { get; set; }

        public List<whisper_token_data> tokens { get; }

        #endregion

    }

}