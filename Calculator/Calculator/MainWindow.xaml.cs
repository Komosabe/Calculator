using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdonisUI;

namespace Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int MaxDigits = 15;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void buttonNumber_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string number = button.Content.ToString() ?? "";
            if (!string.IsNullOrEmpty(number))
                SendToInput(number);
        }
    }

    private void SendToInput(string number)
    {
        if (numberInput.Text == "0")
            numberInput.Text = "";

        numberInput.Text = $"{numberInput.Text}{number}";
    }

    private void numberInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.TextChanged -= numberInput_TextChanged; 

            int cursorPosition = textBox.SelectionStart;

            string rawNumber = new string(textBox.Text.Where(char.IsDigit).ToArray());

            if (rawNumber.Length > MaxDigits)
                rawNumber = rawNumber.Substring(0, MaxDigits);
            
            string formattedNumber = string.Join(" ", 
                Enumerable.Range(0, rawNumber.Length)
                    .Reverse()
                    .GroupBy(i => i / 3)
                    .Select(g => new string(g.Select(i => rawNumber[i]).Reverse().ToArray()))
                    .Reverse());

            textBox.Text = formattedNumber;

            int spacesBeforeCursor = formattedNumber.Take(cursorPosition).Count(c => c == ' ');
            int newCursorPosition = Math.Min(formattedNumber.Length, cursorPosition + spacesBeforeCursor);
            textBox.SelectionStart = newCursorPosition;

            textBox.TextChanged += numberInput_TextChanged; 
        }
    }

    public void buttonRemove_Click(object sender, RoutedEventArgs e)
    {
        if (numberInput.Text == "0")
            return;

        numberInput.Text = numberInput.Text.Substring(0, numberInput.Text.Length - 1);

        if (numberInput.Text == "")
            numberInput.Text = "0";
    }
}