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

private async Task HandleTelnetAsync(Stream stream)
    {
var buffer = new byte[2048];
        while (true)
        {
            int read = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (read <= 0) break;

            int i = 0;
            while (i < read)
            {
                if (buffer[i] == TelnetCommands.IAC)
                {
                    if (i + 1 >= read) break;

                    byte next = buffer[i + 1];

                    if (next == TelnetCommands.SB) // Subnegotiation
                    {
                        int sbStart = i + 2;
                        int sbEnd = Array.IndexOf(buffer, TelnetCommands.SE, sbStart);
                        if (sbEnd == -1) break; // incomplete, wait more

                        byte option = buffer[sbStart];
                        byte subCmd = buffer[sbStart + 1];

                        if (option == TelnetCommands.TERMINAL_TYPE && subCmd == 1) // SEND
                        {
                            // Balas terminal type
                            List<byte> response = new List<byte>
                            {
                                TelnetCommands.IAC, TelnetCommands.SB,
                                TelnetCommands.TERMINAL_TYPE, 0 // IS
                            };

                            byte[] termType = Encoding.ASCII.GetBytes("IBM-3278-2-E");
                            response.AddRange(termType);
                            response.Add(TelnetCommands.IAC);
                            response.Add(TelnetCommands.SE);

                            await stream.WriteAsync(response.ToArray(), 0, response.Count);
                        }

                        i = sbEnd + 1;
                        continue;
                    }

                    if (i + 2 >= read) break;

                    byte command = buffer[i + 1];
                    byte option = buffer[i + 2];

                    byte responseCommand;
                    switch (command)
                    {
                        case TelnetCommands.DO:
                            if (option == TelnetCommands.BINARY || option == TelnetCommands.TERMINAL_TYPE)
                                responseCommand = TelnetCommands.WILL;
                            else
                                responseCommand = TelnetCommands.WONT;
                            break;

                        case TelnetCommands.WILL:
                            if (option == TelnetCommands.SUPPRESS_GO_AHEAD || option == TelnetCommands.BINARY)
                                responseCommand = TelnetCommands.DO;
                            else
                                responseCommand = TelnetCommands.DONT;
                            break;

                        default:
                            i += 3;
                            continue;
                    }

                    byte[] response = new byte[]
                    {
                        TelnetCommands.IAC,
                        responseCommand,
                        option
                    };

                    await stream.WriteAsync(response, 0, response.Length);
                    i += 3;
                }
                else
                {
                    // Ini data layar 5250
                    string hex = BitConverter.ToString(buffer, i, read - i);
                    OnRawDataReceived?.Invoke(hex);
                    Console.WriteLine("DATA >> " + hex);
                    break;
                }

            }
        }
    }

