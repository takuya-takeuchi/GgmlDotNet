using System;

using size_t = System.UInt64;

namespace GgmlDotNet
{

    public sealed class GgmlContext : GgmlNativeObject
    {

        #region Constructors

        internal GgmlContext(IntPtr context)
        {
            this.NativePtr = context;
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

            NativeMethods.ggml_ggml_free(this.NativePtr);
        }

        #endregion

        #endregion

    }

}