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
            int iterations = 20;
            Parallel.For(0, stepSizes.Length, i =>
            {
                double stepSize = stepSizes[i];
                double[,] gdA = new GDLeastSquareError(stepSize, iterations).GetOptimalA(X, y);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareError(X, y, gdA);
                Console.WriteLine($"Minimum square error for stepsize {stepSize} after {iterations} iterations is: {gdAError}");
            });

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.c)
            Console.WriteLine("Printing part 1.c");
            stepSizes = new[] { 0.001, 0.01, 0.02 };
            iterations = 1000;
            Parallel.For(0, stepSizes.Length, i =>
            {
                double stepSize = stepSizes[i];
                double[,] gdA = new SGDLeastSquareError(stepSize, iterations).GetOptimalA(X, y);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareError(X, y, gdA);
                Console.WriteLine($"Minimum square error for stepsize {stepSize} after {iterations} iterations is: {gdAError}");
            });

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.d)
            Console.WriteLine("Printing part 1.d");
            int train_m = 100;
            int test_m = 1000;
            n = 100;
            double[,] X_train = matrixHelper.GenerateRandomMatrix(train_m, n, 0, 1);
            a_true = matrixHelper.GenerateRandomMatrix(n, 1, 0, 1);
            double[,] y_train = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_train, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(train_m, 1, 0, 1), 0.5));
            double[,] X_test = matrixHelper.GenerateRandomMatrix(test_m, n, 0, 1);
            double[,] y_test = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_test, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(test_m, 1, 0, 1), 0.5));

            double step = 0.0095;
            iterations = 20;
            double[,] gdAVector = new GDLeastSquareError(step, iterations).GetOptimalA(X_train, y_train);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            double gdAErrorResult = SquareError.GetSquareError(X_train, y_train, gdAVector);
            Console.WriteLine($"Minimum square error in train data (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_test, y_test, gdAVector);
            Console.WriteLine($"Minimum square error in test data  (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            train_m = 20;
            gdAVector = new GDLeastSquareError(step, iterations).GetOptimalA(X_train, y_train);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_train, y_train, gdAVector);
            Console.WriteLine($"Minimum square error in train data (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_test, y_test, gdAVector);
            Console.WriteLine($"Minimum square error in test data  (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            // End
            Console.ReadKey();
        }
    }
}
