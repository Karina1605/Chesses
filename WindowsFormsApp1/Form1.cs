using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.MinimumSize = new Size(780, 630);
            this.MaximumSize = new Size(780, 630);
            this.BackColor = Color.RosyBrown;
            textBox1.Width =textBox2.Width= 690;
            button1.Width = button2.Width = 160;
            button1.Height = button2.Height = 100;
            button1.TextAlign = button2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            button1.Location= new Point(100, 410);
            button2.Location = new Point(430, 410);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
                MessageBox.Show("Пожалуйста, введите имена двоих игроков", "Не все имена введены", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Form2 form2 = new Form2(textBox1.Text, textBox2.Text);
                this.Hide();
                form2.Show();
            }
           

        }
    }
}
