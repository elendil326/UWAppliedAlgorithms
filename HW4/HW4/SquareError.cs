using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HW4
{
    public static class SquareError
    {
        public static double GetSquareError(double[,] X, double[,] y, double[,] a)
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

                    double[,] atDotXi = matrixHelper.DotProduct(at, xi);
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

        public static double GetDiferentialSquareErrorForAi(double[,] X, double[,] y, double[,] a, int ai)
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

                    double[,] atDotXi = matrixHelper.DotProduct(at, xi);
                    double localPartialResult = atDotXi[0, 0];
                    localPartialResult -= y[i, 0];
                    localPartialResult *= xi[ai, 0];

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

            return result;
        }

        public static double GetL2NormSquare(double[,] A)
        {
            if (A.GetLength(1) > 1) { throw new ArgumentException("This should be a Vector."); }

            double result = 0;

            for (int i = 0; i < A.GetLength(0); i++)
            {
                result += Math.Pow(A[i, 0], 2);
            }

            return result;
        }

        public static double GetSquareErrorL2(double[,] X, double[,] y, double[,] a, double lambda)
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

                    double[,] atDotXi = matrixHelper.DotProduct(at, xi);
                    double localPartialResult = atDotXi[0, 0];
                    localPartialResult -= y[i, 0];
                    localPartialResult *= localPartialResult;
                    localPartialResult += (lambda * GetL2NormSquare(a));

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

        public static double GetDiferentialSquareErrorL2ForAi(double[,] X, double[,] y, double[,] a, double lambda, int ai)
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

                    double[,] atDotXi = matrixHelper.DotProduct(at, xi);
                    double localPartialResult = atDotXi[0, 0];
                    localPartialResult -= y[i, 0];
                    localPartialResult *= xi[ai, 0];
                    localPartialResult += (2 * lambda * a[ai, 0]);

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

            return result;
        }
    }
}
