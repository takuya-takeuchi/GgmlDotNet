using System;

using int32_t = System.Int32;
using size_t = System.Int64;

namespace GgmlDotNet
{

    /// <summary>
    /// Provides the methods of Ggml.
    /// </summary>
    public static partial class Ggml
    {

        #region Methods

        public static GgmlTensor Add(GgmlContext context,
                                     GgmlTensor a,
                                     GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_add(context.NativePtr,
                                                     a.NativePtr,
                                                     b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Conv1D1S(GgmlContext context,
                                          GgmlTensor a,
                                          GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_conv_1d_1s(context.NativePtr,
                                                            a.NativePtr,
                                                            b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Conv1D2S(GgmlContext context,
                                          GgmlTensor a,
                                          GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_conv_1d_2s(context.NativePtr,
                                                            a.NativePtr,
                                                            b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Cpy(GgmlContext context,
                                     GgmlTensor a,
                                     GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_cpy(context.NativePtr,
                                                     a.NativePtr,
                                                     b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor DiagMaskInf(GgmlContext context,
                                             GgmlTensor a,
                                             int32_t n_past)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_diag_mask_inf(context.NativePtr,
                                                               a.NativePtr,
                                                               n_past);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Dup(GgmlContext context,
                                     GgmlTensor a)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_dup(context.NativePtr,
                                                     a.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor FlashAttn(GgmlContext context,
                                           GgmlTensor q,
                                           GgmlTensor k,
                                           GgmlTensor v,
                                           bool masked)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (q == null)
                throw new ArgumentNullException(nameof(q));
            if (k == null)
                throw new ArgumentNullException(nameof(k));
            if (v == null)
                throw new ArgumentNullException(nameof(v));

            context.ThrowIfDisposed();
            q.ThrowIfDisposed();
            k.ThrowIfDisposed();
            v.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_flash_attn(context.NativePtr,
                                                            q.NativePtr,
                                                            k.NativePtr,
                                                            v.NativePtr,
                                                            masked);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor FlashFF(GgmlContext context,
                                         GgmlTensor a,
                                         GgmlTensor b0,
                                         GgmlTensor b1,
                                         GgmlTensor c0,
                                         GgmlTensor c1)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b0 == null)
                throw new ArgumentNullException(nameof(b0));
            if (b1 == null)
                throw new ArgumentNullException(nameof(b1));
            if (c0 == null)
                throw new ArgumentNullException(nameof(c0));
            if (c1 == null)
                throw new ArgumentNullException(nameof(c1));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b0.ThrowIfDisposed();
            b1.ThrowIfDisposed();
            c0.ThrowIfDisposed();
            c1.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_flash_ff(context.NativePtr,
                                                          a.NativePtr,
                                                          b0.NativePtr,
                                                          b1.NativePtr,
                                                          c0.NativePtr,
                                                          c1.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor GELU(GgmlContext context,
                                                GgmlTensor a)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_gelu(context.NativePtr,
                                                     a.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor GetRows(GgmlContext context,
                                         GgmlTensor a,
                                         GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_get_rows(context.NativePtr,
                                                          a.NativePtr,
                                                          b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Mul(GgmlContext context,
                                     GgmlTensor a,
                                     GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_mul(context.NativePtr,
                                                     a.NativePtr,
                                                     b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor MulMat(GgmlContext context,
                                                   GgmlTensor a,
                                                   GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_mul_mat(context.NativePtr,
                                                         a.NativePtr,
                                                         b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewI32(GgmlContext context, int32_t value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_i32(context.NativePtr,
                                                         value);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewF32(GgmlContext context, float value)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_f32(context.NativePtr,
                                                         value);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewTensor1D(GgmlContext context,
                                             GgmlType type,
                                             int ne0)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_tensor_1d(context.NativePtr,
                                                               type,
                                                               ne0);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewTensor2D(GgmlContext context,
                                             GgmlType type,
                                             int ne0,
                                             int ne1)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_tensor_2d(context.NativePtr,
                                                               type,
                                                               ne0,
                                                               ne1);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewTensor3D(GgmlContext context,
                                             GgmlType type,
                                             int ne0,
                                             int ne1,
                                             int ne2)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_tensor_3d(context.NativePtr,
                                                               type,
                                                               ne0,
                                                               ne1,
                                                               ne2);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor NewTensor4D(GgmlContext context,
                                             GgmlType type,
                                             int ne0,
                                             int ne1,
                                             int ne2,
                                             int ne3)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_new_tensor_4d(context.NativePtr,
                                                               type,
                                                               ne0,
                                                               ne1,
                                                               ne2,
                                                               ne3);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Norm(GgmlContext context,
                                      GgmlTensor a)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_norm(context.NativePtr,
                                                      a.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Permute(GgmlContext context,
                                         GgmlTensor a,
                                         int axis0,
                                         int axis1,
                                         int axis2,
                                         int axis3)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_permute(context.NativePtr,
                                                         a.NativePtr,
                                                         axis0,
                                                         axis1,
                                                         axis2,
                                                         axis3);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Repeat(GgmlContext context,
                                        GgmlTensor a,
                                        GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_repeat(context.NativePtr,
                                                        a.NativePtr,
                                                        b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Reshape3D(GgmlContext context,
                                           GgmlTensor a,
                                           int32_t ne0,
                                           int32_t ne1,
                                           int32_t ne2)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_reshape_3d(context.NativePtr,
                                                            a.NativePtr,
                                                            ne0,
                                                            ne1,
                                                            ne2);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Scale(GgmlContext context,
                                       GgmlTensor a,
                                       GgmlTensor b)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();
            b.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_scale(context.NativePtr,
                                                       a.NativePtr,
                                                       b.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor SoftMax(GgmlContext context,
                                         GgmlTensor a)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_soft_max(context.NativePtr,
                                                          a.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor Transpose(GgmlContext context,
                                           GgmlTensor a)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_transpose(context.NativePtr,
                                                           a.NativePtr);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor View1D(GgmlContext context,
                                        GgmlTensor a,
                                        int32_t ne0,
                                        size_t offset)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_view_1d(context.NativePtr,
                                                         a.NativePtr,
                                                         ne0,
                                                         offset);
            return new GgmlTensor(tensor);
        }

        public static GgmlTensor View2D(GgmlContext context,
                                        GgmlTensor a,
                                        int32_t ne0,
                                        int32_t ne1,
                                        size_t nb1,
                                        size_t offset)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            context.ThrowIfDisposed();
            a.ThrowIfDisposed();

            var tensor = NativeMethods.ggml_ggml_view_2d(context.NativePtr,
                                                         a.NativePtr,
                                                         ne0,
                                                         ne1,
                                                         nb1,
                                                         offset);
            return new GgmlTensor(tensor);
        }

        #endregion

    }

}