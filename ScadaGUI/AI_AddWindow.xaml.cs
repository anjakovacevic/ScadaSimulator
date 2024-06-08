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
    /// Interaction logic for AI_AddWindow.xaml
    /// </summary>
    public partial class AI_AddWindow : Window
    {
        public AnalogInput NewAI = new AnalogInput();
        bool addOrUpdate;
        private int currentID = -1;
        public AI_AddWindow(AnalogInput analogInput)
        {
            InitializeComponent();
            this.addrCmb.ItemsSource = new List<string> { "ADDR001", "ADDR002", "ADDR003", "ADDR004" };

            if (analogInput != null)
            {
                currentID = analogInput.ID;
                NewAI.Name = analogInput.Name;
                NewAI.Description = analogInput.Description;
                NewAI.Address = analogInput.Address;
                NewAI.HighLimit = analogInput.HighLimit;
                NewAI.LowLimit = analogInput.LowLimit;
                NewAI.ScanTime = analogInput.ScanTime;
                NewAI.Units = analogInput.Units;
                NewAI.Value = analogInput.Value;

                this.addrCmb.SelectedValue = analogInput.Address;

                this.Title = "Update AI";
                addOrUpdate = true;

                this.DataContext = NewAI;
            }
            else
            {
                this.Title = "Add AI";
                addOrUpdate = false;
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string message))
            {
                try
                {
                    if (addOrUpdate)
                    {
                        // Update
                        foreach (AnalogInput ai in IOContext.Instance.AnalogInputs.Local)
                        {
                            if (ai.ID == currentID)
                            {
                                ai.Name = nameTxt.Text;
                                ai.Description = descTxt.Text;
                                ai.Address = addrCmb.Text;
                                ai.ScanTime = Double.Parse(scanTxt.Text);
                                ai.HighLimit = Double.Parse(upTxt.Text);
                                ai.LowLimit = Double.Parse(lowTxt.Text);
                                ai.Units = unitTxt.Text;
                                IOContext.Instance.Entry(ai).State = System.Data.Entity.EntityState.Modified;
                                IOContext.Instance.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        // Add
                        NewAI.Name = this.nameTxt.Text;
                        NewAI.Description = this.descTxt.Text;
                        NewAI.Address = this.addrCmb.Text;
                        NewAI.ScanTime = Double.Parse(this.scanTxt.Text);
                        NewAI.LowLimit = Double.Parse(this.lowTxt.Text);
                        NewAI.HighLimit = Double.Parse(this.upTxt.Text);
                        NewAI.Units = this.unitTxt.Text;
                        NewAI.Alarming = 0;
                        IOContext.Instance.AnalogInputs.Add(NewAI);
                        IOContext.Instance.SaveChanges();
                        NewAI.Load();
                    }
                    this.Close();
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

        // Error handling
        private bool ValidateInput(out string errorMessage)
        {
            bool isValid = true;
            var errors = new StringBuilder();

            // Validate Name
            if (string.IsNullOrWhiteSpace(nameTxt.Text))
            {
                nameValTxt.Text = "Required field!";
                nameTxt.BorderBrush = Brushes.Red;
                nameValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Name is required.");
                isValid = false;
            }
            else
            {
                nameTxt.ClearValue(Border.BorderBrushProperty);
                nameValTxt.Visibility = Visibility.Hidden;
            }

            // Validate Address
            if (addrCmb.SelectedItem == null)
            {
                addrCmb.BorderBrush = Brushes.Red;
                errors.AppendLine("Address is required.");
                isValid = false;
            }
            else
            {
                addrCmb.ClearValue(Border.BorderBrushProperty);
            }

            // Validate Scan Time
            if (string.IsNullOrWhiteSpace(scanTxt.Text))
            {
                scanValTxt.Text = "Required field!";
                scanTxt.BorderBrush = Brushes.Red;
                scanValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Scan Time is required.");
                isValid = false;
            }
            else if (!double.TryParse(scanTxt.Text, out double scanTime) || scanTime <= 0)
            {
                scanValTxt.Text = "Not a number or must be positive!";
                scanTxt.BorderBrush = Brushes.Red;
                scanValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Valid Scan Time is required and must be a positive number.");
                isValid = false;
            }
            else
            {
                scanTxt.ClearValue(Border.BorderBrushProperty);
                scanValTxt.Visibility = Visibility.Hidden;
            }

            // Validate Low Limit
            if (string.IsNullOrWhiteSpace(lowTxt.Text))
            {
                lowValTxt.Text = "Required field!";
                lowTxt.BorderBrush = Brushes.Red;
                lowValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Low Limit is required.");
                isValid = false;
            }
            else if (!double.TryParse(lowTxt.Text, out double lowLimit))
            {
                lowValTxt.Text = "Not a number!";
                lowTxt.BorderBrush = Brushes.Red;
                lowValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Valid Low Limit is required.");
                isValid = false;
            }
            else
            {
                lowTxt.ClearValue(Border.BorderBrushProperty);
                lowValTxt.Visibility = Visibility.Hidden;
            }

            // Validate High Limit
            if (string.IsNullOrWhiteSpace(upTxt.Text))
            {
                upValTxt.Text = "Required field!";
                upTxt.BorderBrush = Brushes.Red;
                upValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("High Limit is required.");
                isValid = false;
            }
            else if (!double.TryParse(upTxt.Text, out double highLimit))
            {
                upValTxt.Text = "Not a number!";
                upTxt.BorderBrush = Brushes.Red;
                upValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Valid High Limit is required.");
                isValid = false;
            }
            else
            {
                upTxt.ClearValue(Border.BorderBrushProperty);
                upValTxt.Visibility = Visibility.Hidden;
            }

            // Validate that Low Limit is less than High Limit
            if (isValid && Double.Parse(this.lowTxt.Text) >= Double.Parse(this.upTxt.Text))
            {
                lowValTxt.Text = "Low Limit must be less than High Limit.";
                lowTxt.BorderBrush = Brushes.Red;
                lowValTxt.Visibility = Visibility.Visible;
                upTxt.BorderBrush = Brushes.Red;
                upValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Low Limit must be less than High Limit.");
                isValid = false;
            }
            else
            {
                lowTxt.ClearValue(Border.BorderBrushProperty);
                lowValTxt.Visibility = Visibility.Hidden;
                upTxt.ClearValue(Border.BorderBrushProperty);
                upValTxt.Visibility = Visibility.Hidden;
            }

            // Validate Units
            if (string.IsNullOrWhiteSpace(unitTxt.Text))
            {
                errors.AppendLine("Units are required.");
                unitTxt.BorderBrush = Brushes.Red;
                unitValTxt.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                unitTxt.ClearValue(Border.BorderBrushProperty);
                unitValTxt.Visibility = Visibility.Hidden;
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
