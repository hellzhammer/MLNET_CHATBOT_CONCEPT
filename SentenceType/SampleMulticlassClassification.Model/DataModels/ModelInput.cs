//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace Sentence.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("ID"), LoadColumn(0)]
        public float ID { get; set; }


        [ColumnName("Statement"), LoadColumn(1)]
        public string Statement { get; set; }


        [ColumnName("sentenceType"), LoadColumn(2)]
        public string SentenceType { get; set; }


    }
}
