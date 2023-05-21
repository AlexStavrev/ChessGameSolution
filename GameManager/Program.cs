using EasyNetQ;
using GameManager.Messaging;
using GameManager.Models;
using SharedDTOs.Monitoring;
using System.Reflection;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"/app/cloudAMQPConnectionString.txt", Encoding.UTF8);

var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus);

using var activity = Monitoring.ActivitySource.StartActivity(MethodBase.GetCurrentMethod()!.Name);
var gamesManager = new GamesManager(messagePublisher);

Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString, gamesManager).Start()
).Wait();
