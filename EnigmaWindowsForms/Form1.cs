using System;
using System.Collections.Generic;
using System.Drawing;
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

        private Dictionary<char, Button> plugboardButtons = new Dictionary<char, Button>();
        private char? selectedPlugboardChar = null;
        // Словарь для пар: A->B, B->A, ...
        private Dictionary<char, char> plugboardPairs = new Dictionary<char, char>();

        // Массив из 13 ярких цветов для пар
        private static readonly Color[] PairColors = new Color[]
        {
            Color.LightBlue, Color.LightGreen, Color.LightPink, Color.LightSalmon,
            Color.LightGoldenrodYellow, Color.LightCoral, Color.LightCyan,
            Color.LightSkyBlue, Color.MediumPurple, Color.PaleTurquoise,
            Color.Wheat, Color.Khaki, Color.Plum
        };

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
            InitPlugboardButtons();
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

            enigmaInstance.Plugboard.Clear();

            var used = new HashSet<char>();
            foreach (var kv in plugboardPairs)
            {
                char a = char.ToUpper(kv.Key);
                char b = char.ToUpper(kv.Value);
                if (!used.Contains(a) && !used.Contains(b))
                {
                    // Используем метод Add, реализованный в Plugboard
                    try
                    {
                        enigmaInstance.Plugboard.Add(a, b);
                        used.Add(a);
                        used.Add(b);
                    }
                    catch (Exception ex)
                    {
                        // Если что-то не так, например, дублирование — покажем сообщение
                        MessageBox.Show("Ошибка при добавлении пары на коммутационной панели: " + ex.Message);
                    }
                }
            }
        }

        private void InitPlugboardButtons()
        {
            // button6 - button31 — это кнопки A-Z (button6 -> 'A', button7 -> 'B', ...)
            for (int i = 0; i < 26; i++)
            {
                char letter = (char)('A' + i);

                Button btn = this.Controls.Find($"button{6 + i}", true)[0] as Button;
                btn.Tag = letter;
                btn.Text = letter.ToString();
                btn.BackColor = Color.LightGray;
                btn.Click += PlugboardButton_Click;
                plugboardButtons[letter] = btn;
            }

            RefreshPlugboardButtons();
        }

        private void PlugboardButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            char letter = (char)btn.Tag;

            if (selectedPlugboardChar == null)
            {
                // Первый клик: если буква уже соединена - разорвать; иначе - выделить
                if (plugboardPairs.ContainsKey(letter))
                {
                    char paired = plugboardPairs[letter];
                    plugboardPairs.Remove(letter);
                    plugboardPairs.Remove(paired);
                    selectedPlugboardChar = null;
                }
                else
                {
                    selectedPlugboardChar = letter;
                }
            }
            else
            {
                if (selectedPlugboardChar == letter)
                {
                    selectedPlugboardChar = null; // снимаем выделение
                }
                else if (plugboardPairs.ContainsKey(letter))
                {
                    // клик по уже соединённой — разорвать
                    char paired = plugboardPairs[letter];
                    plugboardPairs.Remove(letter);
                    plugboardPairs.Remove(paired);
                    selectedPlugboardChar = null;
                }
                else
                {
                    // связываем две буквы
                    plugboardPairs[selectedPlugboardChar.Value] = letter;
                    plugboardPairs[letter] = selectedPlugboardChar.Value;
                    selectedPlugboardChar = null;
                }
            }
            RefreshPlugboardButtons();
        }

        private void RefreshPlugboardButtons()
        {
            // Определяем уникальные пары и присваиваем каждой свой цвет
            var pairList = new List<Tuple<char, char>>();
            var visited = new HashSet<char>();

            foreach (var kv in plugboardPairs)
            {
                char a = kv.Key;
                char b = kv.Value;
                if (a < b && !visited.Contains(a) && !visited.Contains(b))
                {
                    pairList.Add(Tuple.Create(a, b));
                    visited.Add(a);
                    visited.Add(b);
                }
            }

            // Для каждой буквы вычисляем её цвет (индекс пары)
            var colorIndexes = new Dictionary<char, int>();
            for (int i = 0; i < pairList.Count; i++)
            {
                colorIndexes[pairList[i].Item1] = i;
                colorIndexes[pairList[i].Item2] = i;
            }

            foreach (var kv in plugboardButtons)
            {
                char letter = kv.Key;
                Button btn = kv.Value;

                if (selectedPlugboardChar != null && selectedPlugboardChar == letter)
                {
                    btn.BackColor = Color.Yellow;
                }
                else if (colorIndexes.ContainsKey(letter))
                {
                    btn.BackColor = PairColors[colorIndexes[letter] % PairColors.Length];
                }
                else
                {
                    btn.BackColor = Color.LightGray;
                }
            }
        }

        // Кнопка "Сброс" - сбрасывает машину и очищает результаты (создайте такую кнопку в форме)
        private void buttonReset_Click(object sender, EventArgs e)
        {
            isInitializing = true;
            InitializeEnigma();
            UpdateRotorHeadsInTextBoxes();
            textBox1.Text = "";
            isInitializing = false;
        }

        // Реальное шифрование по мере ввода
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            try
            {
                if (textBox1.Text.Length == 0)
                {
                    label7.Text = textBox3.Text; // Первый ротор
                    label8.Text = textBox4.Text; // Второй ротор
                    label9.Text = textBox5.Text; // Третий ротор
                }
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

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save Enigma settings";
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string rotor1Type = Rotor1.ToString();
                string rotor1Pos = textBox3.Text;
                string rotor2Type = Rotor2.ToString();
                string rotor2Pos = textBox4.Text;
                string rotor3Type = Rotor3.ToString();
                string rotor3Pos = textBox5.Text;
                string reflectorType = Reflector.ToString();

                string settingsLine = $"{rotor1Type} {rotor1Pos} {rotor2Type} {rotor2Pos} {rotor3Type} {rotor3Pos} {reflectorType}";

                System.IO.File.WriteAllText(saveFileDialog1.FileName, settingsLine);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.Title = "Load Enigma settings";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string settingsLine = System.IO.File.ReadAllText(openFileDialog1.FileName).Trim();
                string[] parts = settingsLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 7)
                {

                    try
                    {
                        // Установить типы роторов и рефлектора обновить SelectedIndex комбобоксов 
                        comboBox1.SelectedIndex = comboBox1.Items.IndexOf(((RotorType)Enum.Parse(typeof(RotorType), parts[0])).ToString());
                        textBox3.Text = parts[1];
                        comboBox2.SelectedIndex = comboBox2.Items.IndexOf(((RotorType)Enum.Parse(typeof(RotorType), parts[2])).ToString());
                        textBox4.Text = parts[3];
                        comboBox3.SelectedIndex = comboBox3.Items.IndexOf(((RotorType)Enum.Parse(typeof(RotorType), parts[4])).ToString());
                        textBox5.Text = parts[5];
                        comboBox4.SelectedIndex = comboBox4.Items.IndexOf(((ReflectorType)Enum.Parse(typeof(ReflectorType), parts[6])).ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при загрузке настроек: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный формат файла настроек.");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.Title = "Load Enigma input";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string input = System.IO.File.ReadAllText(openFileDialog1.FileName).Trim();
                    textBox1.Text = input;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке сообщения: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Save Enigma output";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string output = textBox2.Text;
                System.IO.File.WriteAllText(saveFileDialog1.FileName, output);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog1.Title = "Сохранить настройки панели";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Собираем уникальные пары (буквы идут по алфавиту)
                var pairs = new HashSet<string>();
                foreach (var kv in plugboardPairs)
                {
                    char a = kv.Key;
                    char b = kv.Value;
                    if (a < b)
                        pairs.Add($"{a} {b}");
                }
                System.IO.File.WriteAllLines(saveFileDialog1.FileName, pairs);
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog1.Title = "Загрузить настройки панели";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var lines = System.IO.File.ReadAllLines(openFileDialog1.FileName);

                // Очистить текущие пары
                plugboardPairs.Clear();

                foreach (string line in lines)
                {
                    var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        char a = char.ToUpper(parts[0][0]);
                        char b = char.ToUpper(parts[1][0]);
                        // Не добавлять, если буква уже в паре
                        if (!plugboardPairs.ContainsKey(a) && !plugboardPairs.ContainsKey(b) && a != b)
                        {
                            plugboardPairs[a] = b;
                            plugboardPairs[b] = a;
                        }
                    }
                }
                RefreshPlugboardButtons();
            }
        }
    }
}