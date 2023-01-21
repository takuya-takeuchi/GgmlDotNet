using System;
using Xunit;

// ReSharper disable once CheckNamespace
namespace GgmlDotNet.Tests
{

    public sealed partial class GgmlTest : TestBase
    {

        [Fact]
        public void NewTensor1D()
        {
            const long memSize = 128 * 1024 * 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);

            var tensor = Ggml.NewTensor1D(context, GgmlType.F32, 10);
            Assert.Equal(1, tensor.NDims);
            Assert.Equal(10, tensor.NE[0]);
            Assert.Equal(10 * sizeof(float), tensor.NB[1]);

            Ggml.Free(context);

            Assert.Throws<ArgumentNullException>(() => { Ggml.Free(null); });            
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Free(context); });

            @params.Dispose();
        }

        [Fact]
        public void NewTensor2D()
        {
            const long memSize = 128 * 1024 * 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);

            var tensor = Ggml.NewTensor2D(context, GgmlType.I16, 10, 20);
            Assert.Equal(2, tensor.NDims);
            Assert.Equal(10, tensor.NE[0]);
            Assert.Equal(20, tensor.NE[1]);
            Assert.Equal(10 * sizeof(short), tensor.NB[1]);
            Assert.Equal(10 * 20 * sizeof(short), tensor.NB[2]);

            Ggml.Free(context);

            Assert.Throws<ArgumentNullException>(() => { Ggml.Free(null); });            
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Free(context); });

            @params.Dispose();
        }

        [Fact]
        public void NewTensor3D()
        {
            const long memSize = 128 * 1024 * 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);

            var tensor = Ggml.NewTensor3D(context, GgmlType.I32, 10, 20, 30);
            Assert.Equal(3, tensor.NDims);
            Assert.Equal(10, tensor.NE[0]);
            Assert.Equal(20, tensor.NE[1]);
            Assert.Equal(30, tensor.NE[2]);
            Assert.Equal(10 * sizeof(int), tensor.NB[1]);
            Assert.Equal(10 * 20 * sizeof(int), tensor.NB[2]);
            Assert.Equal(10 * 20 * 30 * sizeof(int), tensor.NB[3]);

            Ggml.Free(context);

            Assert.Throws<ArgumentNullException>(() => { Ggml.Free(null); });            
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Free(context); });

            @params.Dispose();
        }

        [Fact]
        public void NewTensor4D()
        {
            const long memSize = 128 * 1024 * 1024;
            var memBuffer = IntPtr.Zero;
            var @params = new GgmlInitParams();
            @params.MemSize = memSize;
            @params.MemBuffer = memBuffer;

            var context = Ggml.Init(@params);

            var tensor = Ggml.NewTensor4D(context, GgmlType.F16, 10, 20, 30, 40);
            Assert.Equal(4, tensor.NDims);
            Assert.Equal(10, tensor.NE[0]);
            Assert.Equal(20, tensor.NE[1]);
            Assert.Equal(30, tensor.NE[2]);
            Assert.Equal(40, tensor.NE[3]);
            Assert.Equal(10 * sizeof(float) / 2, tensor.NB[1]);
            Assert.Equal(10 * 20 * sizeof(float) / 2, tensor.NB[2]);
            Assert.Equal(10 * 20 * 30 * sizeof(float) / 2, tensor.NB[3]);
            // Assert.Equal(10 * 20 * 30 * 40 * sizeof(float) / 2, tensor.NB[4]);

            Ggml.Free(context);

            Assert.Throws<ArgumentNullException>(() => { Ggml.Free(null); });            
            Assert.Throws<ObjectDisposedException>(() => { Ggml.Free(context); });

            @params.Dispose();
        }

    }

}
