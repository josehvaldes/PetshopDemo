// See https://aka.ms/new-console-template for more information
using PetShopML.Model;
using PetShopML.Trainer;

Console.WriteLine($"Petshop Sales Prediction Trainer");
int version = 1;

string projectPath = "c:\\personal\\BallastLane\\PetShopDemo";
string dataPath = $"{projectPath}\\MLModel\\product_12_months_data.csv";

string publishPath = $"{projectPath}\\MLModel";
string regressionModelPath = $"{publishPath}\\Regression_Sales_Model_v{version}.zip";

string product988TimeSeriesModelPath = $"{publishPath}\\product988_month_timeSeriesSSA.zip";

if (args.Length>0) 
{

    if (args[0] == "\\train")
    {
        var trainer = new PetShopTrainer();
        trainer.Init(version, true);
        var completed = trainer.Train(dataPath, regressionModelPath);

    }
    else if (args[0] == "\\test")
    {
        var trainer = new PetShopTrainer();

        trainer.TestPrediction(regressionModelPath);

        var seriesTrainer = new TimeSeriesModelGenerator(regressionModelPath, publishPath);

        float productId = 988;
        var productModelPath = seriesTrainer.PerformTimeSeriesProductForecasting(productId, dataPath);
        Console.WriteLine($"Product Model Path: {productModelPath}");
    }
    else if (args[0] == "\\tsTest")
    {
        var tester = new TimeSeriesModelTester(regressionModelPath, product988TimeSeriesModelPath);
        SaleData data = new SaleData()
        {
            avg = 41,
            count = 26,
            max = 225,
            min = 4,
            month = 11,
            next = float.NaN,
            prev = 1094,
            productId = 988,
            units = 1076,
            year = 2017
        };
        tester.TestPrediction(data, product988TimeSeriesModelPath);
    }
    else 
    {
        Console.WriteLine("Unknow argument");
    }

}

