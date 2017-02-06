using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HW4
{
    class Program
    {

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
                text: 'Square error'
            },
            startOnTick: true,
            endOnTick: true,
            showLastLabel: true
        },
        yAxis: {
            title: {
                text: 'Iterations'
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
        series: [{{Series}}]
    });
});
</script>
</body>
</html>";
        #endregion ScatterPlotTemplate

        #region SeriesTemplate
        private static string _seriesTemplate = @"{
            name: '{{SeriesName}}',
            color: 'rgba({{Red}}, {{Blue}}, {{Green}}, .5)',
            data: {{Data}}
        }";
        #endregion SeriesTemplate

        static string _dataTemplateString = "{{Data}}";
        static string _chartTitleTemplateString = "{{ChartTitle}}";
        static string _seriesNameTemplateString = "{{SeriesName}}";
        static string _seriesTemplateString = "{{Series}}";
        static string _seriesRedColorTemplateString = "{{Red}}";
        static string _seriesBlueColorTemplateString = "{{Blue}}";
        static string _seriesGreenColorTemplateString = "{{Green}}";

        static void Main(string[] args)
        {
            // Part 1
            MatrixHelper matrixHelper = new MatrixHelper();
            int n = 100;
            int m = 1000;
            double[,] X = matrixHelper.GenerateRandomMatrix(m, n, 0, 1);
            double[,] a_true = matrixHelper.GenerateRandomMatrix(n, 1, 0, 1);
            double[,] y = matrixHelper.DotProduct(X, a_true);
            y = matrixHelper.MatrixAddition(y, matrixHelper.GenerateRandomMatrix(m, 1, 0, 0.1));

            // Part 1.a)
            Console.WriteLine("Printing part 1.a");
            double[,] optimalA = new ClosedLeastSquareError().GetOptimalA(X, y);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(optimalA)} from exact method"));
            double optimalAError = SquareError.GetSquareError(X, y, optimalA);
            Console.WriteLine($"Minimum square error with exact formula is: {optimalAError}");

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.b)
            Console.WriteLine("Printing part 1.b");
            double[] stepSizes = new[] { 0.0001, 0.001, 0.00125, 0.001001, 0.00101, 0.00095 };
            int iterations = 20;
            ConcurrentDictionary<string, GDLeastSquareError> gdlses = new ConcurrentDictionary<string, GDLeastSquareError>();
            Parallel.For(0, stepSizes.Length, i =>
            {
                double stepSize = stepSizes[i];
                GDLeastSquareError gdlse = new GDLeastSquareError(stepSize, iterations);
                string name = $"Step size {stepSize}";
                gdlses.GetOrAdd(name, (key) => gdlse);
                double[,] gdA = gdlse.GetOptimalA(X, y);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareError(X, y, gdA);
                Console.WriteLine($"Minimum square error for stepsize {stepSize} after {iterations} iterations is: {gdAError}");
            });

            PrintOnScatterPlot("Standard Gradient Descent", gdlses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IterationResult));

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.c)
            Console.WriteLine("Printing part 1.c");
            stepSizes = new[] { 0.001, 0.01, 0.02 };
            iterations = 1000;
            ConcurrentDictionary<string, SGDLeastSquareError> sgdlses = new ConcurrentDictionary<string, SGDLeastSquareError>();
            Parallel.For(0, stepSizes.Length, i =>
            {
                double stepSize = stepSizes[i];
                SGDLeastSquareError sgdlse = new SGDLeastSquareError(stepSize, iterations);
                string name = $"Step size {stepSize}";
                sgdlses.GetOrAdd(name, (key) => sgdlse);
                double[,] gdA = sgdlse.GetOptimalA(X, y);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareError(X, y, gdA);
                Console.WriteLine($"Minimum square error for stepsize {stepSize} after {iterations} iterations is: {gdAError}");
            });

            PrintOnScatterPlot("Sthocastic Gradient Descent", sgdlses.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IterationResult));

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.d)
            Console.WriteLine("Printing part 1.d");
            int train_m = 100;
            int test_m = 1000;
            n = 100;
            double[,] X_train = matrixHelper.GenerateRandomMatrix(train_m, n, 0, 1);
            a_true = matrixHelper.GenerateRandomMatrix(n, 1, 0, 1);
            double[,] y_train = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_train, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(train_m, 1, 0, 1), 0.5));
            double[,] X_test = matrixHelper.GenerateRandomMatrix(test_m, n, 0, 1);
            double[,] y_test = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_test, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(test_m, 1, 0, 1), 0.5));

            double step = 0.0095;
            iterations = 20;
            double[,] gdAVector = new GDLeastSquareError(step, iterations).GetOptimalA(X_train, y_train);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            double gdAErrorResult = SquareError.GetSquareError(X_train, y_train, gdAVector);
            Console.WriteLine($"Minimum square error in train data (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_test, y_test, gdAVector);
            Console.WriteLine($"Minimum square error in test data  (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            train_m = 20;
            double[,] X_train_20 = matrixHelper.GenerateRandomMatrix(train_m, n, 0, 1);
            double[,] y_train_20 = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_train_20, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(train_m, 1, 0, 1), 0.5));

            gdAVector = new GDLeastSquareError(step, iterations).GetOptimalA(X_train_20, y_train_20);
            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_train_20, y_train_20, gdAVector);
            Console.WriteLine($"Minimum square error in train data (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
            gdAErrorResult = SquareError.GetSquareError(X_test, y_test, gdAVector);
            Console.WriteLine($"Minimum square error in test data  (train data size {train_m}) for stepsize {step} after {iterations} iterations is: {gdAErrorResult}");

            Console.WriteLine();
            Console.WriteLine("===========================================");
            Console.WriteLine();

            // Part 1.e)
            Console.WriteLine("Printing part 1.e");
            double[] lambdas = new[] { 100, 10, 1, 0.1, 0.01, 0.001 };
            train_m = 50;
            double[,] X_train_50 = matrixHelper.GenerateRandomMatrix(train_m, n, 0, 1);
            double[,] y_train_50 = matrixHelper.MatrixAddition(matrixHelper.DotProduct(X_train_50, a_true), matrixHelper.DotProduct(matrixHelper.GenerateRandomMatrix(train_m, 1, 0, 1), 0.5));
            Parallel.For(0, lambdas.Length, i =>
            {
                double lambda = lambdas[i];
                int trainSize = 100;
                double[,] gdA = new GDL2LeastSquareError(step, iterations, lambda).GetOptimalA(X_train, y_train);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                double gdAError = SquareError.GetSquareErrorL2(X_train, y_train, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in train data (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                gdAError = SquareError.GetSquareErrorL2(X_test, y_test, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in test data  (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

                trainSize = 20;
                gdA = new GDL2LeastSquareError(step, iterations, lambda).GetOptimalA(X_train_20, y_train_20);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                gdAError = SquareError.GetSquareErrorL2(X_train_20, y_train_20, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in train data (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                gdAError = SquareError.GetSquareErrorL2(X_test, y_test, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in test data  (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

                trainSize = 50;
                gdA = new GDL2LeastSquareError(step, iterations, lambda).GetOptimalA(X_train_50, y_train_50);
                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                gdAError = SquareError.GetSquareErrorL2(X_train_50, y_train_50, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in train data (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

                //Task.Run(() => Console.WriteLine($"Using A {JsonConvert.SerializeObject(gdA)} from {stepSize} step GD"));
                gdAError = SquareError.GetSquareErrorL2(X_test, y_test, gdA, lambda);
                Console.WriteLine($"Minimum L2 (lambda: {lambda}) square error in test data  (train data size {trainSize}) for stepsize {step} after {iterations} iterations is: {gdAError}");

            });

            // End
            Console.ReadKey();
        }

        private static void PrintOnScatterPlot(string chartTitle, Dictionary<string, List<double>> results)
        {
            Random random = new Random();
            string scatterPlotTemplate = _scatterPlotTemplate.Replace(_chartTitleTemplateString, chartTitle);
            List<string> series = new List<string>();
            foreach (KeyValuePair<string, List<double>> kvp in results)
            {
                string name = kvp.Key;
                List<double> result = kvp.Value;
                string seriesTemplate = _seriesTemplate.Replace(_seriesRedColorTemplateString, random.Next(0, 256).ToString());
                seriesTemplate = seriesTemplate.Replace(_seriesBlueColorTemplateString, random.Next(0, 256).ToString());
                seriesTemplate = seriesTemplate.Replace(_seriesGreenColorTemplateString, random.Next(0, 256).ToString());
                seriesTemplate = seriesTemplate.Replace(_dataTemplateString, JsonConvert.SerializeObject(CreateScatterPlotStructure(result)));
                seriesTemplate = seriesTemplate.Replace(_seriesNameTemplateString, name);
                series.Add(seriesTemplate);
            }
            scatterPlotTemplate = scatterPlotTemplate.Replace(_seriesTemplateString, string.Join(",", series));

            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, scatterPlotTemplate);

            Process.Start("chrome.exe", fileName);
        }

        private static double[,] CreateScatterPlotStructure(List<double> y)
        {
            double[,] structure = new double[y.Count, 2];
            for (int i = 0; i < y.Count; i++)
            {
                structure[i, 0] = i;
                structure[i, 1] = y[i];
            }

            return structure;
        }
    }
}
