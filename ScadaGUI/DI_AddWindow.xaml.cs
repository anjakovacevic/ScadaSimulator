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
            this.DataContext = NewDI;
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
                            }
                        }
                    }
                    else
                    {            
                        // Add
                        IOContext.Instance.DigitalInputs.Add(NewDI);
                    }

                    IOContext.Instance.SaveChanges();
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
            bool retVal = true;
            var errors = new StringBuilder();

            errorMessage = errors.ToString();
            return retVal;
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
