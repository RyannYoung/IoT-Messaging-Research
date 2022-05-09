namespace TestManager;

public class PacketData
{
    public int id { get; set; }
    public DateTime time { get; set; }
    public byte[] sentPDU { get; set; }
    public byte[] receivedPDU { get; set; }
    public string payload { get; set; }
    public DateTime sentTime { get; set; } // also referred to as round-trip-time
    public DateTime ackTime { get; set; }
    public DateTime recTime { get; set; }
}