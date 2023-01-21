using System;

namespace GgmlDotNet
{

    public sealed class GgmlCGraph : GgmlNativeObject
    {

        #region Constructors

        public GgmlCGraph()
        {
            this.NativePtr = NativeMethods.ggml_ggml_cgraph_new();
        }

        #endregion

        #region Properties

        public int NThreads
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_cgraph_get_n_threads(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.ggml_ggml_cgraph_set_n_threads(this.NativePtr, value);
            }
        }

        #endregion

        #region Methods

        #region Overrides 

        /// <summary>
        /// Releases all unmanaged resources.
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            base.DisposeUnmanaged();

            if (this.NativePtr == IntPtr.Zero)
                return;
            
            // nothing to do
            NativeMethods.ggml_ggml_cgraph_delete(this.NativePtr);
        }

        #endregion

        #endregion

    }

}