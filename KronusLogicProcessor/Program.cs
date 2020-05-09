using System;
using System.Collections.Generic;
using PerleyMLMQTTlib.Models;
using Newtonsoft.Json;
using PerleyMLMQTTlib.NeuralNetworking;
using PerleyMLMQTTlib.DataEngine;
using KronusLogicProcessor.MessageEngine;
using MathNet.Numerics;

namespace KronusLogicProcessor
{
    public class Program
    {
        private static readonly string nnDataFileName = "/nndata";
        private static readonly string predDataFileName = "/predictiondata";

        #region Mqtt and file managers
        private static KronusNode client { get; set; }
        private static DataEngineManager dataManager { get; set; }
        #endregion

        #region Neural Net
        private static NetworkBuilder nnb { get; set; }
        private static NeuronModel NNB { get; set; }
        #endregion

        private static MessageDataManager MessageData = new MessageDataManager();

        static void Main(string[] args)
        {
            Console.WriteLine("Starting Applet");

            try
            {
                //prep essentials
                client = new KronusNode();
                client.BuildClient("kronusLogic").Wait();
                dataManager = new DataEngineManager();
                //nnb = new NetworkBuilder();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Applet Started");

            //experimental -- test to make sure everything works as expected
            MessageRecieved(("test", "hey"));

            Console.WriteLine("\n Await Messages...");

            //nnb.InitNetwork();

            do
            {

            } while (client.mqttClient.IsConnected);
        }

        //feed forward, inputs >==> outputs.
        public static void MessageRecieved((string, string) data)
        {
            PerleyMLDataModel dataModel = MessageData.RunPredictionEngine(data);
            string msg = MessageData.Messenger(dataModel);
            (double[,], double[,]) nnData = BuildAndStoreDataModels(dataModel, msg);
            //todo build neural net response generator

            Console.WriteLine("Sending message");
            client.SendMessage(msg, client.Subscription + "/" + "kronusLogic"); // /Messenger
            Console.WriteLine("Done sending message");
        }

        private static (double[,], double[,]) BuildAndStoreDataModels(PerleyMLDataModel dataModel, string outputMessage)
        {
            Console.WriteLine("Starting data storage procedures..");
            
            double[] nnDataModel = MessageData.BuildNNDataModel(dataModel);
            double[,] mainoutput = null;
            double[,] outputItem = null;
            try
            {
                Console.WriteLine("Storing neural net data");
                if (dataManager.FileExists(dataManager.NeuralNetDataPath + nnDataFileName + ".json"))
                {
                    string s = dataManager.GetFileData(dataManager.NeuralNetDataPath + nnDataFileName + ".json");
                    var desItem = JsonConvert.DeserializeObject<double[,]>(s);
                    mainoutput = dataManager.AddColumn(desItem, new double[,] { { nnDataModel[0], nnDataModel[1], nnDataModel[2] } });
                    dataManager.UpdateFile(dataManager.NeuralNetDataPath + nnDataFileName + ".json", JsonConvert.SerializeObject(mainoutput));
                }
                else
                {
                    mainoutput = new double[,] { { nnDataModel[0], nnDataModel[1], nnDataModel[2] } };
                    string jsonNNDataModel = JsonConvert.SerializeObject(mainoutput);
                    dataManager.CreateFile(jsonNNDataModel, dataManager.NeuralNetDataPath + nnDataFileName, "json");
                }
                Console.WriteLine("Done Storing neural net data");
                Console.WriteLine("Storing prediction data");
                if (dataManager.FileExists(dataManager.PredictionEngineDataPath + predDataFileName + ".json"))
                {
                    string s = dataManager.GetFileData(dataManager.PredictionEngineDataPath + predDataFileName + ".json");
                    var desItem = JsonConvert.DeserializeObject<List<PerleyMLDataModel>>(s);
                    desItem.Add(dataModel);
                    dataManager.UpdateFile(dataManager.PredictionEngineDataPath + predDataFileName + ".json", JsonConvert.SerializeObject(desItem));
                }
                else
                {
                    List<PerleyMLDataModel> pdm = new List<PerleyMLDataModel>();
                    pdm.Add(dataModel);
                    string jsonDataModel = JsonConvert.SerializeObject(pdm);
                    dataManager.CreateFile(jsonDataModel, dataManager.PredictionEngineDataPath + predDataFileName, "json");
                }
                Console.WriteLine("Done Storing prediction data");
                Console.WriteLine("Storing predicted outputs");

                if (dataManager.FileExists(dataManager.NeuralNetDataPath + "/outputdata.json"))
                {
                    string s = dataManager.GetFileData(dataManager.NeuralNetDataPath + "/outputdata.json");
                    double[,] desItem = JsonConvert.DeserializeObject<double[,]>(s);
                    outputItem = dataManager.AddColumn(desItem, new double[,] { { NeuralNetDataDictionary.GreetingSubTypes[dataModel.GreetingSubType] } });
                    dataManager.UpdateFile(dataManager.NeuralNetDataPath + "/outputdata.json", JsonConvert.SerializeObject(outputItem));
                }
                else
                {
                    outputItem = new double[,] { { NeuralNetDataDictionary.GreetingSubTypes[dataModel.GreetingSubType] } };
                    dataManager.CreateFile(JsonConvert.SerializeObject(outputItem), dataManager.NeuralNetDataPath + "/outputdata", "json");
                }
                Console.WriteLine("Done Storing predicted outputs");
                Console.WriteLine("Completed data storage procedures..");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            return (mainoutput, outputItem);
        }

        #region Testing Methods


        public static byte[] ToByteArray(string stringPassed)
        {
            System.Text.UTF8Encoding e = new System.Text.UTF8Encoding();

            return e.GetBytes(stringPassed);
        }
        #endregion
    }
}
