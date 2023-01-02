using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace GgmlDotNet
{

    internal sealed partial class NativeMethods
    {

        #region Fields
        
        // Native library file name.
        // If Linux, it will be converted to  libGgmlDotNetNative.so
        // If MacOSX, it will be converted to  libGgmlDotNetNative.dylib
        // If Windows, it will be available after call LoadLibrary.
        // And this file name must not contain period. If it does,
        // CLR does not add extension (.dll) and CLR fails to load library
        internal const string NativeLibrary = "GgmlDotNetNative";

        public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;

        private static readonly WindowsLibraryLoader WindowsLibraryLoader = new WindowsLibraryLoader();

        #endregion

        #region Constructors

        static NativeMethods()
        {
            WindowsLibraryLoader.LoadLibraries(new[]
            {
                $"{NativeLibrary}"
            });
        }

        #endregion

    }

}