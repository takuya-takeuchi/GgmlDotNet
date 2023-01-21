using System;

using GgmlDotNet;

namespace Whisper
{

    internal sealed class whisper_layer_encoder
    {

        #region Properties

        public GgmlTensor attn_ln_0_w { get; set; }

        public GgmlTensor attn_ln_0_b { get; set; }

        public GgmlTensor attn_ln_1_w { get; set; }

        public GgmlTensor attn_ln_1_b { get; set; }

        public GgmlTensor attn_q_w { get; set; }

        public GgmlTensor attn_q_b { get; set; }

        public GgmlTensor attn_k_w { get; set; }

        public GgmlTensor attn_v_w { get; set; }

        public GgmlTensor attn_v_b { get; set; }

        public GgmlTensor mlp_ln_w { get; set; }

        public GgmlTensor mlp_ln_b { get; set; }

        public GgmlTensor mlp_0_w { get; set; }

        public GgmlTensor mlp_0_b { get; set; }

        public GgmlTensor mlp_1_w { get; set; }

        public GgmlTensor mlp_1_b { get; set; }

        #endregion

    }

}