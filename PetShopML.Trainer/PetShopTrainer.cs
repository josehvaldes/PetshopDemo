using Microsoft.ML;
using Microsoft.ML.Data;
using PetShopML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShopML.Trainer
{
    public class PetShopTrainer
    {
        private string _dataPath;

        public PetShopTrainer() 
        {

            _dataPath = "";

        }


        public void Init() 
        {
            string BaseDatasetsRelativePath = @"./Data";
            string DataRelativePath = $"{BaseDatasetsRelativePath}/PetshopData.txt";
            _dataPath = GetAbsolutePath(DataRelativePath);

        }

        public bool Train() 
        {
            var mlContext = new MLContext(seed: 1);

            //IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(TrainingDataLocation, hasHeader: true, ar: ',');

            return true;
        }


        public void Publish() 
        {
        
        }

        private string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory?.FullName ?? @"C:\";

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }


    }
}
