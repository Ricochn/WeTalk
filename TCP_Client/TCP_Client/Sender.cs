using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Client
{

    class Sender
    {
        private string obj;
        public Sender(string str)
        {
            obj = str;
        }
        public void Send(string str)
        {
            try
            {
                TcpClient tcpc = new TcpClient(obj, 5656);
                NetworkStream tcpStream = tcpc.GetStream();
                Byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(str);
                tcpStream.Write(data, 0, data.Length);
                tcpStream.Close();
                tcpc.Close();
            }
            catch(Exception)
            {
                MessageBox.Show("连接被目标主机拒绝");
            }
        }

    }
}
