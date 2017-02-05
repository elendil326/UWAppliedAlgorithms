using HW4;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW4Tests
{
    [TestClass]
    public class MatrixHelperTest
    {
        #region GenerateRandomMatrix
        [TestMethod]
        public void GenereateRandomMatrix_Default_NotEmpty()
        {
            MatrixHelper helper = new MatrixHelper();

            double[,] random = helper.GenerateRandomMatrix();

            Assert.AreNotEqual(0, random.Length);
            bool allZeros = true;
            for (int i = 0; i < random.GetLength(0); i++)
            {
                for (int j = 0; j < random.GetLength(1); j++)
                {
                    if (random[i,j] != 0)
                    {
                        allZeros = false;
                    }
                }
            }

            Assert.IsFalse(allZeros);
        }

        [TestMethod]
        public void GenerateRandomMatrix_PassSize_ExpectedSize()
        {
            MatrixHelper helper = new MatrixHelper();

            double[,] random = helper.GenerateRandomMatrix(10, 20, 0, 1);

            Assert.AreEqual(10, random.GetLength(0));
            Assert.AreEqual(20, random.GetLength(1));
        }
        #endregion GenerateRandomMatrix

        #region MatrixAreEqual
        [TestMethod]
        public void MatrixAreEqual_DifferenceSmallerThanError_True()
        {
            double[,] A =
            {
                { 10, 20, 30 },
                { 4, 5, 6 }
            };

            double[,] B =
            {
                { 10.0001, 20.0001, 30.0001 },
                { 4.0001, 5.0001, 6.0001 }
            };

            MatrixHelper helper = new MatrixHelper();

            Assert.IsTrue(helper.MatrixAreEqual(A, B, 0.001));
        }

        [TestMethod]
        public void MatrixAreEqual_DifferenceBiggerThanError_False()
        {
            double[,] A =
            {
                { 10, 20, 30 },
                { 4, 5, 6 }
            };

            double[,] B =
            {
                { 10.0001, 20.0001, 30.0001 },
                { 4.0001, 5.0001, 6.0001 }
            };

            MatrixHelper helper = new MatrixHelper();

            Assert.IsFalse(helper.MatrixAreEqual(A, B, 0.00001));
        }
        #endregion MatrixAreEqual

        #region DotProduct
        [TestMethod]
        public void DotProduct_FixedMatrices_Success()
        {
            double[,] A =
            {
                { 10, 20, 30 },
                { 4, 5, 6 }
            };

            double[,] B =
            {
                { 10, 20},
                { 4, 5},
                { 30, 6 }
            };

            double[,] expected =
            {
                { 1080, 480 },
                { 240, 141 }
            };

            MatrixHelper helper = new MatrixHelper();

            double[,] actual = helper.DotProduct(A, B);

            Assert.IsTrue(helper.MatrixAreEqual(expected, actual, 0.0001));
        }

        [TestMethod]
        public void DotProduct_MatrixScalar_Sucess()
        {
            double x = -1;
            double[,] A =
            {
                { 0, 1, 2, 3 },
                { 8, 7, 6, 5 }
            };

            double[,] expected =
            {
                { 0, -1, -2, -3 },
                { -8, -7, -6, -5 }
            };
            MatrixHelper helper = new MatrixHelper();

            double[,] actual = helper.DotProduct(A, x);

            Assert.IsTrue(helper.MatrixAreEqual(expected, actual));
        }
        #endregion DotProduct

        #region MatrixAddition
        [TestMethod]
        public void MatrixAddition_SimpleMatrices_Success()
        {
            double[,] A =
            {
                { 10, 20, 30 },
                { 4, 5, 6 }
            };

            double[,] B =
            {
                { 11, 22, 33 },
                { 44, 55, 66 }
            };

            double[,] expected =
            {
                { 21, 42, 63 },
                { 48, 60, 72 }
            };

            MatrixHelper helper = new MatrixHelper();

            double[,] actual = helper.MatrixAddition(A, B);

            Assert.IsTrue(helper.MatrixAreEqual(expected, actual));
        }
        #endregion MatrixAddition

        #region Transpose
        [TestMethod]
        public void Transponse_SimpleMatrix_Success()
        {
            double[,] A =
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 }
            };

            double[,] expected =
            {
                { 1, 5 },
                { 2, 6 },
                { 3, 7 },
                { 4, 8 }
            };

            MatrixHelper helper = new MatrixHelper();

            Assert.IsTrue(helper.MatrixAreEqual(expected, helper.Transpose(A)));
        }
        #endregion Transpose

        #region GenerateIdentityMatrix
        [TestMethod]
        public void GenerateIdentityMatrix_SmallNumber_Success()
        {
            double[,] expected =
            {
                { 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 1 },
            };
            MatrixHelper helper = new MatrixHelper();

            Assert.IsTrue(helper.MatrixAreEqual(expected, helper.GenerateIdentityMatrix(5)));

        }
        #endregion GenerateIdentytMatrix

        #region MatrixInverse
        [TestMethod]
        public void MatrixInverse_RandomMatrix_Success()
        {
            MatrixHelper helper = new MatrixHelper();
            double[,] A = helper.GenerateRandomMatrix(100, 100, 0, 1);
            double[,] expected = helper.GenerateIdentityMatrix(A.GetLength(0));

            double[,] B = helper.MatrixInverse(A);

            Assert.IsTrue(helper.MatrixAreEqual(expected, helper.DotProduct(A, B)));
        }
        #endregion MatrixInverse
    }
}
