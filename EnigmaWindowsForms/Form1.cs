using System;
using System.Windows.Forms;
using EnigmaSim;

namespace EnigmaWindowsForms
{
    public partial class Form1 : Form
    {
        // Выбранные настройки роторов и рефлектора
        RotorType Rotor1;
        RotorType Rotor2;
        RotorType Rotor3;
        ReflectorType Reflector;

        // Главная инстанция машины Enigma для текущей сессии ввода
        private Enigma enigmaInstance;

        // Текст, который уже был зашифрован (для отслеживания изменений)
        private string lastInput = "";
        // Защита от рекурсии при программном изменении полей
        private bool isInitializing = false;

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
            // Привязка обработчика (можно также сделать через дизайнер)
            
        }

        private void InitializeEnigma()
        {
            enigmaInstance = new Enigma();
            enigmaInstance.Rotors.Clear();

            // Установка выбранных пользователем роторов и их начальных позиций
            if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
                throw new Exception("Выберите начальное положение роторов");

            enigmaInstance.Rotors.Add(Rotor1, char.Parse(textBox3.Text));
            enigmaInstance.Rotors.Add(Rotor2, char.Parse(textBox4.Text));
            enigmaInstance.Rotors.Add(Rotor3, char.Parse(textBox5.Text));
            enigmaInstance.Rotors.SetReflector(Reflector);

            lastInput = "";
            textBox2.Text = "";
        }

        // Кнопка "Сброс" - сбрасывает машину и очищает результаты (создайте такую кнопку в форме)
        private void buttonReset_Click(object sender, EventArgs e)
        {
            isInitializing = true;
            InitializeEnigma();
            UpdateRotorHeadsInTextBoxes();
            isInitializing = false;
        }

        // Реальное шифрование по мере ввода
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            try
            {
                // Всегда пересоздаём машину и прогоняем весь текст
                InitializeEnigma();

                string input = textBox1.Text;
                textBox2.Text = "";

                foreach (char c in input)
                {
                    if (char.IsLetter(c))
                    {
                        char encrypted = enigmaInstance.Encrypt(c.ToString())[0];
                        textBox2.Text += encrypted;
                        UpdateRotorHeadsInTextBoxes();
                    }
                    else
                    {
                        textBox2.Text += c;
                    }
                }

                lastInput = input;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateRotorHeadsInTextBoxes()
        {
            var rotors = enigmaInstance.Rotors.List;
            if (rotors != null && rotors.Count >= 3)
            {
                label7.Text = rotors[0].Current.ToString(); // Первый ротор
                label8.Text = rotors[1].Current.ToString(); // Второй ротор
                label9.Text = rotors[2].Current.ToString(); // Третий ротор
            }
        }

        // Выбор типа ротора 1
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: Rotor1 = RotorType.Rotor_I; break;
                case 1: Rotor1 = RotorType.Rotor_II; break;
                case 2: Rotor1 = RotorType.Rotor_III; break;
            }
        }

        // Выбор типа ротора 2
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0: Rotor2 = RotorType.Rotor_I; break;
                case 1: Rotor2 = RotorType.Rotor_II; break;
                case 2: Rotor2 = RotorType.Rotor_III; break;
            }
        }

        // Выбор типа ротора 3
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox3.SelectedIndex)
            {
                case 0: Rotor3 = RotorType.Rotor_I; break;
                case 1: Rotor3 = RotorType.Rotor_II; break;
                case 2: Rotor3 = RotorType.Rotor_III; break;
            }
        }

        // Выбор рефлектора
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox4.SelectedIndex)
            {
                case 0: Reflector = ReflectorType.UWK_A; break;
                case 1: Reflector = ReflectorType.UWK_B; break;
                case 2: Reflector = ReflectorType.UWK_C; break;
            }
        }

        // Валидация ввода для начальных букв роторов (A-Z, только одна буква)
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox3.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.Handled = true;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox4.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.Handled = true;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'A' && e.KeyChar <= 'Z') && textBox5.Text.Length == 0)
                return;
            if (e.KeyChar == (char)Keys.Back)
                return;
            e.Handled = true;
        }
    }
}