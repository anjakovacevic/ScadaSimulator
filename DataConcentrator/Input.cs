using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class Input : Tag
    {
        public double ScanTime { get; set; }
        public bool OnOffScan { get; set; }
        public static event Action ValueChanged;

        public static void Changed()
        {
            ValueChanged?.Invoke();
        }
    }
}
