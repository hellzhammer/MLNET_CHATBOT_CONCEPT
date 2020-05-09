using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gtk;

using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using PerleyMLMQTTlib;
using PerleyMLMQTTlib.Models;

public partial class MainWindow : Gtk.Window
{
    public static MainWindow mainSingleton { get; set; }
    MessengerNode client { get; set; }
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        client = new MessengerNode();
        Build();
        runner();

        SendButton.Clicked += SendMessage;
        ExecuteBashCommand("espeak -p80 'welcome!'");
        mainSingleton = this;
    }

    void ExecuteBashCommand(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "-c \"" + command + "\"" };
        Process proc = new Process() { StartInfo = startInfo, };
        proc.Start();
        proc.WaitForExit();
        proc.Dispose();
    }

    public void UpdateMsg(string newData)
    {
        ExecuteBashCommand("espeak -p80 '" + newData + "'");
        MessagesTextBox.Buffer.Text += "\n Kronus Said: " + newData;
    }

    public async void runner()
    {
        await client.BuildClient("Messenger");
    } 

    private void SendMessage(object sender, EventArgs e)
    {
        string msgData = MessageEntry.Text;
        client.SendMessage(msgData, client.Subscription + "/kronusLogic");
        MessagesTextBox.Buffer.Text += "\n You Said: " + msgData;
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }
}

public class MessengerNode : CentralNodeClient
{
    //public ConversationModel convo = new ConversationModel();
    public MessengerNode()
    {
        //convo.messages = new System.Collections.Generic.List<string>();
    }
    public override void MessageRecieved(MqttApplicationMessageReceivedEventArgs e)
    {
        base.MessageRecieved(e);
        //convo.messages.Add(System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        MainWindow.mainSingleton.UpdateMsg(System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
    }
}
