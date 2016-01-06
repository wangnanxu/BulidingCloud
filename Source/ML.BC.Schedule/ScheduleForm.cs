using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ML.BC.Schedule
{
    public partial class ScheduleForm : Form
    {
        private int _lines = 10;
        private bool _status = false;
        public ScheduleForm()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void Instance_Timer_Elapsed_End(TimerEventArgs e)
        {           
            textBox1.Text += e.Message + "\r\n";
        }
        public void init()
        {
        }
        private void Instance_Timer_Elapsed_Begin(TimerEventArgs e)
        {
            textBox1.Text += e.Message + "\r\n\r\n"; 

            string[] temp = textBox1.Text.Split(new string[]{"\r\n"},StringSplitOptions.RemoveEmptyEntries);
            int count = temp.Count();
            if(_lines < count)
            {
                var tempBuilder = new StringBuilder();
                for(int i = count - _lines;i < count;i++)
                {
                    tempBuilder.Append(temp[i] + "\r\n\r\n");
                }
                textBox1.Text = tempBuilder.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(!_status)
            {
                Schedule.Instance.Timer_Elapsed_Begin += new TimerHandler(Instance_Timer_Elapsed_Begin);
                Schedule.Instance.Timer_Elapsed_End += new TimerHandler(Instance_Timer_Elapsed_End);
            }
            button2.Enabled = true;
            button3.Enabled = true;
            button1.Enabled = false;
            Schedule.Instance.StartTimer();
            label1.Text = "定时器已启动...";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Schedule.Instance.Dispose();
            _status = false;
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            label1.Text = "定时器已停止...";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Schedule.Instance.pauseTimer();
            button3.Enabled = false;
            button4.Enabled = true;
            label1.Text = "定时器已暂停...";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!_status)
            {
                Schedule.Instance.Timer_Elapsed_Begin += new TimerHandler(Instance_Timer_Elapsed_Begin);
                Schedule.Instance.Timer_Elapsed_End += new TimerHandler(Instance_Timer_Elapsed_End);
            }
            button4.Enabled = false;
            button3.Enabled = true;
            Schedule.Instance.StartTimer();
            label1.Text = "定时器已恢复...";
        }
        //private void ScheduleForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出!
        //    if (e.CloseReason == CloseReason.UserClosing)
        //    {
        //        e.Cancel = true;    //取消"关闭窗口"事件
        //        this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果
        //        notifyIcon1.Visible = false;
        //        this.Hide();
        //        return;
        //    }
        //}
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true;
                //使关闭时窗口向右下角缩小的效果
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
                return;
            }
        }

        private void ExieClick(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            this.Close();
            this.Dispose();
            System.Environment.Exit(0);
        }

        private void ScheduleForm_Load(object sender, EventArgs e)
        {

        }
    }
}
