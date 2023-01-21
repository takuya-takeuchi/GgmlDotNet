using System;
using System.Collections.Generic;

namespace Whisper
{

    internal sealed class whisper_print_user_data
    {

        #region Properties

        public WhisperParams @params { get; set; }

        public List<Buffer<float>> pcmf32s { get; set; }

        #endregion

    }

}