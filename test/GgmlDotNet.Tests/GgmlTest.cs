using System;
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
        public void GgmlInitParams()
        {
            const long memSize = 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();

            @params.MemSize = memSize;
            Assert.Equal(memSize, @params.MemSize);

            @params.MemBuffer = memBuffer;
            Assert.Equal(memBuffer, @params.MemBuffer);

            @params.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { var _ = @params.MemSize; });
            Assert.Throws<ObjectDisposedException>(() => { @params.MemSize = memSize; });
            Assert.Throws<ObjectDisposedException>(() => { var _ = @params.MemBuffer; });
            Assert.Throws<ObjectDisposedException>(() => { @params.MemBuffer = memBuffer; });
        }

        [Fact]
        public void GgmlCGraph()
        {
            const int nThreads = 4;
            var cgraph = new GgmlCGraph();

            cgraph.NThreads = nThreads;
            Assert.Equal(nThreads, cgraph.NThreads);

            cgraph.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { var _ = cgraph.NThreads; });
            Assert.Throws<ObjectDisposedException>(() => { cgraph.NThreads = nThreads; });
        }

        [Fact]
        public void Init()
        {
            const long memSize = 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);
            context.Dispose();

            Assert.Throws<ArgumentNullException>(() => { Ggml.Init(null); });

            @params.Dispose();
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Init(@params); });
        }

        [Fact]
        public void Free()
        {
            const long memSize = 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);
            Ggml.Free(context);

            Assert.Throws<ArgumentNullException>(() => { Ggml.Free(null); });            
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Free(context); });

            @params.Dispose();
        }

        [Fact]
        public void TimeInit()
        {
            Ggml.TimeInit();
            Assert.True(true);
        }

        [Fact]
        public void TimeMicrosecond()
        {
            var time = Ggml.TimeMicrosecond();
            Assert.True(time >= 0);
        }

        [Fact]
        public void TimeMillisecond()
        {
            var time = Ggml.TimeMillisecond();
            Assert.True(time >= 0);
        }

        [Fact]
        public void Cycles()
        {
            var time = Ggml.Cycles();
            Assert.True(time >= 0);
        }

        [Fact]
        public void CyclesPerMicrosecond()
        {
            var time = Ggml.CyclesPerMicrosecond();
            Assert.True(time >= 0);
        }

        [Fact]
        public void CpuHasAVX()
        {
            Assert.True(Ggml.CpuHasAVX());
        }

    }

}
