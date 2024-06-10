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
    /// Interaction logic for AO_AddWindow.xaml
    /// </summary>
    public partial class AO_AddWindow : Window
    {
        public AnalogOutput NewAO = new AnalogOutput();
        bool addOrUpdate;
        private int currentID = -1;
        public AO_AddWindow(AnalogOutput analogOutput)
        {
            InitializeComponent();
            this.addrCmb.ItemsSource = new List<string> { "ADDR005", "ADDR006", "ADDR007", "ADDR008" };

            if (analogOutput != null)
            {
                currentID = analogOutput.ID;
                NewAO.Name = analogOutput.Name;
                NewAO.Description = analogOutput.Description;
                NewAO.Address = analogOutput.Address;
                NewAO.HighLimit = analogOutput.HighLimit;
                NewAO.LowLimit = analogOutput.LowLimit;
                NewAO.Units = analogOutput.Units;
                NewAO.Value = analogOutput.Value;
                    
                this.addrCmb.SelectedValue = analogOutput.Address;

                this.Title = "Update AO";
                addOrUpdate = true;

                this.DataContext = NewAO;
            }
            else
            {
                this.Title = "Add AO";
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
                        foreach (AnalogOutput ao in IOContext.Instance.AnalogOutputs.Local)
                        {
                            if (ao.ID == currentID)
                            {
                                ao.Name = nameTxt.Text;
                                ao.Description = descTxt.Text;
                                ao.Address = addrCmb.Text;
                                ao.HighLimit = Double.Parse(upTxt.Text);
                                ao.LowLimit = Double.Parse(lowTxt.Text);
                                ao.Units = unitTxt.Text;
                                double val = Double.Parse(valTxt.Text);
                                if (val < ao.LowLimit)
                                {
                                    ao.Value = ao.LowLimit;
                                }
                                else if (val > ao.HighLimit)
                                {
                                    ao.Value = ao.HighLimit;
                                }
                                else
                                {
                                    ao.Value = val;
                                }
                                IOContext.Instance.Entry(ao).State = System.Data.Entity.EntityState.Modified;
                                IOContext.Instance.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        // Add
                        NewAO.Name = this.nameTxt.Text;
                        NewAO.Description = this.descTxt.Text;
                        NewAO.Address = this.addrCmb.Text;
                        NewAO.InitialValue = Double.Parse(this.valTxt.Text);
                        NewAO.LowLimit = Double.Parse(this.lowTxt.Text);
                        NewAO.HighLimit = Double.Parse(this.upTxt.Text);
                        NewAO.Units = this.unitTxt.Text;
                        if (NewAO.InitialValue > NewAO.HighLimit)
                        {
                            NewAO.Value = NewAO.HighLimit;
                        }
                        else if (NewAO.InitialValue < NewAO.LowLimit)
                        {
                            NewAO.Value = NewAO.LowLimit;
                        }   
                        else
                        {
                            NewAO.Value = NewAO.InitialValue;
                        }
                        IOContext.Instance.AnalogOutputs.Add(NewAO);
                        IOContext.Instance.SaveChanges();
                        NewAO.Load();
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

            // Validate nameTxt
            if (String.IsNullOrWhiteSpace(nameTxt.Text))
            {
                nameValTxt.Text = "Required field!";
                nameTxt.BorderBrush = Brushes.Red;
                nameValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Name is a required field.");
                isValid = false;
            }
            else
            {
                nameTxt.ClearValue(Border.BorderBrushProperty);
                nameValTxt.Visibility = Visibility.Hidden;
            }

            // Validate addrCmb
            if (addrCmb.SelectedItem == null)
            {
                addrCmb.BorderBrush = Brushes.Red;
                errors.AppendLine("Address is a required field.");
                isValid = false;
            }
            else
            {
                addrCmb.ClearValue(Border.BorderBrushProperty);
            }

            // Validate valTxt
            
            if (String.IsNullOrWhiteSpace(valTxt.Text))
            {
                valValTxt.Text = "Required field!";
                valTxt.BorderBrush = Brushes.Red;
                valValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Value is a required field.");
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
                    errors.AppendLine("Value must be a number.");
                    isValid = false;
                }
            }

            // Validate lowTxt
            if (String.IsNullOrWhiteSpace(lowTxt.Text))
            {
                lowValTxt.Text = "Required field!";
                lowTxt.BorderBrush = Brushes.Red;
                lowValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Lower limit is a required field.");
                isValid = false;
            }
            else
            {
                if (Double.TryParse(lowTxt.Text, out double lowResult))
                {
                    lowTxt.ClearValue(Border.BorderBrushProperty);
                    lowValTxt.Visibility = Visibility.Hidden;
                }
                else
                {
                    lowValTxt.Text = "Not a number!";
                    lowTxt.BorderBrush = Brushes.Red;
                    lowValTxt.Visibility = Visibility.Visible;
                    errors.AppendLine("Lower limit must be a number.");
                    isValid = false;
                }
            }

            // Validate upTxt
            if (String.IsNullOrWhiteSpace(upTxt.Text))
            {
                upValTxt.Text = "Required field!";
                upTxt.BorderBrush = Brushes.Red;
                upValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Upper limit is a required field.");
                isValid = false;
            }
            else
            {
                if (Double.TryParse(upTxt.Text, out double upResult))
                {
                    upTxt.ClearValue(Border.BorderBrushProperty);
                    upValTxt.Visibility = Visibility.Hidden;
                }
                else
                {
                    upValTxt.Text = "Not a number!";
                    upTxt.BorderBrush = Brushes.Red;
                    upValTxt.Visibility = Visibility.Visible;
                    errors.AppendLine("Upper limit must be a number.");
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
