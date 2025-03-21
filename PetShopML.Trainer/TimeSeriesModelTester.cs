using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.ML;
using PetShopML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ML.ForecastingCatalog;
using System.Globalization;

namespace PetShopML.Trainer
{
    public class TimeSeriesModelTester
    {
        private MLContext _mlContext;
        private ITransformer? _trainedModel;
        private ITransformer _forecaster;
        public TimeSeriesModelTester(string regressionModelPath, string timeSeriesModelPath) 
        {
            _mlContext = new MLContext(seed: 1);
            using (var stream = File.OpenRead(regressionModelPath))
            {
                _trainedModel = _mlContext.Model.Load(stream, out var modelInputSchema);
            }

            using (var file = File.OpenRead(timeSeriesModelPath))
            {
                _forecaster = _mlContext.Model.Load(file, out DataViewSchema schema);
            }
        }

        public void TestPrediction(SaleData lastMonthSaleData, string timeSeriesModelPath)
        {
            Console.WriteLine("Testing product unit sales forecast Time Series model");

            // We must create a new prediction engine from the persisted model.
            TimeSeriesPredictionEngine<SaleData, ProductUnitTimeSeriesPrediction> forecastEngine = 
                _forecaster.CreateTimeSeriesEngine<SaleData, ProductUnitTimeSeriesPrediction>(_mlContext);

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

            // Get the units of the second forecasted month.
            predictionMonth = lastMonthSaleData.month >= 11 ? (lastMonthSaleData.month + 2) % 12 : lastMonthSaleData.month + 2;
            predictionYear = predictionMonth < lastMonthSaleData.month ? lastMonthSaleData.year + 1 : lastMonthSaleData.year;
            Console.WriteLine($"Product: {lastMonthSaleData.productId}, Month: {predictionMonth}, Year: {predictionYear}, " +
                $"Forecast (units): {originalSalesPrediction.ForecastedProductUnits[1]}");

            // Get the second forecasted month's confidence interval bounds.
            Console.WriteLine($"Confidence interval: [{originalSalesPrediction.ConfidenceLowerBound[1]} - {originalSalesPrediction.ConfidenceUpperBound[1]}]\n");

        }
    }
}
