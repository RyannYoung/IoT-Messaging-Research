namespace HTTP_Client;

public class PacketData
{
    public string Time { get; set; }
    public int Count { get; set; }
    public float Rtt { get; set; }
    public int FrameSize { get; set; }

    public PacketData(string time, int count, float rtt, int frameSize)
    {
        Time = time;
        Count = count;
        Rtt = rtt;
        FrameSize = frameSize;
    }
}