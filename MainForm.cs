using System;
using System.Drawing;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private TerminalSession _session;
    private Screen5250 _screen;

    public MainForm()
    {
        InitializeComponent();
        InitUI();
    }

    private void InitUI()
    {
        this.Text = "Auto5250net Emulator";
        this.Width = 800;
        this.Height = 400;

        Panel panelScreen = new Panel
        {
            Name = "panelScreen",
            Dock = DockStyle.Fill,
            BackColor = Color.Black
        };
        panelScreen.Paint += panelScreen_Paint;
        this.Controls.Add(panelScreen);

        // Connect saat form load
        this.Load += (s, e) =>
        {
            _session = new TerminalSession();
            _session.DataReceived += OnDataReceived;
            _session.Connect("your-ibmi-host"); // ganti host sesuai kebutuhan
        };
    }

    private void OnDataReceived(byte[] data)
    {
        if (InvokeRequired)
        {
            Invoke(() => OnDataReceived(data));
            return;
        }

        var parser = new ScreenParser(data);
        _screen = parser.Parse();

        var panel = this.Controls["panelScreen"];
        panel?.Invalidate();
    }

    private void panelScreen_Paint(object sender, PaintEventArgs e)
    {
        if (_screen == null) return;

        var g = e.Graphics;
        g.Clear(Color.Black);

        var font = new Font("Consolas", 10, FontStyle.Regular);
        var brushNormal = new SolidBrush(Color.LawnGreen);
        var brushReverse = new SolidBrush(Color.Black);
        var backReverse = new SolidBrush(Color.LawnGreen);

        int charWidth = 8;
        int charHeight = 16;

        for (int row = 0; row < Screen5250.Rows; row++)
        {
            for (int col = 0; col < Screen5250.Cols; col++)
            {
                var ch = _screen.Buffer[row, col];

                int x = col * charWidth;
                int y = row * charHeight;

                if (ch.Reverse)
                {
                    g.FillRectangle(backReverse, x, y, charWidth, charHeight);
                    g.DrawString(ch.Char.ToString(), font, brushReverse, x, y);
                }
                else
                {
                    g.DrawString(ch.Char.ToString(), font, brushNormal, x, y);
                }
            }
        }
    }
}
