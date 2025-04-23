
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Numerics;
using Microsoft.Win32;
using System.Reflection;
using System.Collections;

namespace ConsoleApp1
{
    internal class Program
    {
        BitArray t;

        public bool BlumCheck(BigInteger a, BigInteger b)
        { 
            if ((BigInteger.Remainder(a,4) == 3) && (BigInteger.Remainder(b,4) == 3)) { return true; }
            else { return false; }
        }

        public Tuple<BigInteger,BitArray> encrypt(BigInteger p, BigInteger q, BigInteger seed)
        {
            BigInteger n = p * q;
            if((BigInteger.GreatestCommonDivisor(n, seed) == 1) && BlumCheck(p, q))
            {
                BigInteger[] xarray;
                xarray = new BigInteger[t.Length];
                xarray[0] = BigInteger.ModPow(seed,new BigInteger(2),n);
                for (int i = 1;i < xarray.Length; i++)
                {
                    xarray[i] = BigInteger.ModPow(xarray[i - 1], new BigInteger(2), n);
                }
                BitArray res = new BitArray(xarray.Length);
                for (int i = 0;i< xarray.Length; i++)
                {
                    res[i] = xarray[i].IsEven;
                }
                return new Tuple<BigInteger, BitArray>(xarray[19], res);
            }
            else { return null; }
        }

            


        static void Main(string[] args)
        {
            BigInteger p;
            BigInteger q;

        }
    }
}
