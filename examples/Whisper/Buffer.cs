using System;
using System.Runtime.InteropServices;

using size_t = System.Int64;

namespace Whisper
{

    internal sealed unsafe class Buffer<T>
        where T : struct
    {

        #region Fields

        private readonly int _TypeSize = 0;

        private Indexer<T> _Indexer;

        #endregion

        #region Constructors

        public Buffer()
        {
            switch (default(T))
            {
                case float f:
                    this._TypeSize = 4;
                    break;
                case short s:
                    this._TypeSize = 2;
                    break;
                case byte i:
                    this._TypeSize = 1;
                    break;
                default:
                    throw new NotSupportedException();
            }

            this.Data = IntPtr.Zero;
        }

        #endregion

        #region Properties

        public IntPtr Data
        {
            get;
            private set;
        }

        public T this[long index]
        {
            get
            {
                return this._Indexer.Get((int)index);
            }
            set
            {
                this._Indexer.Set((int)index, value);
            }
        }

        public T this[int index]
        {
            get
            {
                return this._Indexer.Get(index);
            }
            set
            {
                this._Indexer.Set(index, value);
            }
        }

        public size_t Size
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public void Free()
        {
            if (this.Data != IntPtr.Zero)
                Marshal.FreeHGlobal(this.Data);
            this.Data = IntPtr.Zero;
            this._Indexer = null;
        }

        public void Resize(size_t size)
        {
            this.Free();            
            this.Size = size;
            this.Data = Marshal.AllocHGlobal((int)(size * this._TypeSize));

            switch (default(T))
            {
                case float f:
                    this._Indexer = new FloatIndexer(this.Data) as Indexer<T>;
                    break;
                case short s:
                    this._Indexer = new ShortIndexer(this.Data) as Indexer<T>;
                    break;
                case byte i:
                    this._Indexer = new ByteIndexer(this.Data) as Indexer<T>;
                    break;
            }
        }

        public void Reserve(size_t size)
        {
            this.Free();            
            this.Size = size;
            this.Data = Marshal.AllocHGlobal((int)(size * this._TypeSize));

            switch (default(T))
            {
                case float f:
                    this._Indexer = new FloatIndexer(this.Data) as Indexer<T>;
                    break;
                case short s:
                    this._Indexer = new ShortIndexer(this.Data) as Indexer<T>;
                    break;
                case byte i:
                    this._Indexer = new ByteIndexer(this.Data) as Indexer<T>;
                    break;
            }
        }

        #endregion

        private abstract class Indexer<T>
            where T: struct
        {

            private readonly IntPtr _Ptr;

            protected Indexer(IntPtr ptr)
            {
                this._Ptr = ptr;
            }

            public abstract T Get(int index);

            public abstract void Set(int index, T value);

        }

        private sealed class ByteIndexer : Indexer<byte>
        {

            private readonly byte* _Pointer;

            public ByteIndexer(IntPtr ptr):
                base(ptr)
            {
                this._Pointer = (byte*)ptr.ToPointer();
            }

            public override byte Get(int index)
            {
                return this._Pointer[index];
            }

            public override void Set(int index, byte value)
            {
                this._Pointer[index] = value;
            }

        }

        private sealed class ShortIndexer : Indexer<short>
        {

            private readonly short* _Pointer;

            public ShortIndexer(IntPtr ptr):
                base(ptr)
            {
                this._Pointer = (short*)ptr.ToPointer();
            }

            public override short Get(int index)
            {
                return this._Pointer[index];
            }

            public override void Set(int index, short value)
            {
                this._Pointer[index] = value;
            }

        }

        private sealed class FloatIndexer : Indexer<float>
        {

            private readonly float* _Pointer;

            public FloatIndexer(IntPtr ptr):
                base(ptr)
            {
                this._Pointer = (float*)ptr.ToPointer();
            }

            public override float Get(int index)
            {
                return this._Pointer[index];
            }

            public override void Set(int index, float value)
            {
                this._Pointer[index] = value;
            }

        }

    }

}