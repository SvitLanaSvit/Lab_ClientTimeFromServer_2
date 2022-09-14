using System.Net.Sockets;
using System.Net;
using Timer = System.Windows.Forms.Timer;
using System.Text;

namespace ServerStartTime
{
    public partial class Form1 : Form
    {
        public delegate void TextDeledate(string text);

        public Form1()
        {
            InitializeComponent();
        }

        Timer timer = null!;
        Socket senderSocket = null!;
        IPEndPoint remoteEP = null!;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new TextDeledate(UpdateFormCaption), "Server task started!");
            txtServer.ReadOnly = true;
            txtServer.Text = "Server send date with time... You shoud to open a window of client.";
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            IPAddress iPAddress = IPAddress.Parse("192.168.0.255");
            remoteEP = new IPEndPoint(iPAddress, 11000);
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            byte[] buff = Encoding.Default.GetBytes(DateTime.Now.ToString());
            await senderSocket.SendToAsync(new ArraySegment<byte>(buff), SocketFlags.None, remoteEP);
        }

        private void UpdateFormCaption(string text)
        {
            this.Text = text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("Server disconnected. Time stoped for client.");
            senderSocket.Shutdown(SocketShutdown.Send);
            senderSocket.Close();
        }
    }
}