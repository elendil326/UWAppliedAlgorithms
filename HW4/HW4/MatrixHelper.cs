using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4
{
    public class MatrixHelper
    {
        public double[,] GenerateRandomMatrix()
        {
            return GenerateRandomMatrix(1000, 100, 0, 1);
        }

        public double[,] GenerateRandomMatrix(int m, int n, double mean, double standardDeviation)
        {
            RandomHelper random = new RandomHelper();
            double[,] randomMatrix = new double[m, n];

            Parallel.For(0, m, i =>
            {
                Parallel.For(0, n, j =>
                {
                    randomMatrix[i, j] = random.GetGaussianRandom(mean, standardDeviation);
                });
            });

            return randomMatrix;
        }

        public double[,] DotProduct(double[,] A, double[,] B)
        {
            if (A.GetLength(1) != B.GetLength(0))
            {
                throw new ArgumentException($"The lengths between matrices do not match. A: {A.GetLength(1)}, B: {B.GetLength(0)}");
            }

            int n = A.GetLength(0);
            int m = B.GetLength(1);
            int p = A.GetLength(1);
            double[,] result = new double[n, m];

            Parallel.For(0, n, i =>
            {
                Parallel.For(0, m, j =>
                {
                    double accumulator = 0;

                    for (int k = 0; k < p; k++)
                    {
                        accumulator += A[i, k] * B[k, j];
                    };

                    result[i, j] = accumulator;
                });
                
            });

            return result;
        }

        public double[,] DotProduct(double[,] A, double x)
        {
            double[,] result = new double[A.GetLength(0), A.GetLength(1)];

            Parallel.For(0, A.GetLength(0), i =>
            {
                Parallel.For(0, A.GetLength(1), j =>
                {
                    result[i, j] = A[i, j] * x;
                });
            });

            return result;
        }

        public double[,] Transpose(double[,] A)
        {
            double[,] result = new double[A.GetLength(1), A.GetLength(0)];

            Parallel.For(0, A.GetLength(0), i =>
            {
                Parallel.For(0, A.GetLength(1), j =>
                {
                    result[j, i] = A[i, j];
                });
            });

            return result;
        }

        public bool MatrixAreEqual(double[,] A, double[,] B)
        {
            return MatrixAreEqual(A, B, 0.00001);
        }

        public bool MatrixAreEqual(double[,] A, double[,] B, double errorRange)
        {
            if (errorRange < 0) { throw new ArgumentException("Error Range is negative", nameof(errorRange)); }

            if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            {
                return false;
            }

            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    if (Math.Abs(A[i, j] - B[i, j]) > errorRange)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public double[,] GenerateIdentityMatrix(int n)
        {
            double[,] result = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                result[i, i] = 1;
            }

            return result;
        }

        public double[,] MatrixInverse(double[,] A)
        {
            if (A.GetLength(0) != A.GetLength(1)) { throw new ArgumentException("Matrix is not square", nameof(A)); }
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = A.GetLength(0);
            double[,] result = new double[n,n];
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i,j] = A[i,j];

            double[][] lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(A, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b); // 
                for (int j = 0; j < n; ++j)
                    result[j,i] = x[j];
            }
            return result;
        } // MatrixInverse

        public int MatrixDecompose(double[,] m, out double[][] lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.GetLength(0);

            // make a copy of m[][] into result lu[][]
            lum = MatrixCreate(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i][j] = m[i,j];


            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j][j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i][j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                } // i

                if (piv != j)
                {
                    double[] tmp = lum[piv]; // swap rows j, piv
                    lum[piv] = lum[j];
                    lum[j] = tmp;

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j][j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i][j] / xjj;
                        lum[i][j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i][k] -= xij * lum[j][k];
                    }
                }

            } // j

            return toggle;
        } // MatrixDecompose

        static double[] Helper(double[][] luMatrix, double[] b) // helper
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        } // Helper

        public double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            Parallel.For(0, rows, i =>
                result[i] = new double[cols]);
            return result;
        }

        public double[,] MatrixAddition(double[,] A, double[,] B)
        {
            if (A.GetLength(0) != B.GetLength(0) || A.GetLength(1) != B.GetLength(1))
            {
                throw new ArgumentException($"The arrays do not have the same dimension size. A: [{A.GetLength(0)}, {A.GetLength(1)}]; B: [{B.GetLength(0)}, {B.GetLength(1)}]");
            }

            double[,] result = new double[A.GetLength(0), A.GetLength(1)];

            Parallel.For(0, A.GetLength(0), i =>
            {
                Parallel.For(0, B.GetLength(1), j =>
                {
                    result[i, j] = A[i, j] + B[i, j];
                });
            });

            return result;
        }
    }
}
