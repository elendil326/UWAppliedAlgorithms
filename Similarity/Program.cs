using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Similarity
{
    class Program
    {
        #region Members
        #region HeatMapTemplate
        static string _heatMapTemplate = @"<html>
<head>
    <script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.min.js""></script>
    <script src=""http://code.highcharts.com/highcharts.js""></script>
    <script src=""http://code.highcharts.com/modules/heatmap.js""></script>
    <script src=""http://code.highcharts.com/modules/exporting.js""></script>
</head>
<body>
<div id=""container"" style=""width:100%; height: 100%; margin: 0 auto""></div>
<script>
    $('#container').highcharts({
        chart: {
            type: 'heatmap'
        },
        title: {
            text: '{{Algorithm}}'
        },
        tooltip: {
            formatter: function () {
                return '<b>Group ' + this.series.yAxis.categories[this.point.y] + ' compared with Group ' + this.series.xAxis.categories[this.point.x] +' </b> had a similarity <br><b>' + this.point.value;
            }
        },
        xAxis: {
            categories: {{Categories}}
        },
        yAxis: {
            categories: {{Categories}}
        },
        colorAxis: {
            stops: [
                [0, '#0E025E'],
                [0.5, '#6DADF7'],
                [1, '#E1FAF9']
            ],
            startOnTick: false,
            endOnTick: false
        },
        series: [{
            data: {{Data}}
        }]
    });
</script>
</body>
</html>";
        #endregion HeatMapTemplate

        #region ScatterPlotTemplate
        static string _scatterPlotTemplate = @"<html>
<head>
	<script src=""http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.min.js""></script>
	<script src=""http://code.highcharts.com/highcharts.js""></script>
	<script src=""http://code.highcharts.com/modules/heatmap.js""></script>
	<script src=""http://code.highcharts.com/modules/exporting.js""></script>
</head>
<body>
<div id=""container"" style=""width:100%; height: 100%; margin: 0 auto""></div>
<script>
$(function () {
    Highcharts.chart('container', {
        chart: {
            type: 'scatter',
            zoomType: 'xy'
        },
        title: {
            text: '{{ChartTitle}}'
        },
        xAxis: {
            title: {
                enabled: true,
                text: 'original cosine similarity'
            },
            startOnTick: true,
            endOnTick: true,
            showLastLabel: true
        },
        yAxis: {
            title: {
                text: 'cosine similarity with {{Dimensions}} dimensions'
            }
        },
        plotOptions: {
            scatter: {
                marker: {
                    radius: 5,
                    states: {
                        hover: {
                            enabled: true,
                            lineColor: 'rgb(100,100,100)'
                        }
                    }
                },
                states: {
                    hover: {
                        marker: {
                            enabled: false
                        }
                    }
                },
                tooltip: {
                    headerFormat: '<b>{series.name}</b><br>',
                    pointFormat: '{point.x}, {point.y}'
                }
            }
        },
        series: [{
            name: 'Similarity',
            color: 'rgba(119, 152, 191, .5)',
            data: {{Data}}
        }]
    });
});
</script>
</body>
</html>";
        #endregion ScatterPlotTemplate

        static string _categoriesTemplateString = "{{Categories}}";
        static string _dataTemplateString = "{{Data}}";
        static string _algorithmTemplateString = "{{Algorithm}}";
        static string _dimensionsTemplateString = "{{Dimensions}}";
        static string _chartTitleTemplateString = "{{ChartTitle}}";

        static string _data50Csv = @".\Data\data50.csv";
        static string _labelCsv = @".\Data\label.csv";
        static string _groupsCsv = @".\Data\groups.csv";
        #endregion Members

        #region Main
        static void Main(string[] args)
        {
            List<Group> groups = GroupFactory.GetGroupArticles(_data50Csv, _labelCsv, _groupsCsv);
            double[,] jaccardMatrix = new double[groups.Count, groups.Count];
            double[,] cosineMatrix = new double[groups.Count, groups.Count];
            double[,] l2Matrix = new double[groups.Count, groups.Count];

            VectorSimilarity[] Similarity = { new JaccardSimilarity(), new CosineSimilarity(), new L2Similarity() };
            Dictionary<Type, double[,]> metricToMatrixMap = new Dictionary<Type, double[,]>
            {
                { typeof(JaccardSimilarity), jaccardMatrix },
                { typeof(CosineSimilarity), cosineMatrix },
                { typeof(L2Similarity), l2Matrix }
            };

            Parallel.ForEach(metricToMatrixMap, kvp =>
            {
                Type similarityMethodType = kvp.Key;
                double[,] matrixResult = kvp.Value;

                Parallel.For(0, groups.Count, i =>
                {
                    for (int j = i; j < groups.Count; j++)
                    {
                        matrixResult[i, j] = CompareGroups(groups[i], groups[j], similarityMethodType);
                        matrixResult[j, i] = matrixResult[i, j];
                    };
                });
            });

            // Part 1.b
            Console.WriteLine("Printing similarity results in heatmap");
            PrintOnHeatMap(jaccardMatrix, groups, "Jaccard Similarity");
            PrintOnHeatMap(cosineMatrix, groups, "Cosine Similarity");
            PrintOnHeatMap(l2Matrix, groups, "L2 Similarity");

            // Part 1.d
            double[,] nearestNeighborGroupMatrix = GetNearestNeighborMatrix<JaccardSimilarity>(groups);

            Console.WriteLine("Printing nearest neighbor in heat map");
            PrintOnHeatMap(nearestNeighborGroupMatrix, groups, "Nearest-neighbor with Jaccard similarity");

            // Part 2.a
            Stopwatch sw = new Stopwatch();
            int[] dimmensions = { 10, 25, 50, 100 };
            Dictionary<int, List<Group>> dimensionGroupsMapping = new Dictionary<int, List<Group>>();
            foreach (int d in dimmensions)
            {
                sw = new Stopwatch();
                sw.Start();
                List<Group> projectedInD = GetProjectedVectors(d, groups, RandomGaussianNumber);
                dimensionGroupsMapping[d] = projectedInD;
                sw.Stop();
                Console.WriteLine($"Time to project to dimensions {d} is {sw.Elapsed.TotalSeconds} seconds");

                sw = new Stopwatch();
                sw.Start();
                nearestNeighborGroupMatrix = GetNearestNeighborMatrix<CosineSimilarity>(projectedInD);
                sw.Stop();
                Console.WriteLine($"Time to generate nearest neighbor for dimension {d} is {sw.Elapsed.TotalSeconds} seconds");

                Console.WriteLine("Printing nearest neighbor in heat map");
                PrintOnHeatMap(nearestNeighborGroupMatrix, groups, $"Nearest-neighbor with Cosine similarity projected in {d}-dimmension");
            }

            // Produce the normal one for comparison
            sw = new Stopwatch();
            sw.Start();
            nearestNeighborGroupMatrix = GetNearestNeighborMatrix<CosineSimilarity>(groups);
            sw.Stop();
            Console.WriteLine($"Time to generate nearest neighbor for dimension is {sw.Elapsed.TotalSeconds} seconds");
            Console.WriteLine("Printing nearest neighbor in heat map");
            PrintOnHeatMap(nearestNeighborGroupMatrix, groups, "Nearest-neighbor with Cosine similarity");

            // part 2.b
            List<double> cosineSimilarityReal = CompareArticles<CosineSimilarity>(3, groups);
            List<double> cosineSimilarity10D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[10]);
            List<double> cosineSimilarity25D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[25]);
            List<double> cosineSimilarity50D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[50]);
            List<double> cosineSimilarity100D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[100]);

            PrintOnScatterPlot(10, "10-dimension chart", cosineSimilarityReal, cosineSimilarity10D);
            PrintOnScatterPlot(25, "25-dimension chart", cosineSimilarityReal, cosineSimilarity25D);
            PrintOnScatterPlot(50, "50-dimension chart", cosineSimilarityReal, cosineSimilarity50D);
            PrintOnScatterPlot(100, "100-dimension chart", cosineSimilarityReal, cosineSimilarity100D);

            // part 2.c
            dimensionGroupsMapping = new Dictionary<int, List<Group>>();
            foreach (int d in dimmensions)
            {
                List<Group> projectedInD = GetProjectedVectors(d, groups, RandomSignedNumber);
                dimensionGroupsMapping[d] = projectedInD;
            }

            cosineSimilarity10D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[10]);
            cosineSimilarity25D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[25]);
            cosineSimilarity50D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[50]);
            cosineSimilarity100D = CompareArticles<CosineSimilarity>(3, dimensionGroupsMapping[100]);

            PrintOnScatterPlot(10, "10-dimension chart", cosineSimilarityReal, cosineSimilarity10D);
            PrintOnScatterPlot(25, "25-dimension chart", cosineSimilarityReal, cosineSimilarity25D);
            PrintOnScatterPlot(50, "50-dimension chart", cosineSimilarityReal, cosineSimilarity50D);
            PrintOnScatterPlot(100, "100-dimension chart", cosineSimilarityReal, cosineSimilarity100D);
        }
        #endregion Main

        #region PrivateMethods
        private static double CompareGroups(Group a, Group b, Type similarityMethodType)
        {
            int combinations = 0;
            double accumulator = 0;

            foreach (Article articleInA in a.Articles)
            {
                foreach (Article articleInB in b.Articles)
                {
                    VectorSimilarity similarityMethod = (VectorSimilarity)Activator.CreateInstance(similarityMethodType);
                    accumulator += similarityMethod.GetSimilarity(articleInA, articleInB);
                    combinations++;
                }
            }

            return accumulator / combinations;
        }

        private static List<double> CompareArticles<T>(int articleId, List<Group> groups) where T: VectorSimilarity
        {
            List<Article> articles = new List<Article>(groups.SelectMany(g => g.Articles));
            Article pivot = articles.Where(a => a.Id == articleId).Single();
            List<double> similarities = new List<double>(articles.Count);

            foreach (Article article in articles)
            {
                VectorSimilarity vs = Activator.CreateInstance<T>();
                similarities.Add(vs.GetSimilarity(article, pivot));
            }

            return similarities;
        }

        private static List<Group> GetProjectedVectors(int d, List<Group> groups, Func<double> getRandomNumber)
        {
            List<Group> projectedVectors = new List<Group>(groups.Count);
            int k = groups.SelectMany(g => g.Articles).SelectMany(a => a.Vector).Max(kvp => kvp.Key); //max word Id
            double[,] M = CreateRandomMatrix(k, d, getRandomNumber);
            foreach (Group group in groups)
            {
                Group projectedGroup = new Group
                {
                    Id = group.Id,
                    Name = group.Name
                };

                foreach (Article article in group.Articles)
                {
                    Article projectedArticle = new Article
                    {
                        Id = article.Id,
                        Vector = SparceMatrixVectorProduct(M, article.Vector)
                    };

                    projectedGroup.Articles.Add(projectedArticle);
                }

                projectedVectors.Add(projectedGroup);
            };

            return projectedVectors;
        }

        private static double[,] GetNearestNeighborMatrix<T>(List<Group> groups) where T: VectorSimilarity
        {
            double[,] nearestNeighborGroupMatrix = new double[groups.Count, groups.Count];
            foreach (Group groupA in groups)
            {
                foreach (Article articleA in groupA.Articles)
                {
                    Article nearestNeighbor = null;
                    Group nearestNeighborGroup = null;
                    double maxSimilarity = double.MinValue;
                    foreach (Group groupB in groups.Where(g => g.Id != groupA.Id))
                    {
                        foreach (Article articleB in groupB.Articles)
                        {
                            VectorSimilarity vs = Activator.CreateInstance<T>();
                            double similarity = vs.GetSimilarity(articleA, articleB);
                            if (similarity > maxSimilarity)
                            {
                                maxSimilarity = similarity;
                                nearestNeighbor = articleB;
                                nearestNeighborGroup = groupB;
                            }
                        }
                    }

                    articleA.NearestNeighbor = nearestNeighbor;
                    nearestNeighborGroupMatrix[groupA.Id - 1, nearestNeighborGroup.Id - 1] += 1;
                }
            }

            return nearestNeighborGroupMatrix;
        }

        private static Dictionary<int, double> SparceMatrixVectorProduct(double[,] M, Dictionary<int, double> v)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();

            for (int r = 0; r < M.GetLength(0); r++)
            {
                double accumulator = 0;
                foreach (KeyValuePair<int, double> kvp in v)
                {
                    int wordId = kvp.Key;
                    double count = kvp.Value;

                    accumulator += count * M[r, wordId - 1];
                }

                if (accumulator > 0)
                {
                    result[r] = accumulator;
                }
            }

            return result;
        }

        private static double[,] CreateRandomMatrix(int k, int d, Func<double> getRandomNumber)
        {
            double[,] M = new double[d, k];
            for (int i = 0; i < d; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    M[i, j] = getRandomNumber();
                }
            }

            return M;
        }

        private static double RandomGaussianNumber()
        {
            int mean = 0;
            int stdDev = 1;
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        private static double RandomSignedNumber()
        {
            Random rand = new Random();
            double d = rand.NextDouble(); //these are uniform(0,1) random doubles
            return d > 0.5 ? 1 : -1;
        }

        private static void PrintOnHeatMap(double[,] matrix, List<Group> groups, string algorithm)
        {
            string template = _heatMapTemplate.Replace(_categoriesTemplateString, JsonConvert.SerializeObject(groups.Select(g => g.Name)));
            template = template.Replace(_algorithmTemplateString, algorithm);
            template = template.Replace(_dataTemplateString, JsonConvert.SerializeObject(CreateHeatMapStructure(matrix)));

            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, template);

            Process.Start("chrome.exe", fileName);
        }

        private static double[,] CreateHeatMapStructure(double[,] matrix)
        {
            double[,] structure = new double[matrix.Length, 3];
            int n = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    structure[n, 0] = i;
                    structure[n, 1] = j;
                    structure[n, 2] = matrix[i, j];
                    n++;
                }
            }

            return structure;
        }

        private static void PrintOnScatterPlot(int dimensions, string chartTitle, List<double> x, List<double> y)
        {
            string scatterPlotTemplate = _scatterPlotTemplate.Replace(_chartTitleTemplateString, chartTitle);
            scatterPlotTemplate = scatterPlotTemplate.Replace(_dimensionsTemplateString, dimensions.ToString());
            scatterPlotTemplate = scatterPlotTemplate.Replace(_dataTemplateString, JsonConvert.SerializeObject(CreateScatterPlotStructure(x, y)));

            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, scatterPlotTemplate);

            Process.Start("chrome.exe", fileName);
        }

        private static double[,] CreateScatterPlotStructure(List<double> x, List<double> y)
        {
            double[,] structure = new double[x.Count, 2];
            for (int i = 0; i < x.Count; i++)
            {
                structure[i, 0] = x[i];
                structure[i, 1] = y[i];
            }

            return structure;
        }
        #endregion PrivateMethods
    }
}
