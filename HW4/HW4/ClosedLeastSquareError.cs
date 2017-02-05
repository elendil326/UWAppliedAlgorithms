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
            double[,] Xt = matrixHelper.Transpose(X);
            double[,] result = matrixHelper.DotProduct(Xt, X);
            result = matrixHelper.MatrixInverse(result);
            result = matrixHelper.DotProduct(result, Xt);
            result = matrixHelper.DotProduct(result, y);

            return result;
        }
    }
}
