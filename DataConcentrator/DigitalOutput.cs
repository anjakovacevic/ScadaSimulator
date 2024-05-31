using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class DigitalOutput : Output
    {
        public bool Value { get; set; }

        public DigitalOutput()
        {
            //null
        }

        public DigitalOutput(string name, string desc, string address, string init)
        {
            Name = name;
            Description = desc;
            Address = address;
            Value = Boolean.Parse(init);
            if (Value)
            {
                InitialValue = 1;
            }
            else
            {
                InitialValue = 0;
            }
        }

        public void Load()
        {
            DictionaryThreads.PLCsim.SetDigitalValue(Address, Convert.ToDouble(Value));
        }
    }
}
