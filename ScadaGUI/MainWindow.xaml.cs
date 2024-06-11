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

            Input.ValueChanged += RefreshInputs;
            // Alarm.AlarmTriggered += ActivatedAlarm;

            this.DataContext = this;
        }
        public void RefreshInputs()
        {
            DIGrid.Dispatcher.Invoke(() => { DIGrid.Items.Refresh(); });
            AIGrid.Dispatcher.Invoke(() => { AIGrid.Items.Refresh(); });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (DigitalInput DI in IOContext.Instance.DigitalInputs.Local)
            {
                DI.Abort();
            }
            foreach (AnalogInput AI in IOContext.Instance.AnalogInputs.Local)
            {
                AI.Abort();
            }
            DictionaryThreads.PLCsim.Abort();
        }

        #region Add all IO
        private void AddDI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital input
            DI_AddWindow dI_AddWindow = new DI_AddWindow(null);
            dI_AddWindow.ShowDialog();

            try
            {
                IOContext.Instance.SaveChanges();
                DIGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddDO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add a digital output
            DO_AddWindow dO_AddWindow = new DO_AddWindow(null);
            dO_AddWindow.ShowDialog();

            try
            {
                IOContext.Instance.SaveChanges();
                DOGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void AddAI_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog input
            AI_AddWindow aI_AddWindow = new AI_AddWindow(null);
            aI_AddWindow.ShowDialog();

            try
            {
                IOContext.Instance.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void AddAO_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to add an analog output
            AO_AddWindow aO_AddWindow = new AO_AddWindow(null);
            aO_AddWindow.ShowDialog();

            try
            {
                IOContext.Instance.SaveChanges();
                AOGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Update all IO
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Logic to update digital input
            if (SelectedTab == 0 && SelectedDI != null) 
            {
                DI_AddWindow updateWindow = new DI_AddWindow(SelectedDI);
                updateWindow.ShowDialog();
                DIGrid.ItemsSource = IOContext.Instance.DigitalInputs.Local;
                DIGrid.Items.Refresh();
            }
            else if (SelectedTab == 2 && SelectedAI != null)
            {
                AI_AddWindow updateWindow = new AI_AddWindow(SelectedAI);
                updateWindow.ShowDialog();
                AIGrid.ItemsSource = IOContext.Instance.AnalogInputs.Local;
                AIGrid.Items.Refresh();
            }
            else if (SelectedTab == 1 && SelectedDO != null)
            {
                DO_AddWindow updateWindow = new DO_AddWindow(SelectedDO);
                updateWindow.ShowDialog();
                DOGrid.ItemsSource = IOContext.Instance.DigitalOutputs.Local;
                DOGrid.Items.Refresh();
            }
            else if (SelectedTab == 3 && SelectedAO != null)
            {
                AO_AddWindow updateWindow = new AO_AddWindow(SelectedAO);
                updateWindow.ShowDialog();
                AOGrid.ItemsSource = IOContext.Instance.AnalogOutputs.Local;
                AOGrid.Items.Refresh();
            }
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
                    DIGrid.Items.Refresh();
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
            if (SelectedDI.OnOffScan == false)
            {
                SelectedDI.Load();
                IOContext.Instance.Entry(SelectedDI).State = System.Data.Entity.EntityState.Modified;
                IOContext.Instance.SaveChanges();
                DIGrid.Items.Refresh();
            }
        }

        private void PauseDI_Click(object sender, RoutedEventArgs e)
        {
            // Logic to pause digital input scanning
            if (SelectedDI.OnOffScan)
            {
                SelectedDI.Unload();
                IOContext.Instance.Entry(SelectedDI).State = System.Data.Entity.EntityState.Modified;
                IOContext.Instance.SaveChanges();
                DIGrid.Items.Refresh();
            }
        }

        private void ContinueAI_Click( object sender, RoutedEventArgs e)
        {
            if (!SelectedAI.OnOffScan) 
            {
                SelectedAI.Load();
                SelectedAI.OnOffScan = true; 
                IOContext.Instance.Entry(SelectedAI).State = System.Data.Entity.EntityState.Modified;
                IOContext.Instance.SaveChanges();
                AIGrid.Items.Refresh();
            }
        }

        private void PauseAI_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAI.OnOffScan) 
            {
                SelectedAI.Unload();
                SelectedAI.OnOffScan = false;
                IOContext.Instance.Entry(SelectedAI).State = System.Data.Entity.EntityState.Modified;
                IOContext.Instance.SaveChanges();
                AIGrid.Items.Refresh();
            }
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

    }
}
