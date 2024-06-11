using DataConcentrator;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScadaGUI
{
    /// <summary>
    /// Interaction logic for Alarm_AddWindow.xaml
    /// </summary>
    public partial class Alarm_AddWindow : Window
    {
        public Alarm_AddWindow()
        {
            InitializeComponent();
            aiCmb.ItemsSource = IOContext.Instance.AnalogInputs.Local.Select(ai => ai.Name).ToList();
            priCmb.ItemsSource = new List<string> { "Low", "High" };
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string message))
            {
                try
                {
                    var selectedAI = IOContext.Instance.AnalogInputs.Local.FirstOrDefault(ai => ai.Name == aiCmb.Text);
                    if (selectedAI != null)
                    {
                        var newAlarm = new Alarm
                        {
                            Name = nameTxt.Text,
                            Message = messTxt.Text,
                            Value = double.Parse(valTxt.Text),
                            OnUpperVal = (bool)upCb.IsChecked,
                            HighPriority = priCmb.Text == "High",
                            TagId = selectedAI.ID,
                            TagName = selectedAI.Name
                        };

                        IOContext.Instance.Alarms.Add(newAlarm);
                        IOContext.Instance.SaveChanges();
                        this.Close();
                    }
                
                 }
                catch (DbEntityValidationException dbEx)
                {
                    var errorMessages = dbEx.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    var fullErrorMessage = string.Join("; ", errorMessages);
                    var exceptionMessage = string.Concat(dbEx.Message, " The validation errors are: ", fullErrorMessage);

                    MessageBox.Show(exceptionMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException;
                    while (innerException != null)
                    {
                        ex = innerException;
                        innerException = ex.InnerException;
                    }

                    MessageBox.Show($"An error occurred while saving changes: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(message, "Invalid input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ValidateInput(out string errorMessage)
        {
            bool isValid = true;
            var errors = new StringBuilder();

            // Validate Name
            if (string.IsNullOrWhiteSpace(messTxt.Text))
            {
                messValTxt.Text = "Required field!";
                messTxt.BorderBrush = Brushes.Red;
                messValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Name is required.");
                isValid = false;
            }
            else
            {
                nameTxt.ClearValue(Border.BorderBrushProperty);
                messValTxt.Visibility = Visibility.Hidden;
            }
            if (aiCmb.SelectedItem == null)
            {
                aiCmb.BorderBrush = Brushes.Red;
                errors.AppendLine("AI is required.");
                isValid = false;
            }
            else
            {
                aiCmb.ClearValue(Border.BorderBrushProperty);
            }
            if (priCmb.SelectedItem == null)
            {
                priCmb.BorderBrush = Brushes.Red;
                errors.AppendLine("Priority is required.");
                isValid = false;
            }
            else
            {
                priCmb.ClearValue(Border.BorderBrushProperty);
            }
            if (String.IsNullOrWhiteSpace(valTxt.Text))
            {
                valValTxt.Text = "Required field!";
                valTxt.BorderBrush = Brushes.Red;
                valValTxt.Visibility = Visibility.Visible;

                isValid = false;
            }
            else
            {
                if (Double.TryParse(valTxt.Text, out double result))
                {
                    valTxt.ClearValue(Border.BorderBrushProperty);
                    valValTxt.Visibility = Visibility.Hidden;
                }
                else
                {
                    valValTxt.Text = "Not a number!";
                    valTxt.BorderBrush = Brushes.Red;
                    valValTxt.Visibility = Visibility.Visible;
                    isValid = false;
                }
            }

            errorMessage = errors.ToString();
            return isValid;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult value = MessageBox.Show("Are you sure?", "Cancel Operation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
