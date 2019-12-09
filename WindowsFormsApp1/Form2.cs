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
    public partial class Form2 : Form
    {
        public Form2( string s1, string s2)
        {
            InitializeComponent();
            this.MinimumSize = new Size(780, 670);
            this.MaximumSize = new Size(780, 670);
            this.BackColor = Color.RosyBrown;
            GameSet gameSet = new GameSet(s1, s2, this);
            Button EndGame = new Button();
            EndGame.Location = new Point(570, 0);
            EndGame.Width = 150;
            EndGame.Height = 60;
            EndGame.Text = "Выйти";
            EndGame.BackColor = Color.RosyBrown;
            EndGame.Font = new Font("Arial", 20f, FontStyle.Regular);
            EndGame.ForeColor = Color.WhiteSmoke;
            EndGame.TextAlign = ContentAlignment.MiddleCenter;
            EndGame.Click += EndGame_Click;
            this.Controls.Add(EndGame);
            Label Caption1 = new Label();
            Caption1.Text = "Игрок 2";
            Caption1.Width = 210;
            Caption1.Height = 60;
            Caption1.Font = new Font("Arial", 20f, FontStyle.Regular);
            Caption1.Location = new Point(570, 90);
            this.Controls.Add(Caption1);
            
            Label Caption2 = new Label();
            Caption2.Text = "Игрок 1";
            Caption2.Width = 210;
            Caption2.Height = 60;
            Caption2.Font = new Font("Arial", 20f, FontStyle.Regular);
            Caption2.Location = new Point(570, 450);
            this.Controls.Add(Caption2);
        }

        private void EndGame_Click(object sender, EventArgs e)
        {
            DialogResult dialog = new DialogResult();
            dialog = MessageBox.Show("Вы уверены?", "Выход", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
                Application.Exit();
        }
    }
}
