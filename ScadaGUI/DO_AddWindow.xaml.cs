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
    /// Interaction logic for DO_AddWindow.xaml
    /// </summary>
    public partial class DO_AddWindow : Window
    {
        private DigitalOutput NewDI = new DigitalOutput();
        bool addOrUpdate;
        private int currentID = -1;

        public DO_AddWindow(DigitalOutput digitaOutput)
        {
            InitializeComponent();

            this.addrCmb.ItemsSource = new List<string> { "ADDR013", "ADDR014", "ADDR015", "ADDR016" };

            if (digitaOutput != null)
            {
                currentID = digitaOutput.ID;
                NewDI.Name = digitaOutput.Name;
                NewDI.Description = digitaOutput.Description;
                NewDI.Address = digitaOutput.Address;
                NewDI.Value = digitaOutput.Value;

                this.addrCmb.SelectedValue = digitaOutput.Address;

                this.Title = "Update DO";
                addOrUpdate = true;

                this.DataContext = NewDI;
            }
            else
            {
                this.Title = "Add DO";
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
                        foreach (DigitalOutput dout in IOContext.Instance.DigitalOutputs.Local)
                        {
                            if (dout.ID == currentID)
                            {
                                dout.Name = nameTxt.Text;
                                dout.Description = descTxt.Text;
                                dout.Address = addrCmb.Text;
                                dout.Value = Boolean.Parse(valTxt.Text);
                                IOContext.Instance.Entry(dout).State = System.Data.Entity.EntityState.Modified;
                                IOContext.Instance.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        // Add
                        DigitalOutput newDO = new DigitalOutput(this.nameTxt.Text, this.descTxt.Text, this.addrCmb.Text, this.valTxt.Text);
                        IOContext.Instance.DigitalOutputs.Add(newDO);
                        IOContext.Instance.SaveChanges();
                        newDO.Load();
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

            if (String.IsNullOrWhiteSpace(valTxt.Text))
            {
                valValTxt.Text = "Required field!";
                valTxt.BorderBrush = Brushes.Red;
                valValTxt.Visibility = Visibility.Visible;
                errors.AppendLine("Initial value is a required field.");
                isValid = false;
            }
            else
            {
                if (Boolean.TryParse(valTxt.Text, out bool result))
                {
                    valTxt.ClearValue(Border.BorderBrushProperty);
                    valValTxt.Visibility = Visibility.Hidden;
                }
                else
                {
                    valValTxt.Text = "Must be True/False!";
                    valTxt.BorderBrush = Brushes.Red;
                    valValTxt.Visibility = Visibility.Visible;
                    errors.AppendLine("Initial value must be True/False.");
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
