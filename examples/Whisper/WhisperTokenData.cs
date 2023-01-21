using System;

using whisper_token = System.Int32;
using int64_t = System.Int64;

namespace Whisper
{

    internal sealed class whisper_token_data
    {

        #region Properties

        public whisper_token id { get; set; }

        public whisper_token tid { get; set; }

        public float p { get; set; }

        public float pt { get; set; }

        public float ptsum { get; set; }

        public int64_t t0 { get; set; }

        public int64_t t1 { get; set; }

        public float vlen { get; set; }

        #endregion

    }

}