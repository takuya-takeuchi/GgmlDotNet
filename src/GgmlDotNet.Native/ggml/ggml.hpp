#ifndef _CPP_GGML_H_
#define _CPP_GGML_H_

#include "shared.hpp"
#include "export.hpp"

#include <ggml/ggml.h>

#pragma region structs

#pragma region ggml_init_params

DLLEXPORT ggml_init_params* ggml_ggml_init_params_new()
{
    return new ggml_init_params();
}

DLLEXPORT void ggml_ggml_init_params_delete(const ggml_init_params* params)
{
    delete params;
}

DLLEXPORT void ggml_ggml_init_params_set_mem_size(ggml_init_params* const params,
                                                  const size_t value)
{
    params->mem_size = value;
}

DLLEXPORT size_t ggml_ggml_init_params_get_mem_size(ggml_init_params* const params)
{
    return params->mem_size;
}

DLLEXPORT void ggml_ggml_init_params_set_mem_buffer(ggml_init_params* const params,
                                                    void* value)
{
    params->mem_buffer = value;
}

DLLEXPORT void* ggml_ggml_init_params_get_mem_buffer(ggml_init_params* const params)
{
    return params->mem_buffer;
}

#pragma endregion ggml_init_params

#pragma region ggml_tensor

DLLEXPORT int32_t ggml_ggml_tensor_get_n_dims(ggml_tensor* const tensor)
{
    return tensor->n_dims;
}

DLLEXPORT int32_t* ggml_ggml_tensor_get_ne(ggml_tensor* const tensor, int32_t* length)
{
    *length = GGML_MAX_DIMS;
    return &(tensor->ne[0]);
}

DLLEXPORT size_t* ggml_ggml_tensor_get_nb(ggml_tensor* const tensor, int32_t* length)
{
    *length = GGML_MAX_DIMS;
    return &(tensor->nb[0]);
}

DLLEXPORT ggml_type ggml_ggml_tensor_get_type(ggml_tensor* const tensor)
{
    return tensor->type;
}

DLLEXPORT void* ggml_ggml_tensor_get_data(ggml_tensor* const tensor)
{
    return tensor->data;
}

DLLEXPORT void ggml_ggml_tensor_set_op(ggml_tensor* const tensor,
                                       const ggml_op value)
{
    tensor->op = value;
}

DLLEXPORT ggml_op ggml_ggml_tensor_get_op(ggml_tensor* const tensor)
{
    return tensor->op;
}

DLLEXPORT void ggml_ggml_tensor_set_grad(ggml_tensor* const tensor,
                                         ggml_tensor* const value)
{
    tensor->grad = value;
}

DLLEXPORT ggml_tensor* ggml_ggml_tensor_get_grad(ggml_tensor* const tensor)
{
    return tensor->grad;
}

DLLEXPORT void ggml_ggml_tensor_set_src0(ggml_tensor* const tensor,
                                         ggml_tensor* const value)
{
    tensor->src0 = value;
}

DLLEXPORT ggml_tensor* ggml_ggml_tensor_get_src0(ggml_tensor* const tensor)
{
    return tensor->src0;
}

DLLEXPORT void ggml_ggml_tensor_set_src1(ggml_tensor* const tensor,
                                         ggml_tensor* const value)
{
    tensor->src1 = value;
}

DLLEXPORT ggml_tensor* ggml_ggml_tensor_get_src1(ggml_tensor* const tensor)
{
    return tensor->src1;
}

#pragma endregion ggml_tensor

#pragma region ggml_cgraph

DLLEXPORT ggml_cgraph* ggml_ggml_cgraph_new()
{
    return new ggml_cgraph();
}

DLLEXPORT void ggml_ggml_cgraph_delete(const ggml_cgraph* cgraph)
{
    delete cgraph;
}

DLLEXPORT void ggml_ggml_cgraph_set_n_threads(ggml_cgraph* const cgraph,
                                              const int32_t value)
{
    cgraph->n_threads = value;
}

DLLEXPORT int32_t ggml_ggml_cgraph_get_n_threads(ggml_cgraph* const cgraph)
{
    return cgraph->n_threads;
}

#pragma endregion ggml_cgraph

#pragma endregion structs

#pragma region functions

DLLEXPORT ggml_context* ggml_ggml_init(const ggml_init_params* params)
{
    const auto& p = *params;
    return ::ggml_init(p);
}

DLLEXPORT void ggml_ggml_free(ggml_context* const context)
{
    ::ggml_free(context);
}

DLLEXPORT void ggml_ggml_build_forward_expand(ggml_cgraph* const cgraph, ggml_tensor* const tensor)
{
    ::ggml_build_forward_expand(cgraph, tensor);
}

DLLEXPORT void ggml_ggml_graph_compute(ggml_context* const ctx, ggml_cgraph* const cgraph)
{
    ::ggml_graph_compute(ctx, cgraph);
}

DLLEXPORT void ggml_ggml_graph_print(const ggml_cgraph* cgraph)
{
    ::ggml_graph_print(cgraph);
}

DLLEXPORT size_t ggml_ggml_used_mem(const ggml_context* ctx)
{
    return ::ggml_used_mem(ctx);
}

DLLEXPORT void ggml_ggml_time_init()
{
    ::ggml_time_init();
}

DLLEXPORT int64_t ggml_ggml_time_ms()
{
    return ::ggml_time_ms();
}

DLLEXPORT int64_t ggml_ggml_time_us()
{
    return ::ggml_time_us();
}

DLLEXPORT int64_t ggml_ggml_cycles()
{
    return ::ggml_cycles();
}

DLLEXPORT int64_t ggml_ggml_cycles_per_ms()
{
    return ::ggml_cycles_per_ms();
}

DLLEXPORT size_t ggml_ggml_type_size(const ggml_type type)
{
    return ::ggml_type_size(type);
}

DLLEXPORT size_t ggml_ggml_element_size(const ggml_tensor* tensor)
{
    return ::ggml_element_size(tensor);
}

DLLEXPORT int32_t ggml_ggml_nelements(const ggml_tensor* tensor)
{
    return ::ggml_nelements(tensor);
}

DLLEXPORT size_t ggml_ggml_nbytes(const ggml_tensor* tensor)
{
    return ::ggml_nbytes(tensor);
}

DLLEXPORT void* ggml_ggml_get_data(const ggml_tensor* tensor)
{
    return ::ggml_get_data(tensor);
}

#pragma region operations

DLLEXPORT ggml_tensor* ggml_ggml_add(ggml_context* const ctx,
                                     ggml_tensor* const a,
                                     ggml_tensor* const b)
{
    return ::ggml_add(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_conv_1d_1s(ggml_context* const ctx,
                                            ggml_tensor* const a,
                                            ggml_tensor* const b)
{
    return ::ggml_conv_1d_1s(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_conv_1d_2s(ggml_context* const ctx,
                                            ggml_tensor* const a,
                                            ggml_tensor* const b)
{
    return ::ggml_conv_1d_2s(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_cpy(ggml_context* const ctx,
                                     ggml_tensor* const a,
                                     ggml_tensor* const b)
{
    return ::ggml_cpy(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_diag_mask_inf(ggml_context* const ctx,
                                               ggml_tensor* const a,
                                               const int n_past)
{
    return ::ggml_diag_mask_inf(ctx, a, n_past);
}

DLLEXPORT ggml_tensor* ggml_ggml_dup(ggml_context* const ctx,
                                     ggml_tensor* const a)
{
    return ::ggml_dup(ctx, a);
}

DLLEXPORT ggml_tensor* ggml_ggml_flash_attn(ggml_context* const ctx,
                                            ggml_tensor* const q,
                                            ggml_tensor* const k,
                                            ggml_tensor* const v,
                                            const bool masked)
{
    return ::ggml_flash_attn(ctx, q, k, v, masked);
}

DLLEXPORT ggml_tensor* ggml_ggml_flash_ff(ggml_context* const ctx,
                                          ggml_tensor* const a,
                                          ggml_tensor* const b0,
                                          ggml_tensor* const b1,
                                          ggml_tensor* const c0,
                                          ggml_tensor* const c1)
{
    return ::ggml_flash_ff(ctx, a, b0, b1, c0, c1);
}

DLLEXPORT ggml_tensor* ggml_ggml_gelu(ggml_context* const ctx,
                                      ggml_tensor* const a)
{
    return ::ggml_gelu(ctx, a);
}

DLLEXPORT ggml_tensor* ggml_ggml_get_rows(ggml_context* const ctx,
                                          ggml_tensor* const a,
                                          ggml_tensor* const b)
{
    return ::ggml_get_rows(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_mul(ggml_context* const ctx,
                                     ggml_tensor* const a,
                                     ggml_tensor* const b)
{
    return ::ggml_mul(ctx, a, b);
}
        
DLLEXPORT ggml_tensor* ggml_ggml_mul_mat(ggml_context* const ctx,
                                         ggml_tensor* const a,
                                         ggml_tensor* const b)
{
    return ::ggml_mul_mat(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_i32(ggml_context* const ctx, const int32_t value)
{
    return ::ggml_new_i32(ctx, value);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_f32(ggml_context* const ctx, const float value)
{
    return ::ggml_new_f32(ctx, value);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_tensor_1d(ggml_context* const ctx,
                                               const ggml_type type,
                                               const int ne0)
{
    return ::ggml_new_tensor_1d(ctx, type, ne0);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_tensor_2d(ggml_context* const ctx,
                                               const ggml_type type,
                                               const int ne0,
                                               const int ne1)
{
    return ::ggml_new_tensor_2d(ctx, type, ne0, ne1);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_tensor_3d(ggml_context* const ctx,
                                               const ggml_type type,
                                               const int ne0,
                                               const int ne1,
                                               const int ne2)
{
    return ::ggml_new_tensor_3d(ctx, type, ne0, ne1, ne2);
}

DLLEXPORT ggml_tensor* ggml_ggml_new_tensor_4d(ggml_context* const ctx,
                                               const ggml_type type,
                                               const int ne0,
                                               const int ne1,
                                               const int ne2,
                                               const int ne3)
{
    return ::ggml_new_tensor_4d(ctx, type, ne0, ne1, ne2, ne3);
}

DLLEXPORT ggml_tensor* ggml_ggml_norm(ggml_context* const ctx,
                                      ggml_tensor* const a)
{
    return ::ggml_norm(ctx, a);
}

DLLEXPORT ggml_tensor* ggml_ggml_permute(ggml_context* const ctx,
                                         ggml_tensor* const a,
                                         int axis0,
                                         int axis1,
                                         int axis2,
                                         int axis3)
{
    return ::ggml_permute(ctx, a, axis0, axis1, axis2, axis3);
}

DLLEXPORT ggml_tensor* ggml_ggml_repeat(ggml_context* const ctx,
                                        ggml_tensor* const a,
                                        ggml_tensor* const b)
{
    return ::ggml_repeat(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_reshape_3d(ggml_context* const ctx,
                                            ggml_tensor* const a,
                                            const int ne0,
                                            const int ne1,
                                            const int ne2)
{
    return ::ggml_reshape_3d(ctx, a, ne0, ne1, ne2);
}

DLLEXPORT ggml_tensor* ggml_ggml_scale(ggml_context* const ctx,
                                       ggml_tensor* const a,
                                       ggml_tensor* const b)
{
    return ::ggml_scale(ctx, a, b);
}

DLLEXPORT ggml_tensor* ggml_ggml_soft_max(ggml_context* const ctx,
                                          ggml_tensor* const a)
{
    return ::ggml_soft_max(ctx, a);
}

DLLEXPORT ggml_tensor* ggml_ggml_transpose(ggml_context* const ctx,
                                           ggml_tensor* const a)
{
    return ::ggml_transpose(ctx, a);
}

DLLEXPORT ggml_tensor* ggml_ggml_view_1d(ggml_context* const ctx,
                                         ggml_tensor* const a,
                                         const int ne0,
                                         const size_t offset)
{
    return ::ggml_view_1d(ctx, a, ne0, offset);
}

DLLEXPORT ggml_tensor* ggml_ggml_view_2d(ggml_context* const ctx,
                                         ggml_tensor* const a,
                                         const int ne0,
                                         const int ne1,
                                         const size_t nb1,
                                         const size_t offset)
{
    return ::ggml_view_2d(ctx, a, ne0, ne1, nb1, offset);
}

#pragma endregion operations

DLLEXPORT int32_t ggml_ggml_cpu_has_avx()
{
    return ::ggml_cpu_has_avx();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_avx2()
{
    return ::ggml_cpu_has_avx();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_avx512()
{
    return ::ggml_cpu_has_avx512();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_fma()
{
    return ::ggml_cpu_has_fma();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_neon()
{
    return ::ggml_cpu_has_neon();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_arm_fma()
{
    return ::ggml_cpu_has_arm_fma();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_f16c()
{
    return ::ggml_cpu_has_f16c();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_fp16_va()
{
    return ::ggml_cpu_has_fp16_va();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_wasm_simd()
{
    return ::ggml_cpu_has_wasm_simd();
}

DLLEXPORT int32_t ggml_ggml_cpu_has_blas()
{
    return ::ggml_cpu_has_avx();
}

#pragma endregion functions

#endif // _CPP_STRING_H_
