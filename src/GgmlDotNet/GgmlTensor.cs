using System;
using System.Runtime.InteropServices;

using int32_t = System.Int32;
using size_t = System.Int64;

namespace GgmlDotNet
{

    public sealed class GgmlTensor : GgmlNativeObject
    {

        #region Constructors

        internal GgmlTensor(IntPtr tensor, bool isEnabledDispose = true):
            base(isEnabledDispose)
        {
            this.NativePtr = tensor;
        }

        #endregion

        #region Properties

        public int32_t NDims
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_tensor_get_n_dims(this.NativePtr);
            }
        }

        public IntPtr Data
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_tensor_get_data(this.NativePtr);
            }
        }

        public GgmlTensor Grad
        {
            get
            {
                this.ThrowIfDisposed();
                var tensor = NativeMethods.ggml_ggml_tensor_get_grad(this.NativePtr);
                return new GgmlTensor(tensor, false);
            }
            set
            {
                this.ThrowIfDisposed();
                value?.ThrowIfDisposed();
                NativeMethods.ggml_ggml_tensor_set_grad(this.NativePtr, value != null ? value.NativePtr : IntPtr.Zero);
            }
        }

        public int32_t[] NE
        {
            get
            {
                this.ThrowIfDisposed();
                var array = NativeMethods.ggml_ggml_tensor_get_ne(this.NativePtr, out var length);
                if (array == IntPtr.Zero)
                    return null;
                
                var ret = new int32_t[length];
                Marshal.Copy(array, ret, 0, length);
                return ret;
            }
        }

        public size_t[] NB
        {
            get
            {
                this.ThrowIfDisposed();
                var array = NativeMethods.ggml_ggml_tensor_get_nb(this.NativePtr, out var length);
                if (array == IntPtr.Zero)
                    return null;
                
                var ret = new size_t[length];
                Marshal.Copy(array, ret, 0, length);
                return ret;
            }
        }

        public GgmlOp Op
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_tensor_get_op(this.NativePtr);
            }
            set
            {
                this.ThrowIfDisposed();
                NativeMethods.ggml_ggml_tensor_set_op(this.NativePtr, value);
            }
        }

        public GgmlTensor Src0
        {
            get
            {
                this.ThrowIfDisposed();
                var tensor = NativeMethods.ggml_ggml_tensor_get_src0(this.NativePtr);
                return new GgmlTensor(tensor, false);
            }
            set
            {
                this.ThrowIfDisposed();
                value?.ThrowIfDisposed();
                NativeMethods.ggml_ggml_tensor_set_src0(this.NativePtr, value != null ? value.NativePtr : IntPtr.Zero);
            }
        }

        public GgmlTensor Src1
        {
            get
            {
                this.ThrowIfDisposed();
                var tensor = NativeMethods.ggml_ggml_tensor_get_src1(this.NativePtr);
                return new GgmlTensor(tensor, false);
            }
            set
            {
                this.ThrowIfDisposed();
                value?.ThrowIfDisposed();
                NativeMethods.ggml_ggml_tensor_set_src1(this.NativePtr, value != null ? value.NativePtr : IntPtr.Zero);
            }
        }

        public GgmlType Type
        {
            get
            {
                this.ThrowIfDisposed();
                return NativeMethods.ggml_ggml_tensor_get_type(this.NativePtr);
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
            // NativeMethods.ggml_ggml_free(this.NativePtr);
        }

        #endregion

        #endregion

    }

}