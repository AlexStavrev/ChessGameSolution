using GameManager.Messaging;
using System.Text;

string cloudAMQPConnectionString = File.ReadAllText(@"..\..\cloudAMQPConnectionString.txt", Encoding.UTF8);

Task.Factory.StartNew(() =>
    new MessageSubscriber(cloudAMQPConnectionString).Start()
);