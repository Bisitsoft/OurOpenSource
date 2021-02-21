using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Utility
{
    public static class ComparerMethods
    {
        public static int ComapreSameLengthByteArray(byte[] x, byte[] y)
        {
            int i, temp;
            if (x.Length != y.Length)
            {
                throw new ArgumentException("x.Length not equals y.Length .");
            }
            for (i = 0; i < x.Length; i++)
            {
                temp = (int)((short)(x[i]) - (short)(y[i]));
                if(temp != 0)
                {
                    return temp;
                }
            }
            return 0;
        }
    }
}
