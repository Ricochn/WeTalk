using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public bool appRun = true;
        private Listener lis;
        private Sender sen;
        string netIp;
        string chatTo;
        public void AddMessage(object sender,AddMessageEventArgs e)
        {
            string message = e.mess;
            string appendText;
            string[] sep = message.Split('>');
            appendText = sep[0] + ">:      " + System.DateTime.Now.ToString() + Environment.NewLine + sep[1] + Environment.NewLine;
            int txtGetMsgLength = this.richTextBox1.Text.Length;
            this.richTextBox1.AppendText(appendText);
            this.richTextBox1.Select(txtGetMsgLength, appendText.Length - Environment.NewLine.Length * 2 - sep[1].Length);
            this.richTextBox1.SelectionColor = Color.Red;
            this.richTextBox1.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            netIp = getNetId();
            this.label1.Text = "本机IP:" + GetMyIpAddress();
            start_listen();
        }
        //button2连接服务器
        private void button2_Click(object sender, EventArgs e)
        {
            if(textBox2.Text.Trim()=="")
            {
                MessageBox.Show("请输入主机号");
                return;
            }
            else
            {
                try
                {
                    chatTo = textBox2.Text;
                    TcpClient tcpc = new TcpClient(textBox2.Text, 5656);
                    NetworkStream tcpStream = tcpc.GetStream();
                    Byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes("1");
                    tcpStream.Write(data, 0, data.Length);
                    tcpStream.Close();
                    tcpc.Close();

                    this.toolStripStatusLabel1.Text = "当前状态：已连接到服务器";
                }
                catch(SocketException)
                {
                    MessageBox.Show("目标主机没有启动监听", "系统提示");
                }
               
            }
        }
        //button3断开
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                sen = new Sender(chatTo);
                sen.Send("0");
                lis.listenerRun = false;
                lis.Stop();
                this.toolStripStatusLabel1.Text = "当前状态：与服务器断开连接 ";
            }
            catch(NullReferenceException)
            { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox2.Text.Trim()=="")
            {
                MessageBox.Show("请选择目标主机");
                return;
            }
            else if(textBox1.Text.Trim()=="")
            {
                MessageBox.Show("消息不能为空");
            }
            else
            {
                try
                {
                    sen = new Sender(chatTo);
                    sen.Send(textBox1.Text);
                    string appendText;
                    appendText = "Me:   " + System.DateTime.Now.ToString() + Environment.NewLine + textBox1.Text + Environment.NewLine;
                    int txtGetMsgLength = this.richTextBox1.Text.Length;
                    this.richTextBox1.AppendText(appendText);
                    this.richTextBox1.Select(txtGetMsgLength, appendText.Length - Environment.NewLine.Length * 2 - textBox2.Text.Length);
                    this.richTextBox1.SelectionColor = Color.Blue;
                    this.richTextBox1.ScrollToCaret();
                }
                catch
                {

                }
                this.textBox1.Text = "";
                this.textBox1.Focus();
            }
        }
        //连接方法
        private void start_listen()
        {
            try
            {
                if(lis.listenerRun==true)
                {
                    lis.listenerRun = false;
                    lis.Stop();
                }
            }
            catch(NullReferenceException)
            {

            }
            finally
            {
                lis = new Listener();
                lis.OnAddMessage += new EventHandler<AddMessageEventArgs>(this.AddMessage);
                lis.StartListener();
            }

        }
        //获取网络号
        string getNetId()
        {
            string netId;
            string ip = GetMyIpAddress();
            netId = ip.Substring(0, ip.LastIndexOf(".") + 1);
            return netId;
        }
        //本机IP
        private static string GetMyIpAddress()
        {
            IPAddress addr = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
            return addr.ToString();
        }
    }
}
