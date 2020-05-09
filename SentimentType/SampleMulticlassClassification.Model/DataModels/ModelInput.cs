//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace Sentiment.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("ID"), LoadColumn(0)]
        public float ID { get; set; }


        [ColumnName("SentimentType"), LoadColumn(1)]
        public string SentimentType { get; set; }


        [ColumnName("Statement"), LoadColumn(2)]
        public string Statement { get; set; }


    }
}
