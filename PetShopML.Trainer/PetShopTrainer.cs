using Microsoft.ML;
using Microsoft.ML.Data;
using PetShopML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ML.TrainCatalogBase;

namespace PetShopML.Trainer
{
    public class PetShopTrainer
    {
        private MLContext _mlContext;
        private bool _overwrite = false;
        public MLContext ModelContext { get { return _mlContext; } }

        public PetShopTrainer() 
        {
            _mlContext = new MLContext(seed: 1);
        }


        public void Init( int version=1, bool overwrite = false) 
        {
            _overwrite = overwrite;
        }

        public bool Train(string dataPath, string outputModelPath) 
        {
            try 
            {
                var trainingDataView = _mlContext.Data.LoadFromTextFile<SaleData>(dataPath, hasHeader: true, separatorChar: ',');

                var trainer = _mlContext.Regression.Trainers.FastTreeTweedie(labelColumnName: "Label", featureColumnName: "Features");

                var trainingPipeline = _mlContext.Transforms.Concatenate(outputColumnName: "NumFeatures", nameof(SaleData.year), nameof(SaleData.month),
                        nameof(SaleData.units), nameof(SaleData.avg), nameof(SaleData.count),
                        nameof(SaleData.max), nameof(SaleData.min), nameof(SaleData.prev))
                  .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CatFeatures", inputColumnName: nameof(SaleData.productId)))
                  .Append(_mlContext.Transforms.Concatenate(outputColumnName: "Features", "NumFeatures", "CatFeatures"))
                  .Append(_mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(SaleData.next)))
                  .Append(trainer);

                var crossValidationResults = _mlContext.Regression.CrossValidate(data: trainingDataView,
                    estimator: trainingPipeline, numberOfFolds: 6, labelColumnName: "Label");

                PrintRegressionFoldsAverageMetrics(trainer.ToString() ?? "Empty", crossValidationResults);

                // Train the model.
                var model = trainingPipeline.Fit(trainingDataView);

                if (File.Exists(outputModelPath))
                {
                    if (!_overwrite)
                    {
                        throw new InvalidDataException("Model already exists");
                    }
                    else
                    {
                        File.Delete(outputModelPath);
                    }
                }

                // Save the model for later comsumption
                _mlContext.Model.Save(model, trainingDataView.Schema, outputModelPath);
                
                Console.WriteLine("Model created");
            } 
            catch (Exception e) 
            {
                Console.WriteLine($"Error: {e.Message}");
                return false;
            }
            
            return true;
        }


        public void TestPrediction(string regressionModelPath) 
        {
            ITransformer trainedModel;
            using (var stream = File.OpenRead(regressionModelPath))
            {
                trainedModel = _mlContext.Model.Load(stream, out var modelInputSchema);
            }

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SaleData, SalesPrediction>(trainedModel);
            Console.WriteLine("** Testing Product **");

            // Predict the nextperiod/month forecast to the one provided
            SalesPrediction prediction = predictionEngine.Predict(SampleSaleData.MonthlyData[0]);
            Console.WriteLine($"Product: {SampleSaleData.MonthlyData[0].productId}, month: {SampleSaleData.MonthlyData[0].month + 1}, year: {SampleSaleData.MonthlyData[0].year} - Real value (units): {SampleSaleData.MonthlyData[0].next}, Forecast Prediction (units): {prediction.Score}");

            // Predicts the nextperiod/month forecast to the one provided
            prediction = predictionEngine.Predict(SampleSaleData.MonthlyData[1]);
            Console.WriteLine($"Product: {SampleSaleData.MonthlyData[1].productId}, month: {SampleSaleData.MonthlyData[1].month + 1}, year: {SampleSaleData.MonthlyData[1].year} - Forecast Prediction (units): {prediction.Score}");

        }


        public static void PrintRegressionFoldsAverageMetrics(string algorithmName, IReadOnlyList<CrossValidationResult<RegressionMetrics>> crossValidationResults)
        {
            var L1 = crossValidationResults.Select(r => r.Metrics.MeanAbsoluteError);
            var L2 = crossValidationResults.Select(r => r.Metrics.MeanSquaredError);
            var RMS = crossValidationResults.Select(r => r.Metrics.RootMeanSquaredError);
            var lossFunction = crossValidationResults.Select(r => r.Metrics.LossFunction);
            var R2 = crossValidationResults.Select(r => r.Metrics.RSquared);

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for {algorithmName} Regression model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average L1 Loss:    {L1.Average():0.###} ");
            Console.WriteLine($"*       Average L2 Loss:    {L2.Average():0.###}  ");
            Console.WriteLine($"*       Average RMS:          {RMS.Average():0.###}  ");
            Console.WriteLine($"*       Average Loss Function: {lossFunction.Average():0.###}  ");
            Console.WriteLine($"*       Average R-squared: {R2.Average():0.###}  ");
            Console.WriteLine($"*************************************************************************************************************");
        }
    }
}
