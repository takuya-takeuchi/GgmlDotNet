using Xunit;

// ReSharper disable once CheckNamespace
namespace GgmlDotNet.Tests
{

    public sealed partial class GgmlTest : TestBase
    {

        #region Fields

        private const string ResultDirectory = "Result";

        private const string TestImageDirectory = "TestImages";

        #endregion

        [Fact]
        public void GetNativeVersion()
        {
            var version = Ggml.GetNativeVersion();
            Assert.True(!string.IsNullOrWhiteSpace(version));
        }

    }

}
