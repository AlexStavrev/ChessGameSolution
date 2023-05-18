using BotAI.Messaging;
using BotAI.Models;
using BotAI.Strategies;
using EasyNetQ;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"/app/cloudAMQPConnectionString.txt", Encoding.UTF8);
var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus);

var bot = new Bot(messagePublisher) { Strategy = StrategyFactory.GetRandomStrategy() };
var task = Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString, bot).Start()
);
bot.JoinGame();

task.Wait();