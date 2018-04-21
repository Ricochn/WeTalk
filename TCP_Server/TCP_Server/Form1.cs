using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCP_Server
{
    public partial class Form1 : Form
    {
        string chatTo;
        private Listener lis;
        string netIp;
        private Sender sen;

        public Form1()
        {
            InitializeComponent();
        }
        //返回信息
        public void AddMessage(object sender,AddMessageEventArgs e)
        {
            string message = e.mess;
            string appendText;
            string[] sep = message.Split('>');
            string[] sepIp = sep[0].Split('<', ':');
            bool checkIp = true;
            for(int i=0;i<listBox1.Items.Count;i++)
            {
                if(listBox1.Items[i].ToString()==sepIp[1])
                {
                    checkIp = false;
                }
            }
            if(checkIp&&sep[1]!="断开")
            {
                this.listBox1.Items.Add(sepIp[1].Trim());
                chatTo = sepIp[1];
            }
            appendText = sep[0] + ">:   " + System.DateTime.Now.ToString() + Environment.NewLine + sep[1] + Environment.NewLine;
            int txtGetMsgLength = this.richTextBox1.Text.Length;
            this.richTextBox1.AppendText(appendText);
            this.richTextBox1.Select(txtGetMsgLength, appendText.Length - Environment.NewLine.Length * 2 - sep[1].Length);
            this.richTextBox1.SelectionColor = Color.Red;
            this.richTextBox1.ScrollToCaret();
        }
        //下线
        public void IpRemo(object sender,AddMessageEventArgs e)
        {
            string[] sep = e.mess.Split(':');
            try
            {
                int index = 0;
               for(int i=0;i<listBox1.Items.Count;i++)
                {
                    if(listBox1.Items[i].ToString()==sep[0].ToString())
                    {
                        index = i;
                        this.listBox1.Items.RemoveAt(index);
                    }
                }
            }
            catch
            {
                MessageBox.Show("没有这个IP");
            }
        }
        
        //启动监听
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.start_listen();
            this.toolStripStatusLabel2.Text = "监听已启动    ";
        }
        //停止监听
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                lis.listenerRun = false;
                lis.Stop();
                this.toolStripStatusLabel2.Text = "监听已停止   ";
            }
            catch(NullReferenceException)
            { }
        }
        //连接
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
            catch (NullReferenceException) { }
            finally
            {
                lis = new Listener();
                lis.OnAddMessage += new EventHandler<AddMessageEventArgs>(this.AddMessage);
                lis.OnIpRemod += new EventHandler<AddMessageEventArgs>(this.IpRemo);
                lis.StartListener();
            }
        }
        //获取网络号
        string getNetId()
        {
            string NetId;
            string ip = GetMyIpAddress();
            NetId = ip.Substring(0, ip.LastIndexOf(".") + 1);
            return NetId;
        }
        //获取本机IP
        private static string GetMyIpAddress()
        {
            IPAddress addr = new System.Net.IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
            return addr.ToString();
        }
        //发送
        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex<0&&chatTo==""&&chatTo==null&&listBox1.SelectedIndex<0)
            {
                MessageBox.Show("请选择目标主机");
                return;
            }
            else if(textBox1.Text.Trim()=="")
            {
                MessageBox.Show("消息内容不能为空！", "错误");
                this.textBox1.Focus();
                return;
            }
            else
            {
                try
                {
                    sen = new Sender(chatTo);
                    sen.Send(textBox1.Text);
                    string appendText;
                    appendText = "Me:" + System.DateTime.Now.ToString() + Environment.NewLine + textBox1.Text + Environment.NewLine;
                    int txtGetMsgLength = this.richTextBox1.Text.Length;
                    this.richTextBox1.AppendText(appendText);
                    this.richTextBox1.Select(txtGetMsgLength, appendText.Length - Environment.NewLine.Length * 2 - textBox1.Text.Length);
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
        private void listBox1_MouseDoubleClick(object sender,MouseEventArgs e)
        {
            if(e.Clicks!=0)
            {
                if(listBox1.SelectedItem!=null)
                {
                    this.start_listen();
                    chatTo = listBox1.SelectedItem.ToString();
                }
            }
        }





        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            netIp = getNetId();
            this.label1.Text = "本机的IP是:" + GetMyIpAddress();
        }
    }
}
