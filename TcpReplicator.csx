using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public static class TcpReplicator
{
    public static void Replicate(IPAddress ipAddress, int port, CancellationToken cancellationToken, Queue<byte[]> replies)
    {
        var tcpListener = new TcpListener(ipAddress, port);

        tcpListener.Start();

       
		using (var tcpClient = tcpListener.AcceptTcpClient())
		{
			using (var stream = tcpClient.GetStream())
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (!stream.DataAvailable)
						continue;

					if (replies.Count == 0)
						break;

					ReadAllAvailableData(stream); // We'll just read everything that exists, but we don't care about the answer.

					var nextAnswer = replies.Dequeue();
					stream.Write(nextAnswer, 0, nextAnswer.Length);

					new ManualResetEvent(false).WaitOne(10);
				}
			}
		}
		
		tcpListener.Stop();
    }

    public static void ReadAllAvailableData(NetworkStream stream)
    {
        var buffer = new byte[1024];
        do
        {
            stream.Read(new byte[1024], 0, buffer.Length);

        } while (stream.DataAvailable);
    }
}