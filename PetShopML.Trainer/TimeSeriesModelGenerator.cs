using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using PetShopML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShopML.Trainer
{
    public class TimeSeriesModelGenerator
    {
        private string _outputPath;
        private MLContext _mlContext;
        private ITransformer _trainedModel;

        public TimeSeriesModelGenerator( string regressionModelPath, string outputPath) 
        {
            _outputPath = outputPath;
            _mlContext = new MLContext(seed: 1);

            using (var stream = File.OpenRead(regressionModelPath))
            {
                _trainedModel = _mlContext.Model.Load(stream, out var modelInputSchema);
            }
        }

        public string PerformTimeSeriesProductForecasting(float productId, string dataPath ) 
        {
            var productModelName = $"product{productId}_month_timeSeriesSSA.zip";
            var productModelPath = $"{_outputPath}{Path.DirectorySeparatorChar}{productModelName}";

            if (File.Exists(productModelPath))
            {
                File.Delete(productModelPath);
            }

            IDataView SaleDataView = LoadData(_mlContext, productId, dataPath);
            var singleSaleDataSeries = _mlContext.Data.CreateEnumerable<SaleData>(SaleDataView, false).OrderBy(p => p.year).ThenBy(p => p.month);
            SaleData lastMonthSaleData = singleSaleDataSeries.Last();

            FitAndSaveModel(_mlContext, SaleDataView, productModelPath);
            TestPrediction(_mlContext, lastMonthSaleData, productModelPath);

            return productModelPath;
        }

        private IDataView LoadData(MLContext mlContext, float productId, string dataPath)
        {
            // Load the data series for the specific product that will be used for forecasting sales.
            IDataView allProductsDataView = mlContext.Data.LoadFromTextFile<SaleData>(dataPath, hasHeader: true, separatorChar: ',');
            IDataView SaleDataView = mlContext.Data.FilterRowsByColumn(allProductsDataView, nameof(SaleData.productId), productId, productId + 1);

            return SaleDataView;
        }

        private static void FitAndSaveModel(MLContext mlContext, IDataView SaleDataSeries, string outputModelPath)
        {
            Console.WriteLine("Fitting product forecasting Time Series model");

            //update to 12 since not enough data was collected
            const int numSeriesDataPoints = 12; //The underlying data has a total of 12 months worth of data for each product

            // Create and add the forecast estimator to the pipeline.
            IEstimator<ITransformer> forecastEstimator = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(ProductUnitTimeSeriesPrediction.ForecastedProductUnits),
                inputColumnName: nameof(SaleData.units), // This is the column being forecasted.
                windowSize: 5, // Window size is set to the time period represented in the product data cycle; our product cycle is based on 5 months, so this is set to a factor of 5, e.g. 3.
                seriesLength: numSeriesDataPoints, // This parameter specifies the number of data points that are used when performing a forecast.
                trainSize: numSeriesDataPoints, // This parameter specifies the total number of data points in the input time series, starting from the beginning.
                horizon: 2, // Indicates the number of values to forecast; 2 indicates that the next 2 months of product units will be forecasted.
                confidenceLevel: 0.95f, // Indicates the likelihood the real observed value will fall within the specified interval bounds.
                confidenceLowerBoundColumn: nameof(ProductUnitTimeSeriesPrediction.ConfidenceLowerBound), //This is the name of the column that will be used to store the lower interval bound for each forecasted value.
                confidenceUpperBoundColumn: nameof(ProductUnitTimeSeriesPrediction.ConfidenceUpperBound)); //This is the name of the column that will be used to store the upper interval bound for each forecasted value.

            // Fit the forecasting model to the specified product's data series.
            ITransformer forecastTransformer = forecastEstimator.Fit(SaleDataSeries);

            // Create the forecast engine used for creating predictions.
            TimeSeriesPredictionEngine<SaleData, ProductUnitTimeSeriesPrediction> forecastEngine = forecastTransformer.CreateTimeSeriesEngine<SaleData, ProductUnitTimeSeriesPrediction>(mlContext);

            // Save the forecasting model so that it can be loaded within an end-user app.
            forecastEngine.CheckPoint(mlContext, outputModelPath);
        }

        public void TestPrediction(MLContext mlContext, SaleData lastMonthSaleData, string outputModelPath)
        {
            Console.WriteLine("Testing product unit sales forecast Time Series model");

            // Load the forecast engine that has been previously saved.
            ITransformer forecaster;
            using (var file = File.OpenRead(outputModelPath))
            {
                forecaster = mlContext.Model.Load(file, out DataViewSchema schema);
            }

            // We must create a new prediction engine from the persisted model.
            TimeSeriesPredictionEngine<SaleData, ProductUnitTimeSeriesPrediction> forecastEngine = forecaster.CreateTimeSeriesEngine<SaleData, ProductUnitTimeSeriesPrediction>(mlContext);

            // Get the prediction; this will include the forecasted product units sold for the next 2 months since this the time period specified in the `horizon` parameter when the forecast estimator was originally created.
            Console.WriteLine("\n** Original prediction **");
            ProductUnitTimeSeriesPrediction originalSalesPrediction = forecastEngine.Predict();

            // Compare the units of the first forecasted month to the actual units sold for the next month.
            var predictionMonth = lastMonthSaleData.month == 12 ? 1 : lastMonthSaleData.month + 1;
            var predictionYear = predictionMonth < lastMonthSaleData.month ? lastMonthSaleData.year + 1 : lastMonthSaleData.year;
            Console.WriteLine($"Product: {lastMonthSaleData.productId}, Month: {predictionMonth}, Year: {predictionYear} " +
                $"- Real Value (units): {lastMonthSaleData.next}, Forecast Prediction (units): {originalSalesPrediction.ForecastedProductUnits[0]}");

            // Get the first forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[0]} - {originalSalesPrediction.ConfidenceUpperBound[0]}]\n");

            //
            // Get the units of the second forecasted month.
            predictionMonth = lastMonthSaleData.month >= 11 ? (lastMonthSaleData.month + 2) % 12 : lastMonthSaleData.month + 2;
            predictionYear = predictionMonth < lastMonthSaleData.month ? lastMonthSaleData.year + 1 : lastMonthSaleData.year;
            Console.WriteLine($"Product: {lastMonthSaleData.productId}, Month: {predictionMonth}, Year: {predictionYear}, " +
                $"Forecast (units): {originalSalesPrediction.ForecastedProductUnits[1]}");
            // Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[1]} - {originalSalesPrediction.ConfidenceUpperBound[1]}]\n");


            //NOTE: remove prediction based on new default data
            //Console.WriteLine("** Updated prediction **");
            //ProductUnitTimeSeriesPrediction updatedSalesPrediction = forecastEngine.Predict(SampleSaleData.MonthlyData[1], horizon: 1);
            // Save a checkpoint of the forecasting model.
            //forecastEngine.CheckPoint(mlContext, outputModelPath);

            // Get the units of the updated forecast.
            //predictionMonth = lastMonthSaleData.month >= 11 ? (lastMonthSaleData.month + 2) % 12 : lastMonthSaleData.month + 2;
            //predictionYear = predictionMonth < lastMonthSaleData.month ? lastMonthSaleData.year + 1 : lastMonthSaleData.year;
            //Console.WriteLine($"Product: {lastMonthSaleData.productId}, Month: {predictionMonth}, Year: {predictionYear}, " +
            //    $"Forecast (units): {updatedSalesPrediction.ForecastedProductUnits[0]}");

            //// Get the updated forecast's confidence interval bounds.
            //Console.WriteLine($"Confidence interval: [{updatedSalesPrediction.ConfidenceLowerBound[0]} - {updatedSalesPrediction.ConfidenceUpperBound[0]}]\n");
        }

    }
}
