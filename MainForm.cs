using System;
using System.Windows.Forms;
using Auto5250net.Terminal;

namespace Auto5250net.WinForms
{
    public partial class MainForm : Form
    {
        private TextBox outputTextBox;
        private Button connectButton;
        private TerminalSession session;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.outputTextBox = new TextBox();
            this.connectButton = new Button();

            this.outputTextBox.Multiline = true;
            this.outputTextBox.ScrollBars = ScrollBars.Vertical;
            this.outputTextBox.Dock = DockStyle.Fill;

            this.connectButton.Text = "Connect to IBM i";
            this.connectButton.Dock = DockStyle.Top;
            this.connectButton.Click += ConnectButton_Click;

            this.Controls.Add(outputTextBox);
            this.Controls.Add(connectButton);

            this.Text = "5250 Emulator (Basic)";
            this.Width = 800;
            this.Height = 600;
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Inisialisasi session terminal
                session = new TerminalSession();
                session.OnRawDataReceived += AppendText;

                // Koneksi ke IBM i via SSL
                await session.ConnectAsync("your-ibmi-host", 992);
                AppendText("Connected to IBM i...\r\n");

                // Mulai baca data dari server
                _ = session.StartReadingAsync();
            }
            catch (Exception ex)
            {
                AppendText("Error: " + ex.Message + "\r\n");
            }
        }

        // Method buat nambahin teks ke outputTextBox
        private void AppendText(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => outputTextBox.AppendText(text + "\r\n")));
            }
            else
            {
                outputTextBox.AppendText(text + "\r\n");
            }
        }
    }
}
