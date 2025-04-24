using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Auto5250net.Terminal
{
    public class TerminalSession
    {
        private SslStream sslStream;
        private TcpClient client;

        public event Action<string> OnRawDataReceived;

        public async Task ConnectAsync(string host, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(host, port);

            sslStream = new SslStream(client.GetStream(), false,
                (sender, cert, chain, errors) => true);
            await sslStream.AuthenticateAsClientAsync(host);
        }

        public async Task StartReadingAsync()
        {
            byte[] buffer = new byte[4096];
            int bytesRead;

            while ((bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                string hex = BitConverter.ToString(buffer, 0, bytesRead);
                OnRawDataReceived?.Invoke(hex);
            }
        }

        public async Task SendAsync(byte[] data)
        {
            if (sslStream != null)
            {
                await sslStream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
