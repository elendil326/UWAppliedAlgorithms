using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public abstract class VectorSimilarity
    {
        public virtual void TraverseVectors(Article x, Article y)
        {
            // First check elements of X
            foreach (KeyValuePair<int, double> kvp in x.Vector)
            {
                int xWordId = kvp.Key;
                int yWordId = kvp.Key;
                double xCount = kvp.Value;
                double yCount = y.Vector.ContainsKey(yWordId) ? y.Vector[yWordId] : 0;

                SimilarityOperation(xCount, yCount);
            }

            // Then check any left overs that are only in Y. X elements are already accounted for.
            foreach (KeyValuePair<int, double> kvp in y.Vector)
            {
                int yWordId = kvp.Key;
                double yCount = kvp.Value;
                if (x.Vector.ContainsKey(yWordId))
                {
                    continue;
                }

                SimilarityOperation(0, yCount);
            }
        }

        public abstract void SimilarityOperation(double x, double y);

        public abstract double GetSimilarity(Article x, Article y);
    }
}
