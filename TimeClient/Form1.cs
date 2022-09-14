using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TimeClient
{
    public partial class Form1 : Form
    {
        public delegate void TextDelegate(string text);
        public Form1()
        {
            InitializeComponent();
            txtInfoClient.ReadOnly = true;
        }

        Task task = null!;
        IPEndPoint remoteEP = null!;
        Socket receiverSocket = null!;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new TextDelegate(UpdateFormCaption), "Client get data from server...");
            if (task == null)
            {
                task = Task.Run(async () =>
                {
                    receiverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                    IPAddress iPAddress = IPAddress.Parse("192.168.0.83");
                    IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 11000);
                    try
                    {
                        receiverSocket.Bind(iPEndPoint);
                        do
                        {
                            remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                            byte[] buffer = new byte[1024];
                            SocketReceiveFromResult res = await receiverSocket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, remoteEP);
                            string text = Encoding.Default.GetString(buffer, 0, res.ReceivedBytes);
                            txtInfoClient.BeginInvoke(new TextDelegate(UpdateText), $"{text}");
                        } while (true);
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
        }

        private void UpdateText(string text)
        {
            txtInfoClient.Text = text;
        }

        private void UpdateFormCaption(string text)
        {
            this.Text = text;
        }
    }
}