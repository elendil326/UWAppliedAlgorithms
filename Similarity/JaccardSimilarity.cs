using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class JaccardSimilarity : VectorSimilarity
    {
        private double Numerator { get; set; } = 0;

        private double Denominator { get; set; } = 0;

        public override void SimilarityOperation(double x, double y)
        {
            Numerator += Math.Min(x, y);
            Denominator += Math.Max(x, y);
        }

        public override double GetSimilarity(Article x, Article y)
        {
            TraverseVectors(x, y);
            return Numerator / Denominator;
        }
    }
}
