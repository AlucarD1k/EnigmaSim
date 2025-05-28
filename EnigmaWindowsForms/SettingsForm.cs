using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnigmaWindowsForms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void UpdateTheme()
        {
            button2.BackColor = EnigmaWindowsForms.Properties.Settings.Default.BtnLoadColor;

            button1.BackColor = EnigmaWindowsForms.Properties.Settings.Default.BtnSaveColor;

            button3.BackColor = EnigmaWindowsForms.Properties.Settings.Default.BtnResetColor;

            this.ForeColor = EnigmaWindowsForms.Properties.Settings.Default.TextColor;

            button4.BackColor = EnigmaWindowsForms.Properties.Settings.Default.TextColor;

            foreach (Control control in this.Controls)
            {
                if (control.GetType() != typeof(PictureBox))
                {
                    control.ForeColor = EnigmaWindowsForms.Properties.Settings.Default.TextColor;
                }
            }

            button5.BackColor = EnigmaWindowsForms.Properties.Settings.Default.FormColor;
            this.BackColor = EnigmaWindowsForms.Properties.Settings.Default.FormColor;

            this.Font = EnigmaWindowsForms.Properties.Settings.Default.FormFont;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            UpdateTheme();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button1.BackColor = colorDialog1.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                button2.BackColor = colorDialog2.Color;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog3.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor = colorDialog3.Color;
                button8.BackColor = colorDialog3.Color;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (colorDialog4.ShowDialog() == DialogResult.OK)
            {
                button4.BackColor = colorDialog4.Color;
                foreach (Control control in this.Controls)
                {
                    if (control.GetType() != typeof(PictureBox))
                    {
                        control.ForeColor = colorDialog4.Color;
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (colorDialog5.ShowDialog() == DialogResult.OK)
            {
                button5.BackColor = colorDialog5.Color;
                this.BackColor = colorDialog5.Color;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Font = fontDialog1.Font;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.BtnSaveColor = Color.FromArgb(128, 128, 255);
            //Properties.Settings.Default.BtnLoadColor = Color.FromArgb(128, 255, 128);
            //Properties.Settings.Default.BtnResetColor = Color.FromArgb(255, 128, 128);
            //Properties.Settings.Default.FormColor = Color.FromArgb(240, 240, 240);
            //Properties.Settings.Default.TextColor = Color.FromArgb(0, 0, 0);
            //Properties.Settings.Default.FormFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);

            EnigmaWindowsForms.Properties.Settings.Default.Reset();
            
            UpdateTheme();

            EnigmaWindowsForms.Properties.Settings.Default.Save();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            EnigmaWindowsForms.Properties.Settings.Default.BtnLoadColor = button2.BackColor;
            EnigmaWindowsForms.Properties.Settings.Default.BtnSaveColor = button1.BackColor;
            EnigmaWindowsForms.Properties.Settings.Default.BtnResetColor = button3.BackColor;
            EnigmaWindowsForms.Properties.Settings.Default.TextColor = button4.BackColor;
            EnigmaWindowsForms.Properties.Settings.Default.FormColor = button5.BackColor;
            EnigmaWindowsForms.Properties.Settings.Default.FormFont = this.Font;

            EnigmaWindowsForms.Properties.Settings.Default.Save();
        }
    }
}
