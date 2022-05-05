using Amqp;

var address = new Address("amqp://guest:guest@localhost:5672");
var connection = new Connection(address);
var session = new Session(connection);
var receiver = new ReceiverLink(session, "receiver-link", "q1");

Console.WriteLine("Receiver connected to broker.");
var message = receiver.Receive(TimeSpan.MaxValue);

Console.WriteLine($"Received: {message.Body}");
receiver.Accept(message);

receiver.Close();
session.Close();
connection.Close();