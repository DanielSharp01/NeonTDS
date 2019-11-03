using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        //create a new server
        var server = new GameServer();

        //start listening for messages and copy the messages back to the client
        Task.Factory.StartNew(async () => {
            while (true)
            {
                var received = await server.Receive();
                server.Reply("copy " + received.Message, received.Sender);
                if (received.Message == "quit")
                    break;
            }
        });

        //create a new client
        var client = PlayerClient.ConnectTo("127.0.0.1", 32123);

        //wait for reply messages from server and send them to console 
        Task.Factory.StartNew(async () => {
            while (true)
            {
                try
                {
                    var received = await client.Receive();
                    Console.WriteLine(received.Message);
                    if (received.Message.Contains("quit"))
                        break;
                }
                catch (Exception ex)
                {
                    Debug.Write(ex);
                }
            }
        });

        //sending datas for the server
        string data;
        do
        {
            data = "datas";
            client.Send(data);
        } while (data != "quit");
    }
}