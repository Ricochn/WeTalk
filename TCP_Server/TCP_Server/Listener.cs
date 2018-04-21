using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Server
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
        public event EventHandler<AddMessageEventArgs> OnIpRemod;
        public Listener()
        { }

        public void StartListener()
        {
            th=new Thread(new ThreadStart(Listen));
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
                IPEndPoint ipLocalEndPoint = new IPEndPoint(addr, 5656);

                tcp1 = new TcpListener(ipLocalEndPoint);
                tcp1.Start();

                while(listenerRun)
                {
                    Socket s = tcp1.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[1024];
                    int i = s.Receive(stream);
                    string msg;
                    string str = System.Text.Encoding.UTF8.GetString(stream);
                    if(str.Substring(0,1)=="1")
                    {
                        string str_ = "欢迎登陆！";
                        TcpClient tcpc = new TcpClient(((IPEndPoint)s.RemoteEndPoint).Address.ToString(), 5657);
                        NetworkStream tcpStream = tcpc.GetStream();
                        Byte[] data = System.Text.Encoding.UTF8.GetBytes(str_);
                        tcpStream.Write(data, 0, data.Length);
                        tcpStream.Close();
                        tcpc.Close();

                        msg = "<" + remote + ">" + "上线";
                        AddMessageEventArgs arg = new AddMessageEventArgs();
                        arg.mess = msg;
                    }
                    else if(str.Substring(0,1)=="0")
                    {
                        msg = "<" + remote + ">"+"断开";
                        AddMessageEventArgs argRe = new AddMessageEventArgs();
                        argRe.mess = remote.ToString();
                        OnIpRemod(this, argRe);
                    }

                    else
                    {
                        msg = "<" + remote + ">" +
                            System.Text.UTF8Encoding.UTF8.GetString(stream);
                        AddMessageEventArgs arg = new AddMessageEventArgs();
                        arg.mess = msg;
                        OnAddMessage(this, arg);
                    }
                }

            }
            catch(System.Security.SecurityException)
            {
                MessageBox.Show("防火墙禁止连接！");
            }
            catch (Exception) { }
        }
    }
}
