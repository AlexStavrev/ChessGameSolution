using BoardManager.Messaging;
using EasyNetQ;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"..\..\cloudAMQPConnectionString.txt", Encoding.UTF8);
var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus);