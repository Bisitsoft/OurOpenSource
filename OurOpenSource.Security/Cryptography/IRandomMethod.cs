using System;
using System.Collections.Generic;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
    public interface IRandomMethod
    {
        int SeedNeedLength { get; }
        void SetSeed(byte[] seed);
        int GetInt(int minValue, int maxValue);
        virtual byte[] GetBytes(int n, byte minValue, byte maxValue)
        {
            int i;
            byte[] r = new byte[n];
            for (i = 0; i < n; i++)
            {
                //unchecked //GetInt不一定正确。
                //{
                r[i] = (byte)GetInt(minValue, maxValue);
                //}
            }
            return r;
        }
    }
}
