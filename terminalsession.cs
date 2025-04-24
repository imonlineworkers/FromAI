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
       private readonly string host;
    private readonly int port;
    private SslStream sslStream;

    public Action<string> OnDataReceived;

    public TerminalSession(string host, int port)
    {
        this.host = host;
        this.port = port;
    }

    public async Task ConnectAsync()
    {
        var client = new TcpClient();
        await client.ConnectAsync(host, port);

        sslStream = new SslStream(client.GetStream(), false,
            new RemoteCertificateValidationCallback((s, cert, chain, sslErrors) => true));

        await sslStream.AuthenticateAsClientAsync(host);
        Console.WriteLine("Connected to IBM i");

        _ = Task.Run(() => ReadLoop());
    }

    private async Task ReadLoop()
    {
        var buffer = new byte[2048];
        while (true)
        {
            int bytes = await sslStream.ReadAsync(buffer, 0, buffer.Length);
            if (bytes <= 0) break;

            int i = 0;
            while (i < bytes)
            {
                if (buffer[i] == 255) // IAC
                {
                    if (i + 2 >= bytes) break;
                    byte command = buffer[i + 1];
                    byte option = buffer[i + 2];

                    byte responseCmd = 0;
                    if (command == 253) // DO
                        responseCmd = (option == 0x18 || option == 0x00) ? (byte)251 : (byte)252; // WILL / WONT
                    else if (command == 251) // WILL
                        responseCmd = (option == 0x03) ? (byte)253 : (byte)254; // DO / DONT

                    byte[] response = new byte[] { 255, responseCmd, option };
                    await sslStream.WriteAsync(response, 0, response.Length);
                    i += 3;
                }
                else
                {
                    string hex = BitConverter.ToString(buffer, i, bytes - i);
                    OnDataReceived?.Invoke(hex);
                    break;
                }
            }
        }
    }

    public async Task SendRawAsync(byte[] data)
    {
        await sslStream.WriteAsync(data, 0, data.Length);
        await sslStream.FlushAsync();
    }

        }
    }

