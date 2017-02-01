using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public static class GroupFactory
    {
        public static List<Group> GetGroupArticles(string dataCsv, string labelCsv, string groupsCsv)
        {
            List<Group> groups = new List<Group>();
            List<string> groupNames = GetGroupNames(groupsCsv);
            Dictionary<int, int> articleGroupMapping = GetArticleGroupMapping(labelCsv);
            Dictionary<int, Article> articleMapping = GetArticleMapping(dataCsv);
            Dictionary<int, HashSet<int>> groupLabelMapping = new Dictionary<int, HashSet<int>>();
            for (int m = 0; m < groupNames.Count; m++)
            {
                Group group = new Group
                {
                    Id = m + 1,
                    Name = groupNames[m]
                };

                if (!groupLabelMapping.ContainsKey(group.Id))
                {
                    groupLabelMapping[group.Id] = new HashSet<int>(articleGroupMapping.Where(kvp => kvp.Value == group.Id).Select(kvp => kvp.Key));
                }
                HashSet<int> articlesOfGroupM = groupLabelMapping[group.Id];
                group.Articles.AddRange(articleMapping.Where(kvp => articlesOfGroupM.Contains(kvp.Key)).Select(kvp => kvp.Value));

                groups.Add(group);
            }

            return groups;
        }

        public static List<string> GetGroupNames(string groupsCsv)
        {
            List<string> groupNames = new List<string>();
            using (StreamReader groupsReader = new StreamReader(groupsCsv))
            {
                string line = null;
                while ((line = groupsReader.ReadLine()) != null)
                {
                    groupNames.Add(line);
                }
            }

            return groupNames;
        }

        public static Dictionary<int, int> GetArticleGroupMapping(string labelCsv)
        {
            Dictionary<int, int> labelGroupMapping = new Dictionary<int, int>();
            using (StreamReader labelReader = new StreamReader(labelCsv))
            {
                int n = 0;
                string line = null;
                while ((line = labelReader.ReadLine()) != null)
                {
                    n++;
                    int m = int.Parse(line);
                    labelGroupMapping[n] = m;
                }
            }

            return labelGroupMapping;
        }

        public static Dictionary<int, Article> GetArticleMapping(string data50Csv)
        {
            Dictionary<int, Article> articleMapping = new Dictionary<int, Article>();
            using (StreamReader articleReader = new StreamReader(data50Csv))
            {
                string line = null;
                while ((line = articleReader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    int articleId = int.Parse(parts[0]);
                    int wordId = int.Parse(parts[1]);
                    int count = int.Parse(parts[2]);

                    Article article = new Article();
                    if (!articleMapping.ContainsKey(articleId))
                    {
                        article.Id = articleId;
                        articleMapping[articleId] = article;
                    }

                    article = articleMapping[articleId];
                    article.Vector[wordId] = count;
                }
            }

            return articleMapping;
        }
    }
}
