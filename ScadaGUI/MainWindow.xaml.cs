using DataConcentrator;
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

            this.DataContext = this;
        }

        private void AddDI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital input
        }

        private void AddDO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital output
        }

        private void AddAI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog input
        }

        private void AddAO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog output
        }

        private void AddAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an alarm
        }

        private void ContinueDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to continue digital input scanning
        }

        private void PauseDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to pause digital input scanning
        }

        private void UpdateDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update digital input
        }

        private void DeleteDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete digital input
        }

        private void ContinueAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to resume analog input scanning
        }

        private void PauseAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to pause analog input scanning
        }

        private void DetailsAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show details of analog input
        }

        private void AlarmsAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show alarms for analog input
        }

        private void UpdateAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog input
        }

        private void DeleteAI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete analog input
        }

        private void DetailsAO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to show details of analog output
        }

        private void UpdateAO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog output
        }

        private void DeleteAO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete analog output
        }

        private void ConfirmAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Logic to confirm alarm
        }

        private void DeleteAlarm_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete alarm
        }

        private void ConfirmHistory_Click(object sender, RoutedEventArgs e)
        {
            // Logic to confirm alarm history
        }

        private void EraseHistory_Click(object sender, RoutedEventArgs e)
        {
            // Logic to erase alarm history
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save configuration or perform any necessary cleanup
        }
        private void UpdateDO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update analog output
        }

        private void DeleteDO_Click(object sender, RoutedEventArgs e)
        {
            // Logic to delete analog output
        }
    }
}
