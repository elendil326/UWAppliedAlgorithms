using Newtonsoft.Json;
using System;
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

            // Part 1.a)
            Console.WriteLine("Printing part 1.a");
            double[,] optimalA = new ClosedLeastSquareError().GetOptimalA(X, y);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(optimalA)} from exact method"));
            double optimalAError = SquareError.GetSquareError(X, y, optimalA);
            Console.WriteLine($"Minimum square error with exact formula is: {optimalAError}");

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.b)
            Console.WriteLine("Printing part 1.b");
            double[] stepSizes = new[] { 0.0001, 0.001, 0.00125, 0.001001, 0.00101, 0.00095 };
            Parallel.For(0, stepSizes.Length, i =>
            {
                double stepSize = stepSizes[i];
                double[,] gdA = new GDLeastSquareError(stepSize, 20).GetOptimalA(X, y);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareError(X, y, gdA);
                Console.WriteLine($"Minimum square error for stepsize {stepSize} is: {gdAError}");
            });


            // End
            Console.ReadKey();
        }
    }
}
