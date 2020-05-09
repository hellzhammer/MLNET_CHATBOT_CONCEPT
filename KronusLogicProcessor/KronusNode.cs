using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using PerleyMLMQTTlib;
using PerleyMLMQTTlib.Models;

namespace KronusLogicProcessor
{
    public class KronusNode : CentralNodeClient
    {
        public override void MessageRecieved(MqttApplicationMessageReceivedEventArgs e)
        {
            base.MessageRecieved(e);

            Console.WriteLine("NEW MSG+++++++++++++++++++++++++++");

            string data = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Console.WriteLine(data);

            Program.MessageRecieved(("Messenger", data));

            Console.WriteLine("END MSG+++++++++++++++++++++++++++");
        }
       
        private PerleyMLDataModel MessageRecievedHandler(MqttApplicationMessageReceivedEventArgs e) 
        {
            //process message to get data to build response.

            return null;
        }

        private void HandleResponse(MqttApplicationMessageReceivedEventArgs e, PerleyMLDataModel data)
        {
            //return response to sender
            SendMessage("TODO: Add message builder logic", Subscription + "/kronusLogic"); // /Messenger
        }
    }
}
