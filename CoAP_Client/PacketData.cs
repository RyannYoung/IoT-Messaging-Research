namespace CoAP_Client;

public class PacketData
{

    public int id { get; set; }
    public string time { get; set; }
    public int sentPDUSize { get; set; }
    public int receivedPDUSize { get; set; }
    public string payload { get; set; }
    public double sentTimeMS { get; set; } // also referred to as round-trip-time
    public double ackTimeMS { get; set; }
    public double recTimeMS { get; set; }

    public PacketData(int id, string time, int sentPduSize, int receivedPduSize, string payload, double sentTimeMs, double ackTimeMs, double recTimeMs)
    {
        this.id = id;
        this.time = time;
        sentPDUSize = sentPduSize;
        receivedPDUSize = receivedPduSize;
        this.payload = payload;
        sentTimeMS = sentTimeMs;
        ackTimeMS = ackTimeMs;
        recTimeMS = recTimeMs;
    }
}