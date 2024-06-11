using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataConcentrator
{
    public class AnalogInput : Input
    {
        public double Value { get; set; }
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }
        public int Alarming { get; set; }


        public virtual List<Alarm> Alarms { get; set; } = new List<Alarm>();

        private Thread ScanThread;
        private readonly object locker = new object();

        /* ManualResetEvent is a synchronization primitive used to pause and resume threads by signaling; 
         * when set, threads continue, and when reset, threads wait.*/
        private ManualResetEvent pauseWaitHandle = new ManualResetEvent(true);

        /*The `Load` function starts a new scanning thread if it isn't already running, adds this thread
         * to a dictionary, and enables scanning by setting `OnOffScan` to true and signaling any paused
         * operations to continue.*/
        public void Load()
        {
            if (ScanThread == null || !ScanThread.IsAlive)
            {
                ScanThread = new Thread(Scan);
                ScanThread.Start();
            }
            DictionaryThreads.dict.Add(Name, ScanThread);
            OnOffScan = true;
            pauseWaitHandle.Set();
        }
        /*
        The `Scan` function continuously monitors an analog value from a PLC simulator at intervals defined 
        by `ScanTime`, adjusting the value to stay within specified limits (`HighLimit` and `LowLimit`). 
        If scanning is enabled (`OnOffScan`), it locks the process to update the value and checks against 
        alarms, triggering any that are activated by the current value.
        */
        public void Scan()
        {
            while (true)
            {
                pauseWaitHandle.WaitOne();

                lock (locker)
                {
                    if (!OnOffScan)
                    {
                        pauseWaitHandle.Reset();
                        continue;
                    }

                    double CurrentValue = DictionaryThreads.PLCsim.GetAnalogValue(Address);
                    CurrentValue = Math.Round(CurrentValue, 2);

                    if (CurrentValue > HighLimit)
                    {
                        Value = HighLimit;
                    }
                    else if (CurrentValue < LowLimit)
                    {
                        Value = LowLimit;
                    }
                    else
                    {
                        Value = CurrentValue;
                        Input.Changed();
                    }

                    foreach (Alarm alarm in Alarms)
                    {
                        if (alarm.OnUpperVal)
                        {
                            if (Value > alarm.Value && !alarm.Activated)
                            {
                                alarm.TriggerAlarm();
                            }
                        }
                        else
                        {
                            if (Value < alarm.Value && !alarm.Activated)
                            {
                                alarm.TriggerAlarm();
                            }
                        }
                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(ScanTime));
            }
        }

        public void Unload()
        {
            DictionaryThreads.dict.Remove(Name);
            OnOffScan = false;
            pauseWaitHandle.Reset();
        }

        public void Abort()
        {
            DictionaryThreads.dict.Remove(Name);
            if (ScanThread != null)
            {
                ScanThread.Abort();
            }
        }
    }
}