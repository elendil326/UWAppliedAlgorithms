using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Article> Articles { get; private set; } = new List<Article>();
    }
}
