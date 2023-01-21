using System;

using size_t = System.Int64;

namespace GgmlDotNet
{

    public sealed class GgmlInitParams : GgmlNativeObject
    {

        #region Constructors

        public GgmlInitParams()
        {
            this.NativePtr = NativeMethods.ggml_ggml_init_params_new();
        }

        #endregion

        #region Properties

        public size_t MemSize
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_init_params_get_mem_size(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.ggml_ggml_init_params_set_mem_size(this.NativePtr, value);
            }
        }

        public IntPtr MemBuffer
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_init_params_get_mem_buffer(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.ggml_ggml_init_params_set_mem_buffer(this.NativePtr, value);
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

            NativeMethods.ggml_ggml_init_params_delete(this.NativePtr);
        }

        #endregion

        #endregion

    }

}