using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class CosineSimilarity : VectorSimilarity
    {
        private double Numerator { get; set; } = 0;

        private double DenominatorX { get; set; } = 0;

        private double DenominatorY { get; set; } = 0;

        public override void SimilarityOperation(double x, double y)
        {
            Numerator += x * y;
            DenominatorX += x * x;
            DenominatorY += y * y;
        }

        public override double GetSimilarity(Article x, Article y)
        {
            TraverseVectors(x, y);
            double denominator = Math.Sqrt(DenominatorX) * Math.Sqrt(DenominatorY);

            return Numerator / denominator;
        }
    }
}
