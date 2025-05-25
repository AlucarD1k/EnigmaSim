using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnigmaSim;

namespace EnigmaWindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            textBox3.Text = "A";
            textBox4.Text = "B";
            textBox5.Text = "C";
        }
        RotorType Rotor1;
        RotorType Rotor2;
        RotorType Rotor3;
        ReflectorType Reflector;

        private void button1_Click(object sender, EventArgs e)
        {

            Enigma en = new Enigma();



            //Plugboard
            //en.Plugboard.Add('X', 'D');
            //en.Plugboard.Add('A', 'V');

            //en.Plugboard.Add('D', 'X');
            //en.Plugboard.Add('V', 'A');
            try
            {
                //Rotors
                if (textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                    throw new Exception("Выберите начальное положение роторов");
                en.Rotors.Add(Rotor1, char.Parse(textBox3.Text));
                en.Rotors.Add(Rotor2, char.Parse(textBox4.Text));
                en.Rotors.Add(Rotor3, char.Parse(textBox5.Text));

                //Reflector
                en.Rotors.SetReflector(Reflector);

                string result = en.Encrypt(textBox1.Text);

                textBox2.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                Rotor1 = RotorType.Rotor_I;
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                Rotor1 = RotorType.Rotor_II;
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Rotor1 = RotorType.Rotor_III;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                Rotor2 = RotorType.Rotor_I;
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                Rotor2 = RotorType.Rotor_II;
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                Rotor2 = RotorType.Rotor_III;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0)
            {
                Rotor3 = RotorType.Rotor_I;
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                Rotor3 = RotorType.Rotor_II;
            }
            else if (comboBox3.SelectedIndex == 2)
            {
                Rotor3 = RotorType.Rotor_III;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 0)
            {
                Reflector = ReflectorType.UWK_A;
            }
            else if (comboBox4.SelectedIndex == 1)
            {
                Reflector = ReflectorType.UWK_B;
            }
            else if (comboBox4.SelectedIndex == 2)
            {
                Reflector = ReflectorType.UWK_C;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox3.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.KeyChar = '\n';
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox4.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.KeyChar = '\n';
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox5.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.KeyChar = '\n';
        }
    }
}
