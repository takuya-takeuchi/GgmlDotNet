using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using id  = System.Int32;
using token = System.String;

namespace Whisper
{

    internal sealed class whisper_vocab
    {

        #region Fields

        public const id token_translate = 50358;

        public const id token_transcribe = 50359;

        #endregion

        #region Constructors

        public whisper_vocab()
        {
            this.n_vocab = 51864;

            this.token_to_id = new Dictionary<token, id>();
            this.id_to_token = new Dictionary<id, token>();

            this.probs_id = new List<Pair<double, id>>();

            this.token_eot = 50256;
            this.token_sot = 50257;
            this.token_prev = 50360;
            this.token_solm = 50361; // ??
            this.token_not = 50362; // no timestamps
            this.token_beg = 50363;
        }

        #endregion

        #region Properties

        public int n_vocab { get; set; }

        public IDictionary<token, id> token_to_id { get; }

        public IDictionary<id, token> id_to_token { get; }
        
        public List<Pair<double, id>> probs_id { get; }

        public id token_eot { get; set; }

        public id token_sot { get; set; }

        public id token_prev { get; set; }

        public id token_solm { get; set; }
        
        public id token_not { get; set; }
        
        public id token_beg { get; set; }

        #endregion

        #region Method
        
        public bool is_multilingual()
        {
            return this.n_vocab == 51865;
        }

        #endregion

    }

}