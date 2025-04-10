using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.YoDA
{

    //Need Optimisation! I KNOW!
    public class L2Random
    {
        private static Random rnd;

        static L2Random()
        {
            rnd = new Random();
        }

        public static int Next()
        {
            return rnd.Next();
        }

        public static int Next(int maxValue)
        {
            return rnd.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return rnd.Next(minValue, maxValue);
        }

        public static byte[] NextBytes(int Length)
        {
            byte[] ret = new byte[Length];
            rnd.NextBytes(ret);
            return ret;
        }

        public static double NextDouble()
        {
            return rnd.NextDouble();
        }
    }

}
