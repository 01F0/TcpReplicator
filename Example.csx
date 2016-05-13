#load "TcpReplicator.csx"
using System.Linq;
using System.Threading;

const int port = 9999;
var cancellationTokenSource = new CancellationTokenSource();
var answers = new Queue<byte[]>();
answers.Enqueue(new byte[] { 0x2A });

var task = new Task(() => TcpReplicator.Replicate(IPAddress.Loopback, port, cancellationTokenSource.Token, answers));
task.Start();

new ManualResetEvent(false).WaitOne(500); // Give it a little time to start up.

var tcpClient = new TcpClient("localhost", port);

var bytesToWrite = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()); // This doesn't get evaluated by the server.

System.Console.WriteLine(Encoding.UTF8.GetString(bytesToWrite));

using (var stream = tcpClient.GetStream())
{
    var buffer = new byte[1];

    stream.Write(bytesToWrite, 0, bytesToWrite.Length);
    stream.Read(buffer, 0, buffer.Length);
    System.Console.WriteLine(buffer[0]);
}

tcpClient.Close();