using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace lab01
{
    public interface ICalculator
    {
        string ReplaceSubstring(string input, string oldSubstring, string newSubstring);
        string RemoveSubstring(string input, string substring);
        char GetCharacterAtIndex(string input, int index);
        int GetLength(string input);
        int CountVowels(string input);
        int CountConsonants(string input);
        int CountSentences(string input);
        int CountWords(string input);
    }

    public partial class Form1 : Form
    {
        private readonly ICalculator _calculator;

        public Form1()
        {
            InitializeComponent();
            _calculator = new Calculator();
            ((Calculator)_calculator).OperationCompleted += Calculator_OperationCompleted;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                string oldSubstring = txtOldSubstring.Text;
                string newSubstring = txtNewSubstring.Text;
                string result = _calculator.ReplaceSubstring(input, oldSubstring, newSubstring);
                lblResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                string substring = txtSubstring.Text;
                string result = _calculator.RemoveSubstring(input, substring);
                lblResult.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetChar_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int index = int.Parse(txtIndex.Text);
                char result = _calculator.GetCharacterAtIndex(input, index);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLength_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int result = _calculator.GetLength(input);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVowels_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int result = _calculator.CountVowels(input);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConsonants_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int result = _calculator.CountConsonants(input);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSentences_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int result = _calculator.CountSentences(input);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWords_Click(object sender, EventArgs e)
        {
            try
            {
                string input = txtInput.Text;
                int result = _calculator.CountWords(input);
                lblResult.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Calculator_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            MessageBox.Show($"{e.OperationName}", "Operation Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblOldSubstring_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public class Calculator : ICalculator
    {
        public event OperationCompletedEventHandler OperationCompleted;

        public string ReplaceSubstring(string input, string oldSubstring, string newSubstring)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldSubstring))
                throw new ArgumentException("Input or old substring cannot be null or empty.");

            string result = input.Replace(oldSubstring, newSubstring);
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public string RemoveSubstring(string input, string substring)
            {
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
                    throw new ArgumentException("Input or substring cannot be null or empty.");

                string pattern = @"\b" + Regex.Escape(substring) + @"\b";
                string result = Regex.Replace(input, pattern, string.Empty);

                OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
                return result;
            }


    public char GetCharacterAtIndex(string input, int index)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            if (index < 0 || index >= input.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

            char result = input[index];
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public int GetLength(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            int result = input.Length;
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public int CountVowels(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            char[] vowels = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я' };
            int result = input.Count(c => vowels.Contains(c));
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public int CountConsonants(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            char[] consonants = "бвгджзйклмнпрстфхцчшщъьБВГДЖЗЙКЛМНПРТФХЦЧШЩЪЬ".ToCharArray();
            int result = input.Count(c => consonants.Contains(c));
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public int CountSentences(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            int result = Regex.Split(input, @"(?<=[\.!\?])\s+").Length;
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        public int CountWords(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            int result = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            OnOperationCompleted(new OperationCompletedEventArgs("Рабочая тема (❁´◡`❁)"));
            return result;
        }

        protected virtual void OnOperationCompleted(OperationCompletedEventArgs e)
        {
            OperationCompleted?.Invoke(this, e);
        }
    }

    public delegate void OperationCompletedEventHandler(object sender, OperationCompletedEventArgs e);

    public class OperationCompletedEventArgs : EventArgs
    {
        public string OperationName { get; }

        public OperationCompletedEventArgs(string operationName)
        {
            OperationName = operationName;
        }
    }
}
