using System;

using int32_t = System.Int32;

namespace Whisper
{

    internal sealed class Pair<T1, T2>
    {

        #region Constructors

        public Pair(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        #endregion

        #region Properties

        public T1 Item1 { get; set; }

        public T2 Item2 { get; set; }

        #endregion

    }

}