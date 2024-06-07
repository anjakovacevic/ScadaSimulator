using DataConcentrator;
using System;
using System.Data.Entity;
using System.Windows;

namespace ScadaGUI
{
    public partial class MainWindow : Window
    {
        public AnalogInput SelectedAI { get; set; }
        public AnalogOutput SelectedAO { get; set; }
        public DigitalInput SelectedDI { get; set; }
        public DigitalOutput SelectedDO { get; set; }
        public Alarm SelectedAlarm { get; set; }
        public AlarmHistory SelectedHistory { get; set; }

        private int selectedTab;
        public int SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            IOContext.Instance.DigitalInputs.Load();
            IOContext.Instance.DigitalOutputs.Load();
            IOContext.Instance.AnalogInputs.Load();
            IOContext.Instance.AnalogOutputs.Load();
            IOContext.Instance.AlarmHistories.Load();
            IOContext.Instance.Alarms.Load();

            foreach (DigitalInput di in IOContext.Instance.DigitalInputs)
            {
                if (di.OnOffScan)
                {
                    di.Load();
                }
            }

            foreach (AnalogInput ai in IOContext.Instance.AnalogInputs)
            {
                if (ai.OnOffScan)
                {
                    ai.Load();
                }
            }

            DIGrid.ItemsSource = IOContext.Instance.DigitalInputs.Local;
            DOGrid.ItemsSource = IOContext.Instance.DigitalOutputs.Local;
            AIGrid.ItemsSource = IOContext.Instance.AnalogInputs.Local;
            AOGrid.ItemsSource = IOContext.Instance.AnalogOutputs.Local;
            AlarmsGrid.ItemsSource = IOContext.Instance.Alarms.Local;
            HistoryGrid.ItemsSource = IOContext.Instance.AlarmHistories.Local;

            this.DataContext = this;
        }

        #region Add and Update DI
        private void AddDI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital input
            DI_AddWindow dI_AddWindow = new DI_AddWindow(null);
            dI_AddWindow.ShowDialog();
            DIGrid.ItemsSource = IOContext.Instance.DigitalInputs.Local;

            try
            {
                IOContext.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void UpdateDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update digital input
            if (SelectedTab == 0 && SelectedDI != null) 
            {
                DI_AddWindow updateWindow = new DI_AddWindow(SelectedDI);
                updateWindow.ShowDialog();
                DIGrid.ItemsSource = IOContext.Instance.DigitalInputs.Local;
            }

            try
            {
                IOContext.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Add and Update DO
        private void AddDO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital output
        }
        private void UpdateDO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog output
        }
        #endregion

        #region Add and Update AI
        private void AddAI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog input
        }
        private void UpdateAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog input
        }
        #endregion

        #region Add and Update AO
        private void AddAO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog output
        }

        private void UpdateAO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog output
        }
        #endregion

        private void AddAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an alarm
        }

        #region Delete Buttons (IO, Alarms..)
        private void DeleteIO_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult value = MessageBox.Show("Are you sure?", "Deleting Entry", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (value == MessageBoxResult.Yes)
            {
                if (SelectedTab == 0 && SelectedDI != null) 
                {
                    IOContext.Instance.DigitalInputs.Remove(SelectedDI);
                }
                else if (SelectedTab == 1 && SelectedDO != null)
                {
                    IOContext.Instance.DigitalOutputs.Remove(SelectedDO);
                }
                else if (SelectedTab == 2 && SelectedAI != null)
                {
                    IOContext.Instance.AnalogInputs.Remove(SelectedAI);
                }
                else if (SelectedTab == 3 && SelectedAO != null)
                {
                    IOContext.Instance.AnalogOutputs.Remove(SelectedAO);
                }

                try
                {
                    IOContext.Instance.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
        }

        private void DeleteAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete alarm
        }

        private void EraseHistory_Click(object sender, RoutedEventArgs e)
        {
            // Logic to erase alarm history
        }

        #endregion

        private void ContinueDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to continue digital input scanning
        }

        private void PauseDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to pause digital input scanning
        }

        private void ContinueAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to resume analog input scanning
        }

        private void PauseAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to pause analog input scanning
        }

        private void AlarmsAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show alarms for analog input
        }
        private void ConfirmAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Logic to confirm alarm
        }
        private void ConfirmHistory_Click(object sender, RoutedEventArgs e)
        {
            // Logic to confirm alarm history
        }

        private void DetailsAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show details of analog input
        }
        private void DetailsAO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show details of analog output
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save configuration or perform any necessary cleanup
        }
    }
}
