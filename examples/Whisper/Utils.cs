/*
 * This sample program is ported by C# from https://github.com/ggerganov/ggml/tree/master/examples/whisper.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using DrWavDotNet;
using GgmlDotNet;

using int64_t = System.Int64;
using whisper_token = System.Int32;

namespace Whisper
{

    internal sealed partial class Program
    {

        #region Methods

        private static void Rotate<T>(IList<T> list, int times)
        {
            if (list == null || list.Count == 0)
            return;
            if (times == 0)
            {
                return;
            }
            else if (times > 0)
            {
                // ro right
                T first = list[0];
                list.RemoveAt(0);
                list.Add(first);
                Rotate(list, times - 1);
            }
            else if (times < 0)
            {
                // to left
                T last = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                list.Insert(0, last);
                Rotate(list, times + 1);
            }
        }

        #endregion

    }

}