using BotAI.Messaging;
using BotAI.Models;
using EasyNetQ;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"..\..\cloudAMQPConnectionString.txt", Encoding.UTF8);
var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus);

var bot = new Bot(messagePublisher);
Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString, bot).Start()
);
bot.JoinGame();