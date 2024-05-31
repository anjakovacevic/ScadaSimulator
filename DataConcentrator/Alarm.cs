using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConcentrator
{
    public class Alarm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public bool OnUpperVal { get; set; }
        public string Message { get; set; }
        public bool Activated { get; set; }
        public bool HighPriority { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public string TagName { get; set; }
        public virtual AnalogInput Tag { get; set; }

        public static event Action<Alarm> AlarmTriggered;

        public Alarm()
        {
            Activated = false;
        }

        public void TriggerAlarm()
        {
            AlarmTriggered?.Invoke(this);
        }
    }
}
