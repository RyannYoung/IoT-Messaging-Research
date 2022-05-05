namespace HTTP_Server;

public class Response
{
    public byte[]? Data { get; set; }
    public string MimeType { get; set; } = "text/plain";
}