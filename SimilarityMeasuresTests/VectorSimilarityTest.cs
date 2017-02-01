using Similarity;
using Similarity.Fakes;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimilarityTests
{
    [TestClass]
    public class VectorSimilarityTest
    {
        [TestMethod]
        public void TraverseVector_TwoDifferentVectors_AllTraversed()
        {
            int totalUniqueSums = 0;
            StubVectorSimilarity vectorSim = new StubVectorSimilarity
            {
                CallBase = true,
                SimilarityOperationDoubleDouble = (x, y) =>
                {
                    totalUniqueSums++;
                }
            };

            Article a = new Article();
            Article b = new Article();

            a.Vector[1] = 2;
            a.Vector[2] = 3;
            b.Vector[1] = 1;
            b.Vector[4] = 0;
            b.Vector[7] = 8;

            vectorSim.TraverseVectors(a, b);

            Assert.AreEqual(4, totalUniqueSums);
        }
    }
}
