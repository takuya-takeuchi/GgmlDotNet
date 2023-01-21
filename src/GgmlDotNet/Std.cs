using System;

using size_t = System.Int64;

namespace GgmlDotNet
{

    /// <summary>
    /// Provides the methods of c++ standard library.
    /// </summary>
    public static class Std
    {

        #region Methods

        public static IntPtr MemCpy(IntPtr dest, IntPtr src, size_t count)
        {
            return NativeMethods.cstd_memcpy(dest, src, count);
        }

        public static IntPtr MemSet(IntPtr dest, int ch, size_t count)
        {
            return NativeMethods.cstd_memset(dest, ch, count);
        }

        #endregion

    }

}