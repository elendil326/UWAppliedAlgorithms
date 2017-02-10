using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class SGDLeastSquareError : ILeastSquareError
    {
        private Random _random = new Random();

        public double Alpha { get; private set; }

        public int Iterations { get; private set; }

        public List<double> IterationResult { get; private set; }

        public SGDLeastSquareError(double alpha, int iterations)
        {
            Alpha = alpha;
            Iterations = iterations;
            IterationResult = new List<double>(Iterations + 1);
        }

        public double[,] GetOptimalA(double[,] X, double[,] y)
        {
            int m = X.GetLength(0);
            int n = X.GetLength(1);
            double[,] a = new double[n, 1];

            IterationResult.Add(SquareError.GetSquareError(X, y, a));
            for (int i = 0; i < Iterations; i++)
            {
                double[,] updatedA = new double[n, 1];
                int j = _random.Next(0, n);
                updatedA[j, 0] = a[j, 0] - Alpha * SquareError.GetDiferentialSquareErrorForAi(X, y, a, j);

                a = updatedA;
                IterationResult.Add(SquareError.GetSquareError(X, y, a));
            }

            return a;
        }
    }
}
;