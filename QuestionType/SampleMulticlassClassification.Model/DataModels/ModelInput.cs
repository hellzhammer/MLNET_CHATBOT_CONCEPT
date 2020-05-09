//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace Question.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("ID"), LoadColumn(0)]
        public float ID { get; set; }


        [ColumnName("Statement"), LoadColumn(1)]
        public string Statement { get; set; }


        [ColumnName("QuestionType"), LoadColumn(2)]
        public string QuestionType { get; set; }


    }
}
