using Similarity;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimilarityTests
{
    /// <summary>
    /// Summary description for JaccardSimilarityTest
    /// </summary>
    [TestClass]
    public class JaccardSimilarityTest
    {

        [TestMethod]
        public void GetSimilarity_TwoDifferentVectors_ExpectedResult()
        {
            JaccardSimilarity js = new JaccardSimilarity();
            Article x = new Article();
            Article y = new Article();

            x.Vector[1] = 1;
            x.Vector[2] = 3;
            x.Vector[3] = 5;

            y.Vector[1] = 2;
            y.Vector[2] = 4;
            y.Vector[5] = 3;

            double expected = 0.28571428571428571428571428571429;
            double actual = js.GetSimilarity(x, y);

            Assert.IsTrue(Math.Abs(expected - actual) < 0.001);
        }
    }
}
