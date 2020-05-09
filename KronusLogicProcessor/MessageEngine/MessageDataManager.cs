using System;
using System.Collections.Generic;
using KronusLogicProcessor.Greeting.DisjuctiveGreetingType;
using KronusLogicProcessor.Greeting.GeneralGreetingType;
using KronusLogicProcessor.Greeting.GreetingOrStatement;
using KronusLogicProcessor.Greeting.GreetingType;
using KronusLogicProcessor.QuestionStatementLogic;
using KronusLogicProcessor.QuestionTypeLogic;
using KronusLogicProcessor.SentenceTypeLogic;
using KronusLogicProcessor.SentimentTypeLogic;
using PerleyMLMQTTlib.Models;
using PerleyMLMQTTlib.NeuralNetworking;

namespace KronusLogicProcessor.MessageEngine
{
    public class MessageDataManager
    {
        #region Classifiers
        private QuestionStatementProcessor qsp = new QuestionStatementProcessor();
        private QuestionTypeProcessor qtp = new QuestionTypeProcessor();
        private SentenceTypeProcessor stp = new SentenceTypeProcessor();
        private SentimentTypeProcessor senttp = new SentimentTypeProcessor();
        private GreetingStatementClassifier gsp = new GreetingStatementClassifier();
        private GreetingTypeClassifier gtp = new GreetingTypeClassifier();
        private GeneralGreetingLogic ggtp = new GeneralGreetingLogic();
        private DisjuctiveGreetingLogic dgtp = new DisjuctiveGreetingLogic();
        #endregion

        public MessageDataManager()
        {
            //prep classifiers
            Console.WriteLine("Prep Classifiers");
            qsp.ClassifyMain();
            qtp.QuestionTypeMain();
            stp.SentenceTypeMain();
            senttp.SentimentMain();
            gsp.GreetingMain();
            gtp.GreetingTypeMain();
            ggtp.GeneralGreetingMain();
            dgtp.DisjuctiveGreetingMain();
            Console.WriteLine("Done Prepping Classifiers");
        }

        public string Messenger(PerleyMLDataModel perleyData)
        {
            Console.WriteLine("\nBuilding Message ++++++++++++++");
            string msg = string.Empty;
            if (perleyData.Greeting)
            {
                string pred = perleyData.GreetingSubType;
                if (perleyData.GreetingType == "NotMuchGreeting")
                {
                    pred = "NotMuchGreeting";
                }
                else if(perleyData.GreetingType == "NotBadGreeting")
                {
                    pred = "NotBadGreeting";
                }

                switch (pred)
                {
                    case "WhatsUp":
                        msg = "Not much, and you?";
                        break;
                    case "HowAreYou":
                        msg = "Pretty good, and you?";
                        break;
                    case "HangOut":
                        msg = "I am sorry, I dont have legs.";
                        break;
                    case "Hey":
                        msg = "Hey, how are you?";
                        break;
                    case "NotBadGreeting":
                        msg = "Not bad.";
                        break;
                    case "NotMuchGreeting":
                        msg = "Not much.";
                        break;
                    case "None":
                        msg = "Goodbye";
                        break;
                }
                Console.WriteLine("Message Built: " + msg);
            }
            Console.WriteLine("\nDone Building Message++++++++++++++");
            return msg;
        }

        public double[] BuildNNDataModel(PerleyMLDataModel data)
        {
            double[] currentData = null;

            try
            {
                if (data.Greeting)
                {
                    currentData = new double[] { 1,
                    NeuralNetDataDictionary.GreetingTypes[data.GreetingType],
                        NeuralNetDataDictionary.GreetingSubTypes[data.GreetingSubType]
                         };
                }
                else
                {
                    currentData = new double[] { 0,
                    NeuralNetDataDictionary.GreetingTypes[data.GreetingType],
                        NeuralNetDataDictionary.GreetingSubTypes[data.GreetingSubType]
                         };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            return currentData;
        }

        double[,] AddColumn(double[,] original, double[] added)
        {
            int lastRow = original.GetUpperBound(0);
            int lastColumn = original.GetUpperBound(1);
            // Create new array.
            double[,] result = new double[lastRow + 1, lastColumn + 2];
            // Copy the array.
            for (int i = 0; i <= lastRow; i++)
            {
                for (int x = 0; x <= lastColumn; x++)
                {
                    result[i, x] = original[i, x];
                }
            }
            // Add the new column.
            for (int i = 0; i < added.Length; i++)
            {
                result[i, lastColumn + 1] = added[i];
            }
            return result;
        }

        public PerleyMLDataModel RunPredictionEngine((string, string) data)
        {
            Console.WriteLine("Classifying");
            string statementType = "None";
            string questionType = "None";

            bool greeting = gsp.GetPrediction(new GreetingStatement.Model.DataModels.ModelInput
            {
                ID = new Random().Next(),
                Statement = data.Item2,
                Greeting = false
            });

            string greetingType = "None";
            if (greeting)
            {
                //get greeting type
                greetingType = gtp.GetPrediction(new GreetingType.Model.DataModels.ModelInput
                {
                    ID = new Random().Next(),
                    Statement = data.Item2,
                    GreetingType = "unknown"
                });
            }
            string GreetingSubType = "N/A";
            switch (greetingType)
            {
                case "DisjunctiveGreeting":
                    GreetingSubType = dgtp.GetPrediction(new DisjunctiveGreetingType.Model.DataModels.ModelInput
                    {
                        ID = 1234,
                        Statement = data.Item2,
                        GreetingType = "unknown"
                    });
                    break;
                case "GeneralQuestionGreeting":
                    GreetingSubType = ggtp.GetPrediction(new GeneralGreetingType.Model.DataModels.ModelInput
                    {
                        ID = 1234,
                        Statement = data.Item2,
                        GreetingType = "unknown"
                    });
                    break;
                case "StatementGreeting":
                    GreetingSubType = "Hey";
                    break;
                case "NotBadGreeting":
                    GreetingSubType = "None";
                    break;
                case "NotMuchGreeting":
                    GreetingSubType = "None";
                    break;
                case "None":
                    GreetingSubType = "None";
                    break;
            }

            #region TODO Stuff
            //not being used currently....
            bool question = qsp.GetPrediction(
                new QuestionStatement.Model.DataModels.ModelInput
                {
                    Statement = data.Item2,
                    ID = new Random().Next(),
                    StatementType = false
                }
            );

            if (question)
            {
                questionType = qtp.GetPrediction(
                    new Question.Model.DataModels.ModelInput
                    {
                        ID = new Random().Next(),
                        QuestionType = "unknown",
                        Statement = data.Item2
                    }
                );
            }
            else
            {
                statementType = stp.GetPrediction(
                    new Sentence.Model.DataModels.ModelInput
                    {
                        ID = new Random().Next(),
                        SentenceType = "unknown",
                        Statement = data.Item2
                    }
                );
            }
            string sentimentType = senttp.GetPrediction(
                new Sentiment.Model.DataModels.ModelInput
                {
                    ID = new Random().Next(),
                    SentimentType = "unknown",
                    Statement = data.Item2
                }
            );
            #endregion

            Console.WriteLine("Done Classifying");

            Console.WriteLine("Building Data Model");
            PerleyMLDataModel perleyData = new PerleyMLDataModel
            {
                ID = new Random().Next(),
                Greeting = greeting,
                GreetingType = greetingType,
                GreetingSubType = GreetingSubType,
                Message = data.Item2,
                Question = question,
                QuestionType = questionType,
                SentenceType = statementType,
                SentimentType = sentimentType
            };

            #region Console Outputs
            Console.WriteLine("Finished Data Model");

            Console.WriteLine("\n++++++++++++++++++++++++++++++");

            Console.WriteLine("ID: " + perleyData.ID);
            Console.WriteLine("Message: " + perleyData.Message);
            Console.WriteLine("Greeting: " + perleyData.Greeting);
            Console.WriteLine("Greeting Type: " + perleyData.GreetingType);
            Console.WriteLine("Question: " + perleyData.Question);
            Console.WriteLine("Question Type: " + perleyData.QuestionType);
            Console.WriteLine("Sentence Type: " + perleyData.SentenceType);
            Console.WriteLine("Sentiment Type: " + perleyData.SentimentType);

            Console.WriteLine("\n++++++++++++++++++++++++++++++");
            #endregion

            return perleyData;
        }
    }
}
