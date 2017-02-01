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
    public class CosineSimilarityTest
    {

        [TestMethod]
        public void GetSimilarity_TwoDifferentVectors_ExpectedResult()
        {
            CosineSimilarity cs = new CosineSimilarity();
            Article x = new Article();
            Article y = new Article();

            x.Vector[1] = 1;
            x.Vector[2] = 3;
            x.Vector[3] = 5;

            y.Vector[1] = 2;
            y.Vector[2] = 4;
            y.Vector[5] = 3;

            double expected = 0.43943537440204113472653679374377;
            double actual = cs.GetSimilarity(x, y);

            Assert.IsTrue(Math.Abs(expected - actual) < 0.001);
        }
    }
}
