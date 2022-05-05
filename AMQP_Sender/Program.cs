using Amqp;

var address = new Address("amqp://guest:guest@localhost:5672");
var connection = new Connection(address);
var session = new Session(connection);

var message = new Message("Hello AMQP!");
var senderLink = new SenderLink(session, "sender-link", "IOTData");
senderLink.Send(message);

Console.WriteLine("Sent Message!");

senderLink.Close();
session.Close();
connection.Close();