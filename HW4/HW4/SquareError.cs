using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HW4
{
    public class SquareError
    {
        public double GetSquareError<T>(double[,] X, double[,] y) where T : ILeastSquareError, new()
        {
            ILeastSquareError leastSquareError = new T();
            double[,] a = leastSquareError.GetOptimalA(X, y);

            Task.Run(() =>Console.WriteLine($"Using A {JsonConvert.SerializeObject(a)} from method {leastSquareError.GetType().ToString()}"));

            return GetSquareError(X, y, a);
        }

        public double GetSquareError(double[,] X, double[,] y, double[,] a)
        {
            int m = X.GetLength(0);
            int n = X.GetLength(1);
            double result = 0;
            object lockResult = new object();
            MatrixHelper matrixHelper = new MatrixHelper();
            double[,] at = matrixHelper.Transpose(a);

            Parallel.For(
                // From inclusive
                0,
                // To exclusive
                m,
                // local initial partial result
                () => 0.0d,
                // Loop body
                (i, loopState, partialResult) =>
                {
                    double[,] xi = new double[n, 1];
                    for (int j = 0; j < n; j++)
                    {
                        xi[j, 0] = X[i, j];
                    }

                    double[,] atDotXi = matrixHelper.DotProduct(at, xi); ;
                    double localPartialResult = atDotXi[0,0];
                    localPartialResult -= y[i, 0];
                    localPartialResult *= localPartialResult;

                    return localPartialResult + partialResult;
                },
                // Final step for each local context
                (localPartialSum) =>
                {
                    lock (lockResult)
                    {
                        result += localPartialSum;
                    }
                });

            return 0.5 * result;
        }
    }
}
