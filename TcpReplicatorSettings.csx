using System.Collections.Generic;
using System.Net;

public class TcpReplicatorSettings
{
    public IPAddress HostAddress { get; set; }

    public int HostPort { get; set; }

    public Queue<byte[]> ReplyQueue { get; set; }

    public IDelayStrategy DelayStrategy { get; set; }
}