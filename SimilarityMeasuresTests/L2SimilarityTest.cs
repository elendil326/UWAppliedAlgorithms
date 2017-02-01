using Similarity;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimilarityTests
{
    [TestClass]
    public class L2SimilarityTest
    {
        [TestMethod]
        public void GetSimilarity_TwoDifferentVectors_ExpectedResult()
        {
            L2Similarity ls = new L2Similarity();
            Article x = new Article();
            Article y = new Article();

            x.Vector[1] = 1;
            x.Vector[2] = 3;
            x.Vector[3] = 5;

            y.Vector[1] = 2;
            y.Vector[2] = 4;
            y.Vector[5] = 3;

            double expected = -6;
            double actual = ls.GetSimilarity(x, y);

            Assert.IsTrue(Math.Abs(expected - actual) < 0.001);
        }
    }
}
