using System;
using System.Text;

using int64_t = System.Int64;
using size_t = System.Int64;

namespace GgmlDotNet
{

    /// <summary>
    /// Provides the methods of Ggml.
    /// </summary>
    public static partial class Ggml
    {

        #region Methods

        public static GgmlContext Init(GgmlInitParams @params)
        {
            if (@params == null)
                throw new ArgumentNullException(nameof(@params));

            @params.ThrowIfDisposed();

            var context = NativeMethods.ggml_ggml_init(@params.NativePtr);
            return new GgmlContext(context);
        }

        public static void Free(GgmlContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();
            context.Dispose();
        }

        public static void BuildForwardExpand(GgmlCGraph cgraph, GgmlTensor tensor)
        {
            if (cgraph == null)
                throw new ArgumentNullException(nameof(cgraph));
            if (tensor == null)
                throw new ArgumentNullException(nameof(tensor));

            cgraph.ThrowIfDisposed();
            tensor.ThrowIfDisposed();

            NativeMethods.ggml_ggml_build_forward_expand(cgraph.NativePtr, tensor.NativePtr);
        }

        public static void GraphCompute(GgmlContext context, GgmlCGraph cgraph)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (cgraph == null)
                throw new ArgumentNullException(nameof(cgraph));

            context.ThrowIfDisposed();
            cgraph.ThrowIfDisposed();

            NativeMethods.ggml_ggml_graph_compute(cgraph.NativePtr, cgraph.NativePtr);
        }

        public static void GraphPrint(GgmlCGraph cgraph)
        {
            if (cgraph == null)
                throw new ArgumentNullException(nameof(cgraph));

            cgraph.ThrowIfDisposed();

            NativeMethods.ggml_ggml_graph_print(cgraph.NativePtr);
        }

        public static size_t UsedMem(GgmlContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.ThrowIfDisposed();

            return NativeMethods.ggml_ggml_used_mem(context.NativePtr);
        }

        public static void TimeInit()
        {
            NativeMethods.ggml_ggml_time_init();
        }

        public static int64_t TimeMillisecond()
        {
            return NativeMethods.ggml_ggml_time_ms();
        }

        public static int64_t TimeMicrosecond()
        {
            return NativeMethods.ggml_ggml_time_us();
        }

        public static int64_t Cycles()
        {
            return NativeMethods.ggml_ggml_cycles();
        }

        public static int64_t CyclesPerMicrosecond()
        {
            return NativeMethods.ggml_ggml_cycles_per_ms();
        }

        public static size_t TypeSize(GgmlType type)
        {
            return NativeMethods.ggml_ggml_type_size(type);
        }

        public static int NElements(GgmlTensor tensor)
        {
            if (tensor == null)
                throw new ArgumentNullException(nameof(tensor));

            tensor.ThrowIfDisposed();

            return NativeMethods.ggml_ggml_nelements(tensor.NativePtr);
        }

        public static size_t NBytes(GgmlTensor tensor)
        {
            if (tensor == null)
                throw new ArgumentNullException(nameof(tensor));

            tensor.ThrowIfDisposed();

            return NativeMethods.ggml_ggml_nbytes(tensor.NativePtr);
        }

        public static IntPtr GetData(GgmlTensor tensor)
        {
            if (tensor == null)
                throw new ArgumentNullException(nameof(tensor));

            tensor.ThrowIfDisposed();

            return NativeMethods.ggml_ggml_get_data(tensor.NativePtr);
        }

        public static size_t ElementSize(GgmlTensor tensor)
        {
            if (tensor == null)
                throw new ArgumentNullException(nameof(tensor));

            tensor.ThrowIfDisposed();

            return NativeMethods.ggml_ggml_element_size(tensor.NativePtr);
        }

        #endregion

        #region Properties

        private static Encoding _Encoding = Encoding.UTF8;

        public static Encoding Encoding
        {
            get => _Encoding;
            set => _Encoding = value ?? Encoding.UTF8;
        }

        #endregion

    }

}