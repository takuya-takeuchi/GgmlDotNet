using System;
using System.IO;
using System.Text;

namespace GgmlDotNet
{

    /// <summary>
    /// Provides the methods of Ggml.
    /// </summary>
    public static partial class Ggml
    {

        #region Methods

        /// <summary>
        /// Get the string representation of the version of wrapper library of Ggml.
        /// </summary>
        /// <returns>The string representation of the version of wrapper library of Ggml.</returns>
        public static string GetNativeVersion()
        {
            return StringHelper.FromStdString(NativeMethods.get_version(), true);
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