using BotAI.Messaging;
using BotAI.Models;
using EasyNetQ;
using System.Runtime.CompilerServices;
using System.Text;

internal class Program
{
    public static Bot Bot { get; private set; }

    private static void Main(string[] args)
    {
        string cloudAMQPConnectionString = File.ReadAllText(@"..\..\cloudAMQPConnectionString.txt", Encoding.UTF8);

        var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);

        var messagePublisher = new MessagePublisher(bus);

        Bot = new Bot(messagePublisher);
    }
    
}