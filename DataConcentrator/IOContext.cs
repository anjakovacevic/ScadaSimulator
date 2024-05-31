using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class IOContext : DbContext
    {
        private static IOContext instance;
        public static IOContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IOContext();
                }
                return instance;
            }
        }

        public DbSet<AnalogInput> AnalogInputs { get; set; }
        public DbSet<AnalogOutput> AnalogOutputs { get; set; }
        public DbSet<DigitalInput> DigitalInputs { get; set; }
        public DbSet<DigitalOutput> DigitalOutputs { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<AlarmHistory> AlarmHistories { get; set; }
    }
}