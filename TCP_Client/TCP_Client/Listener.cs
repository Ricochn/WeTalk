using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Client
{
    public class AddMessageEventArgs:EventArgs
    {
        public string mess;
    }
    class Listener
    {
        private Thread th;
        private TcpListener tcp1;
        public bool listenerRun = true;
        public event EventHandler<AddMessageEventArgs> OnAddMessage;
        public Listener()
        {

        }
        public void StartListener()
        {
            th = new Thread(new ThreadStart(Listen));
            th.Start();
        }
        public void Stop()
        {
            tcp1.Stop();
            th.Abort();
        }
        private void Listen()
        {
            try
            {
                IPAddress addr = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
                IPEndPoint ipLocalEndPoint = new IPEndPoint(addr, 5657);
                tcp1 = new TcpListener(ipLocalEndPoint);
                tcp1.Start();


                while(listenerRun)
                {
                    Socket s = tcp1.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[512];
                    int i = s.Receive(stream);
                    string msg = "<" + remote + ">" + System.Text.UTF8Encoding.UTF8.GetString(stream);
                    AddMessageEventArgs arg = new AddMessageEventArgs();
                    arg.mess = msg;
                    OnAddMessage(this, arg);
                }
            }
            catch(System.Security.SecurityException)
            {
                MessageBox.Show("防火墙禁止连接！");
            }
            catch(Exception)
            {
                MessageBox.Show("监听已经停止!");
            }
        }
    }
}
