﻿using EasyNetQ;
using GameManager.Messaging;
using GameManager.Models;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"..\..\cloudAMQPConnectionString.txt", Encoding.UTF8);

var bus = RabbitHutch.CreateBus(cloudAMQPConnectionString);
var messagePublisher = new MessagePublisher(bus);

var gamesManager = new GamesManager(messagePublisher);
Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString, gamesManager).Start()
);