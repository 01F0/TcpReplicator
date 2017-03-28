using System.Net.Sockets;
using System.Threading;

public static class TcpReplicator
{
     public static void Replicate(TcpReplicatorSettings tcpReplicatorSettings, CancellationToken cancellationToken)
    {
        var tcpListener = new TcpListener(tcpReplicatorSettings.HostAddress, tcpReplicatorSettings.HostPort);

        tcpListener.Start();
       
		using (var tcpClient = tcpListener.AcceptTcpClient())
		{
			using (var stream = tcpClient.GetStream())
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (stream.DataAvailable)
					{
						if (tcpReplicatorSettings.ReplyQueue.Count == 0)
							break;

						ReadAllAvailableData(stream); // We'll just read everything that exists, but we don't care about the answer.
                    
                    				tcpReplicatorSettings.DelayStrategy.Run();

						var nextAnswer = tcpReplicatorSettings.ReplyQueue.Dequeue();
						stream.Write(nextAnswer, 0, nextAnswer.Length);
					}

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
