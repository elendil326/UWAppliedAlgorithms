using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class L2Similarity : VectorSimilarity
    {
        private double Accumulator { get; set; } = 0;

        public override double GetSimilarity(Article x, Article y)
        {
            TraverseVectors(x, y);
            return -1 * Math.Sqrt(Accumulator);
        }

        public override void SimilarityOperation(double x, double y)
        {
            Accumulator += Math.Pow(x - y, 2);
        }
    }
}
