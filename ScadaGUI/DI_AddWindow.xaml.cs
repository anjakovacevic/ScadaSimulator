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
    /// Interaction logic for DI_AddWindow.xaml
    /// </summary>
    public partial class DI_AddWindow : Window
    {
        private DigitalInput NewDI = new DigitalInput();
        bool addOrUpdate;
        private int currentID = -1;

        public DI_AddWindow(DigitalInput digitalInput)
        { 
            InitializeComponent();         
            
            this.addrCmb.ItemsSource = new List<string> { "ADDR009", "ADDR010", "ADDR011", "ADDR012" };

            if (digitalInput != null)
            {
                currentID = digitalInput.ID;
                NewDI.Name = digitalInput.Name;
                NewDI.Description = digitalInput.Description;
                NewDI.Address = digitalInput.Address;
                NewDI.ScanTime = digitalInput.ScanTime;

                this.addrCmb.SelectedValue = digitalInput.Address;

                this.Title = "Update DI";
                addOrUpdate = true;

                this.DataContext = NewDI;
            }
            else
            {
                this.Title = "Add DI";
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
                        foreach (DigitalInput di in IOContext.Instance.DigitalInputs.Local)
                        {
                            if (di.ID == currentID)
                            {
                                di.Name = nameTxt.Text;
                                di.Description = descTxt.Text;
                                di.Address = addrCmb.Text;
                                di.ScanTime = Double.Parse(scanTxt.Text);
                                IOContext.Instance.Entry(di).State = System.Data.Entity.EntityState.Modified;
                                IOContext.Instance.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        // Add
                        DigitalInput newDI = new DigitalInput(this.nameTxt.Text, this.descTxt.Text, this.addrCmb.Text, Double.Parse(this.scanTxt.Text));
                        IOContext.Instance.DigitalInputs.Add(newDI);
                        IOContext.Instance.SaveChanges();
                        newDI.Load();
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
                errors.AppendLine("Name cannot be empty.");
                isValid = false;
                nameValTxt.Text = "Required field!";
                nameTxt.BorderBrush = Brushes.Red;
                nameValTxt.Visibility = Visibility.Visible;
            }
            else
            {
                nameTxt.ClearValue(Border.BorderBrushProperty);
                nameValTxt.Visibility = Visibility.Hidden;
            }

            // No validation needed for Description

            // Validate Address
            if (addrCmb.SelectedItem == null)
            {
                errors.AppendLine("Address must be selected.");
                isValid = false;
                addrCmb.BorderBrush = Brushes.Red;
            }
            else
            {
                addrCmb.ClearValue(Border.BorderBrushProperty);
            }

            // Validate Scan Time
            if (!double.TryParse(scanTxt.Text, out double scanTime) || scanTime <= 0)
            {
                errors.AppendLine("Scan Time must be a positive number.");
                isValid = false;
                scanValTxt.Text = "Required field!";
                scanTxt.BorderBrush = Brushes.Red;
                scanValTxt.Visibility = Visibility.Visible;
            }
            else
            {
                scanTxt.ClearValue(Border.BorderBrushProperty);
                scanValTxt.Visibility = Visibility.Hidden;
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
