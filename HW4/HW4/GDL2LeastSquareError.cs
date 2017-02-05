using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class GDL2LeastSquareError : ILeastSquareError
    {
        public double Alpha { get; private set; }

        public int Iterations { get; private set; }

        public double Lambda { get; private set; }

        public GDL2LeastSquareError(double alpha, int iterations, double lambda)
        {
            Alpha = alpha;
            Iterations = iterations;
            Lambda = lambda;
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
                    updatedA[j, 0] = a[j, 0] - Alpha * SquareError.GetDiferentialSquareErrorL2ForAi(X, y, a, Lambda, j);
                }

                a = updatedA;
            }

            return a;
        }
    }
}
