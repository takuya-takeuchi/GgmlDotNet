using System;
using System.Runtime.InteropServices;

using size_t = System.Int64;

// ReSharper disable once CheckNamespace
namespace GgmlDotNet
{

    internal sealed partial class NativeMethods
    {

        #region cstd

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr cstd_memcpy(IntPtr dest, IntPtr src, size_t count);

        [DllImport(NativeLibrary, CallingConvention = CallingConvention)]
        public static extern IntPtr cstd_memset(IntPtr dest, int ch, size_t count);

        #endregion

    }

}