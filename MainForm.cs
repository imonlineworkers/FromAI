using Auto5250net.Terminal;

private TerminalSession session;

private async void ConnectButton_Click(object sender, EventArgs e)
{
    try
    {
        session = new TerminalSession();
        session.OnRawDataReceived += AppendText;

        await session.ConnectAsync("your-ibmi-host", 992);
        AppendText("Connected to IBM i...\r\n");

        _ = session.StartReadingAsync();
    }
    catch (Exception ex)
    {
        AppendText("Error: " + ex.Message + "\r\n");
    }
}
