using System.Globalization;
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
using Calculator.Helpers;

namespace Calculator;

public partial class MainWindow : Window
{
    private const int MaxDigits = 15;
    private readonly string DecimalSeparator = ",";

    private decimal FirstValue { get; set; }

    IOperation Operation;
    
    public MainWindow()
    {
        InitializeComponent();
        buttonSum.Tag = new Sum();
        buttonSubstract.Tag = new Subtract();
        buttonMultiply.Tag = new Multiply();
        buttonDivide.Tag = new Divide();
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
    
    private void buttonNegate_Click(object sender, RoutedEventArgs e)
    {
        string current = numberInput.Text.Replace(" ", "");

        if (current == "0" || string.IsNullOrEmpty(current))
        {
            numberInput.Text = "-";
            return;
        }

        bool isNegative = current.StartsWith("-");

        if (isNegative)
            current = current.Substring(1);
        else
            current = "-" + current;

        numberInput.Text = current;
    }
    
    private void buttonPoint_Click(object sender, RoutedEventArgs e)
    {
        if (!numberInput.Text.Contains(DecimalSeparator))
        {
            SendToInput(DecimalSeparator);
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

            string[] parts = textBox.Text.Split(new[] { DecimalSeparator }, StringSplitOptions.None);

            bool isNegative = parts[0].StartsWith("-");
            string integerPart = isNegative ? parts[0].Substring(1) : parts[0];
            string decimalPart = parts.Length > 1 ? parts[1] : "";

            string rawInteger = new string(integerPart.Where(char.IsDigit).ToArray());
            if (rawInteger.Length > MaxDigits)
                rawInteger = rawInteger.Substring(0, MaxDigits);

            string formattedInteger = string.Join(" ",
                Enumerable.Range(0, rawInteger.Length)
                    .Reverse()
                    .GroupBy(i => i / 3)
                    .Select(g => new string(g.Select(i => rawInteger[i]).Reverse().ToArray()))
                    .Reverse());

            if (isNegative)
                formattedInteger = "-" + formattedInteger;

            string formattedNumber = formattedInteger;
            if (parts.Length > 1 || textBox.Text.EndsWith(DecimalSeparator))
            {
                formattedNumber += DecimalSeparator + decimalPart;
            }

            textBox.Text = formattedNumber;

            textBox.SelectionStart = Math.Min(formattedNumber.Length, cursorPosition);

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

    public void buttonClearAll_Click(object sender, RoutedEventArgs e)
    {
        FirstValue = 0;
        Operation = null;
        numberInput.Text = "0";
    }

    public void Window_PreviewTextNumberInput(object sender, TextCompositionEventArgs e)
    {
        switch (e.Text)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                SendToInput(e.Text);
                break;

            case "*":
                buttonMultiply.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                break;

            case "-":
                buttonSubstract.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                break;

            case "+":
                buttonSum.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                break;

            case "/":
                buttonDivide.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                break;

            case "=":
                buttonEquals.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                break;

            default:
                if (e.Text == DecimalSeparator) 
                    buttonPoint.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                else if (e.Text[0] == (char)8) 
                    buttonRemove.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                else if (e.Text[0] == (char)13) 
                    buttonEquals.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                break;
        }

        buttonEquals.Focus();
    }
    
    private void Operator_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is IOperation selectedOperation)
        {
            try
            {
                string input = numberInput.Text.Replace(" ", "");
                FirstValue = decimal.Parse(input, CultureInfo.GetCultureInfo("pl-PL"));
                Operation = selectedOperation;

                numberInput.Text = "0"; 
            }
            catch
            {
                numberInput.Text = "Błąd";
            }
        }
    }
    
    private void buttonEquals_Click(object sender, RoutedEventArgs e)
    {
        if (Operation == null)
            return;

        try
        {
            string input = numberInput.Text.Replace(" ", "");
            decimal secondValue = decimal.Parse(input, CultureInfo.GetCultureInfo("pl-PL"));

            decimal result = Operation.Operation(FirstValue, secondValue);
            numberInput.Text = result.ToString(CultureInfo.GetCultureInfo("pl-PL"));
            Operation = null; 
        }
        catch
        {
            numberInput.Text = "Błąd";
        }
    }
    
    private void buttonPercent_Click(object sender, RoutedEventArgs e)
    {
        if (Operation == null)
            return;

        try
        {
            string input = numberInput.Text.Replace(" ", "");
            decimal percentValue = decimal.Parse(input, CultureInfo.GetCultureInfo("pl-PL"));

            decimal secondValue = FirstValue * (percentValue / 100);

            decimal result = Operation.Operation(FirstValue, secondValue);
            numberInput.Text = result.ToString(CultureInfo.GetCultureInfo("pl-PL"));

            Operation = null;
        }
        catch
        {
            numberInput.Text = "Błąd";
        }
    }
}