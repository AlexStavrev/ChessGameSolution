using BoardManager.ApiClient;
using BoardManager.Messaging;
using BoardManager.Models;
using EasyNetQ;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"/app/cloudAMQPConnectionString.txt", Encoding.UTF8);
var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus, new ApiClient());

var board = new ChessBoard(messagePublisher);
var task = Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString, board).Start()
);
board.RegisterBoard();

task.Wait();