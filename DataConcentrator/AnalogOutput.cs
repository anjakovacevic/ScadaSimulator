using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class AnalogOutput : Output
    {
        public double Value { get; set; }
        public double LowLimit { get; set; }
        public double HighLimit { get; set; }
        public string Units { get; set; }

        public void Load()
        {
            DictionaryThreads.PLCsim.SetAnalogValue(Address, Value);
        }
    }
}
