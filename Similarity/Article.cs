using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Article
    {
        public int Id { get; set; }

        public Dictionary<int, double> Vector { get; set; } = new Dictionary<int, double>();

        public Article NearestNeighbor { get; set; } = null;
    }
}
