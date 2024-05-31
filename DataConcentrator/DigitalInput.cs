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
    public class DigitalInput : Input
    {
        public bool Value { get; set; }

        Thread ScanThread;

        public readonly object locker1 = new object();

        public DigitalInput()
        {
            OnOffScan = false;
        }

        public DigitalInput(string name, string desc, string address, double scan)
        {
            Name = name;
            Description = desc;
            Address = address;
            ScanTime = scan;
            OnOffScan = false;
        }

        public void Load()
        {
            if (ScanThread == null)
            {
                ScanThread = new Thread(Scan);
                ScanThread.Start();
            }
            OnOffScan = true;
            DictionaryThreads.dict.Add(Name, ScanThread);
        }

        public void Scan()
        {
            while (true)
            {
                Thread.Sleep((int)(ScanTime * 1000));

                if (OnOffScan)
                {
                    lock (locker1)
                    {
                        bool PLCValue;
                        if (DictionaryThreads.PLCsim.GetAnalogValue(Address) == 0)
                        {
                            PLCValue = false;
                        }
                        else if (DictionaryThreads.PLCsim.GetAnalogValue(Address) == 1)
                        {
                            PLCValue = true;
                        }
                        else
                        {
                            //PROBLEM WITH SIMULATOR
                            PLCValue = false;
                        }
                        if (PLCValue != Value)
                        {
                            Value = PLCValue;
                            Input.Changed();
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
