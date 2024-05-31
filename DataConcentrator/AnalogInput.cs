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

        Thread ScanThread;
        private readonly object locker = new object();

        public void Load()
        {
            if (ScanThread == null)
            {
                ScanThread = new Thread(Scan);
                ScanThread.Start();
            }
            DictionaryThreads.dict.Add(Name, ScanThread);
            OnOffScan = true;
        }

        public void Scan()
        {
            while (true)
            {
                Thread.Sleep((int)(ScanTime * 1000));

                if (OnOffScan)
                {
                    lock (locker)
                    {
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
                            if (alarm.OnUpperVal == true)
                            {
                                if ((Value > alarm.Value) && !alarm.Activated)
                                {
                                    alarm.Activated = true;
                                    alarm.TriggerAlarm();
                                }
                            }
                            else
                            {
                                if ((Value < alarm.Value) && !alarm.Activated)
                                {
                                    alarm.Activated = true;
                                    alarm.TriggerAlarm();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Unload()
        {
            DictionaryThreads.dict.Remove(Name);
            OnOffScan = false;
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