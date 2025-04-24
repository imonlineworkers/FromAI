using System;
using System.Windows.Forms;
using Auto5250net.Terminal;

namespace Auto5250net.WinForms
{
    public partial class MainForm : Form
    {
        private TerminalSession _session;

    public MainForm()
    {
        InitializeComponent();
        _session = new TerminalSession("your_ibm_host", 992);
        _session.OnDataReceived += data =>
        {
            string parsed = ParseToAscii(data);
            DisplayOnGrid(parsed);
        };
    }

    private async void connectBtn_Click(object sender, EventArgs e)
    {
        await _session.ConnectAsync();
    }
}

    private Label[,] screenLabels;

    private void InitializeComponent()
    {
        this.Text = "Auto5250net - Emulator";
        this.Size = new System.Drawing.Size(850, 550);

        var connectBtn = new Button();
        connectBtn.Text = "Connect";
        connectBtn.Location = new System.Drawing.Point(10, 10);
        connectBtn.Click += connectBtn_Click;
        this.Controls.Add(connectBtn);

        screenLabels = new Label[24, 80];
        var startY = 50;
        var font = new System.Drawing.Font("Consolas", 9);

        for (int row = 0; row < 24; row++)
        {
            for (int col = 0; col < 80; col++)
            {
                var label = new Label();
                label.Text = " ";
                label.Font = font;
                label.Size = new System.Drawing.Size(10, 16);
                label.Location = new System.Drawing.Point(10 + col * 10, startY + row * 16);
                this.Controls.Add(label);
                screenLabels[row, col] = label;
            }
        }
    }

    private string ParseToAscii(string hexString)
    {
        var bytes = hexString.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
        var sb = new StringBuilder();

        foreach (var b in bytes)
        {
            if (b >= 0x20 && b <= 0x7E)
                sb.Append((char)b);
            else
                sb.Append(' ');
        }

        return sb.ToString();
    }

    private void DisplayOnGrid(string asciiData)
    {
        int index = 0;
        for (int row = 0; row < 24; row++)
        {
            for (int col = 0; col < 80; col++)
            {
                if (index < asciiData.Length)
                {
                    screenLabels[row, col].Text = asciiData[index].ToString();
                    index++;
                }
            }
        }

    }
}



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
