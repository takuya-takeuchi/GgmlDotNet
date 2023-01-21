namespace GgmlDotNet
{

    /// <summary>
    /// Provides the methods of Ggml.
    /// </summary>
    public static partial class Ggml
    {

        #region Methods

        public static bool CpuHasAVX()
        {
            return NativeMethods.ggml_ggml_cpu_has_avx() > 0;
        }

        public static bool CpuHasAVX2()
        {
            return NativeMethods.ggml_ggml_cpu_has_avx2() > 0;
        }

        public static bool CpuHasAVX512()
        {
            return NativeMethods.ggml_ggml_cpu_has_avx512() > 0;
        }

        public static bool CpuHasFMA()
        {
            return NativeMethods.ggml_ggml_cpu_has_fma() > 0;
        }

        public static bool CpuHasNEON()
        {
            return NativeMethods.ggml_ggml_cpu_has_neon() > 0;
        }

        public static bool CpuHasArmFMA()
        {
            return NativeMethods.ggml_ggml_cpu_has_arm_fma() > 0;
        }

        public static bool CpuHasF16C()
        {
            return NativeMethods.ggml_ggml_cpu_has_f16c() > 0;
        }

        public static bool CpuHasFP16VA()
        {
            return NativeMethods.ggml_ggml_cpu_has_fp16_va() > 0;
        }

        public static bool CpuHasWasmSIMD()
        {
            return NativeMethods.ggml_ggml_cpu_has_wasm_simd() > 0;
        }

        public static bool CpuHasBLAS()
        {
            return NativeMethods.ggml_ggml_cpu_has_blas() > 0;
        }

        #endregion

    }

}