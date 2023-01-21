using System;
using System.Runtime.InteropServices;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using int64_t = System.Int64;
using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using size_t = System.Int64;

using ggml_context = System.IntPtr;
using ggml_init_params = System.IntPtr;
using ggml_tensor = System.IntPtr;
using ggml_cgraph = System.IntPtr;

// ReSharper disable once CheckNamespace
namespace GgmlDotNet
{

    internal sealed partial class NativeMethods
    {

        internal enum ErrorType
        {

            OK = 0x00000000,

            #region General

            GeneralError = 0x76000000,

            GeneralFileIOError      = -(GeneralError | 0x00000001),

            GeneralOutOfRange       = -(GeneralError | 0x00000002),

            #endregion

        }

        #region Structs

        #region ggml_init_params

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_init_params ggml_ggml_init_params_new();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_init_params_delete(ggml_init_params @params);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_init_params_set_mem_size(ggml_init_params @params,
                                                                     size_t value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern size_t ggml_ggml_init_params_get_mem_size(ggml_init_params @params);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_init_params_set_mem_buffer(ggml_init_params @params,
                                                                       IntPtr value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr ggml_ggml_init_params_get_mem_buffer(ggml_init_params @params);

        #endregion

        #region ggml_tensor

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int32_t ggml_ggml_tensor_get_n_dims(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr ggml_ggml_tensor_get_ne(ggml_tensor tensor, out int32_t length);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr ggml_ggml_tensor_get_nb(ggml_tensor tensor, out int32_t length);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern GgmlType ggml_ggml_tensor_get_type(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr ggml_ggml_tensor_get_data(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_tensor_set_op(ggml_tensor tensor,
                                                          GgmlOp value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern GgmlOp ggml_ggml_tensor_get_op(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_tensor_set_grad(ggml_tensor tensor,
                                                            ggml_tensor value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_tensor_get_grad(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_tensor_set_src0(ggml_tensor tensor,
                                                            ggml_tensor value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_tensor_get_src0(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_tensor_set_src1(ggml_tensor tensor,
                                                            ggml_tensor value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_tensor_get_src1(ggml_tensor tensor);

        #endregion
        
        #region ggml_cgraph

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_cgraph ggml_ggml_cgraph_new();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_cgraph_delete(ggml_cgraph cgraph);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_cgraph_set_n_threads(ggml_cgraph cgraph,
                                                                 int32_t value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int32_t ggml_ggml_cgraph_get_n_threads(ggml_cgraph cgraph);

        #endregion

        #endregion

        #region Functions

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_context ggml_ggml_init(ggml_init_params @params);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_free(ggml_context context);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_time_init();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int64_t ggml_ggml_time_ms();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int64_t ggml_ggml_time_us();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int64_t ggml_ggml_cycles();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int64_t ggml_ggml_cycles_per_ms();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern size_t ggml_ggml_type_size(GgmlType type);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern size_t ggml_ggml_element_size(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int32_t ggml_ggml_nelements(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern size_t ggml_ggml_nbytes(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr ggml_ggml_get_data(ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_build_forward_expand(ggml_cgraph cgraph, ggml_tensor tensor);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_graph_compute(ggml_context ctx, ggml_cgraph cgraph);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern void ggml_ggml_graph_print(ggml_cgraph cgraph);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern size_t ggml_ggml_used_mem(ggml_context ctx);

        #region operations

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_add(ggml_context ctx,
                                                       ggml_tensor a,
                                                       ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_conv_1d_1s(ggml_context ctx,
                                                              ggml_tensor a,
                                                              ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_conv_1d_2s(ggml_context ctx,
                                                              ggml_tensor a,
                                                              ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_cpy(ggml_context ctx,
                                                       ggml_tensor a,
                                                       ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_diag_mask_inf(ggml_context ctx,
                                                                 ggml_tensor a,
                                                                 int32_t n_past);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_dup(ggml_context ctx,
                                                       ggml_tensor a);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_flash_attn(ggml_context ctx,
                                                              ggml_tensor q,
                                                              ggml_tensor k,
                                                              ggml_tensor v,
                                                              bool masked);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_flash_ff(ggml_context ctx,
                                                            ggml_tensor a,
                                                            ggml_tensor b0,
                                                            ggml_tensor b1,
                                                            ggml_tensor c0,
                                                            ggml_tensor c1);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_gelu(ggml_context ctx,
                                                        ggml_tensor a);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_get_rows(ggml_context ctx,
                                                            ggml_tensor a,
                                                            ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_mul(ggml_context ctx,
                                                       ggml_tensor a,
                                                       ggml_tensor b);
        
        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_mul_mat(ggml_context ctx,
                                                           ggml_tensor a,
                                                           ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_i32(ggml_context ctx, int32_t value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_f32(ggml_context ctx, float value);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_tensor_1d(ggml_context ctx,
                                                                 GgmlType type,
                                                                 int32_t ne0);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_tensor_2d(ggml_context ctx,
                                                                 GgmlType type,
                                                                 int32_t ne0,
                                                                 int32_t ne1);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_tensor_3d(ggml_context ctx,
                                                                 GgmlType type,
                                                                 int32_t ne0,
                                                                 int32_t ne1,
                                                                 int32_t ne2);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_new_tensor_4d(ggml_context ctx,
                                                                 GgmlType type,
                                                                 int32_t ne0,
                                                                 int32_t ne1,
                                                                 int32_t ne2,
                                                                 int32_t ne3);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_norm(ggml_context ctx,
                                                        ggml_tensor a);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_permute(ggml_context ctx,
                                                           ggml_tensor a,
                                                           int axis0,
                                                           int axis1,
                                                           int axis2,
                                                           int axis3);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_repeat(ggml_context ctx,
                                                          ggml_tensor a,
                                                          ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_reshape_3d(ggml_context ctx,
                                                              ggml_tensor a,
                                                              int32_t ne0,
                                                              int32_t ne1,
                                                              int32_t ne2);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_scale(ggml_context ctx,
                                                         ggml_tensor a,
                                                         ggml_tensor b);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_soft_max(ggml_context ctx,
                                          ggml_tensor a);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_transpose(ggml_context ctx,
                                                             ggml_tensor a);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_view_1d(ggml_context ctx,
                                                           ggml_tensor a,
                                                           int32_t ne0,
                                                           size_t offset);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern ggml_tensor ggml_ggml_view_2d(ggml_context ctx,
                                                           ggml_tensor a,
                                                           int32_t ne0,
                                                           int32_t ne1,
                                                           size_t nb1,
                                                           size_t offset);

        #endregion

        #region system info

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_avx();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_avx2();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_avx512();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_fma();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_neon();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_arm_fma();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_f16c();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_fp16_va();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_wasm_simd();

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern int ggml_ggml_cpu_has_blas();

        #endregion

        #endregion

    }

}