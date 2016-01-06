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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = true;
            this.AcceptButton = button1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if(textBox1.Text == EnterpriseData.Common.CommonConfig.SCHEDULEPASSWORD)
            {
            ScheduleForm Schedule = new ScheduleForm();
            Schedule.Show();
            this.Hide();
            }
            else
            {
                label2.Text = "密码错误，请重新输入";
                button1.Enabled = true;
            }
        }
    }
}
