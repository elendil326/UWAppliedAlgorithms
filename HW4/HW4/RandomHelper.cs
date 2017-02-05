using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class RandomHelper
    {
        private Random _random = new Random();

        public double GetGaussianRandom()
        {
            return GetGaussianRandom(0, 1);
        }

        public double GetGaussianRandom(double mean, double standardDeviation)
        {
            double u1 = _random.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + standardDeviation * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }
    }
}
