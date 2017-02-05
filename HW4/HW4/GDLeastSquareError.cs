using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class GDLeastSquareError : ILeastSquareError
    {
        public double Alpha { get; private set; }

        public int Iterations { get; private set; }

        public GDLeastSquareError(double alpha, int iterations)
        {
            Alpha = alpha;
            Iterations = iterations;
        }

        public double[,] GetOptimalA(double[,] X, double[,] y)
        {
            int m = X.GetLength(0);
            int n = X.GetLength(1);
            double[,] a = new double[n, 1];

            for (int i = 0; i < Iterations; i++)
            {
                double[,] updatedA = new double[n, 1];
                for (int j = 0; j < n; j++)
                {
                    updatedA[j, 0] = a[j, 0] - Alpha * SquareError.GetDiferentialSquareErrorForAi(X, y, a, j);
                }

                a = updatedA;
            }

            return a;
        }
    }
}
