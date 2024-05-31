using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using PLCSimulator;

namespace DataConcentrator
{
    public class DictionaryThreads
    {
        public static Dictionary<string, Thread> dict = new Dictionary<string, Thread>();

        private static PLCSimulatorManager plcSim;
        public static PLCSimulatorManager PLCsim
        {
            get
            {
                if (plcSim == null)
                {
                    plcSim = new PLCSimulatorManager();
                    plcSim.StartPLCSimulator();
                }
                return plcSim;
            }
        }
    }
}
