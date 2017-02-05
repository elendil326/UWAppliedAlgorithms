using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class ClosedLeastSquareError : ILeastSquareError
    {
        public double[,] GetOptimalA(double[,] X, double[,] y)
        {
            MatrixHelper matrixHelper = new MatrixHelper();
            double[,] result = new double[1,1];

            return result;
        }
    }
}
