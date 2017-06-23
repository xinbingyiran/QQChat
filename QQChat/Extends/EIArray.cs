using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQChat
{
    public class EIArray<T>
    {
        public int Length;
        private T[] v;
        public EIArray()
        {
            Length = 0;
            v = new T[20];
        }
        public EIArray(int length)
        {
            Length = 0;
            v = new T[length];
        }
        public T this[int i]
        {
            get
            {
                if (i < 0) { return default(T); };
                if (Length > i)
                {
                    return v[i];
                }
                return default(T);
            }
            set
            {
                if (Length <= i)
                {
                    Length = i + 1;
                    if (Length > v.Length)
                    {
                        var clenth = v.Length;
                        var nv = new T[Length];
                        Array.Copy(v, nv, clenth);
                        v = nv;
                    }
                }
                v[i] = value;
            }
        }
    }
}
