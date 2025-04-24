using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public class TerminalSession
{
    private TcpClient _client;
    private SslStream _sslStream;

    public event Action<byte[]> DataReceived;

    public bool IsConnected => _client?.Connected ?? false;

    public void Connect(string host, int port = 992)
    {
        _client = new TcpClient();
        _client.Connect(host, port);

        _sslStream = new SslStream(
            _client.GetStream(),
            false,
            new RemoteCertificateValidationCallback(ValidateServerCertificate),
            null
        );

        _sslStream.AuthenticateAsClient(host);

        Task.Run(() => ReceiveLoop());
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];
        try
        {
            while (true)
            {
                int bytesRead = await _sslStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                byte[] data = buffer.Take(bytesRead).ToArray();
                DataReceived?.Invoke(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Receive error: " + ex.Message);
        }
    }

    public void SendData(byte[] data)
    {
        if (_sslStream != null && _sslStream.CanWrite)
        {
            _sslStream.Write(data, 0, data.Length);
            _sslStream.Flush();
        }
    }

    public void Disconnect()
    {
        try
        {
            _sslStream?.Close();
            _client?.Close();
        }
        catch { }
    }

    private static bool ValidateServerCertificate(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
}
