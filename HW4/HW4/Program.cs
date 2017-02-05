using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Part 1
            MatrixHelper matrixHelper = new MatrixHelper();
            int n = 100;
            int m = 1000;
            double[,] X = matrixHelper.GenerateRandomMatrix(m, n, 0, 1);
            double[,] a_true = matrixHelper.GenerateRandomMatrix(n, 1, 0, 1);
            double[,] y = matrixHelper.DotProduct(X, a_true);
            y = matrixHelper.MatrixAddition(y, matrixHelper.GenerateRandomMatrix(m, 1, 0, 0.1));

            SquareError sqe = new SquareError();

            // Part 1.a)
            double optimalA = sqe.GetSquareError<ClosedLeastSquareError>(X, y);
            Console.WriteLine($"Minimum square error is: {optimalA}");

            // Part 1.b)

            // End
            Console.ReadKey();
        }
    }
}
