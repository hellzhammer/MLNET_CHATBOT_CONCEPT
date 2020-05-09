using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using QuestionStatement.Model.DataModels;

namespace KronusLogicProcessor.QuestionStatementLogic
{
    public class QuestionStatementProcessor
    {
        //Machine Learning model to load and use for predictions
        private string MODEL_FILEPATH = "/home/matt/Projects/KronusLogicProcessor/QuestionOrStatement/SampleBinaryClassification.Model/MLModel.zip";

        //Dataset to use for predictions 
        private string DATA_FILEPATH = Environment.CurrentDirectory + "/Data/QuestionStatementData.tsv";
        MLContext mlContext { get; set; }
        public void ClassifyMain()
        {
            mlContext = new MLContext();

            // Training code used by ML.NET CLI and AutoML to generate the model
            //ModelBuilder.CreateModel();

            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);
            GetPrediction(sampleData);
            Console.WriteLine("=============== End of process===============");
        }

        public bool GetPrediction(ModelInput sampleData)
        {
            ITransformer mlModel = mlContext.Model.Load(MODEL_FILEPATH, out DataViewSchema inputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
            // Try a single prediction
            ModelOutput predictionResult = predEngine.Predict(sampleData);
            Console.WriteLine($"Single Prediction --> Actual value: {sampleData.StatementType} | Predicted value: {predictionResult.Prediction} | Predicted scores: [{String.Join(",", predictionResult.Score)}]");
            return predictionResult.Prediction;
        }

        // Method to load single row of data to try a single prediction
        // You can change this code and create your own sample data here (Hardcoded or from any source)
        private ModelInput CreateSingleDataSample(string dataFilePath)
        {
            // Read dataset to get a single row for trying a prediction          
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: dataFilePath,
                                            hasHeader: true,
                                            separatorChar: '\t',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Here (ModelInput object) you could provide new test data, hardcoded or from the end-user application, instead of the row from the file.
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                        .First();
            return sampleForPrediction;
        }

        public string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
