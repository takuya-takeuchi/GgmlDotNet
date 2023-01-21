using System;
using System.Collections.Generic;

using GgmlDotNet;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class whisper_model
    {

        #region Constructors

        public whisper_model()
        {
            this.type = e_model.MODEL_UNKNOWN;

            this.hparams = new whisper_hparams();
            this.filters = new whisper_filters();

            this.layers_encoder = new List<whisper_layer_encoder>();
            this.layers_decoder = new List<whisper_layer_decoder>();

            this.tensors = new Dictionary<string, GgmlTensor>();
        }

        #endregion

        #region Properties

        public e_model type { get; set; }

        public whisper_hparams hparams { get; }

        public whisper_filters filters { get; }

        public GgmlTensor e_pe { get; set; }

        public GgmlTensor e_conv_1_w { get; set; }

        public GgmlTensor e_conv_1_b { get; set; }

        public GgmlTensor e_conv_2_w { get; set; }

        public GgmlTensor e_conv_2_b { get; set; }

        public GgmlTensor e_ln_w { get; set; }

        public GgmlTensor e_ln_b { get; set; }

        public GgmlTensor d_pe { get; set; }

        public GgmlTensor d_te { get; set; }

        public GgmlTensor d_ln_w { get; set; }

        public GgmlTensor d_ln_b { get; set; }

        public List<whisper_layer_encoder> layers_encoder;

        public List<whisper_layer_decoder> layers_decoder;

        public GgmlTensor memory_k { get; set; }

        public GgmlTensor memory_v { get; set; }

        public GgmlTensor memory_cross_k { get; set; }

        public GgmlTensor memory_cross_v { get; set; }

        public GgmlContext ctx { get; set; }

        public GgmlContext ctx_mem { get; set; }

        public int n_loaded { get; set; }

        public IDictionary<string, GgmlTensor> tensors { get; }

        #endregion

    }

}