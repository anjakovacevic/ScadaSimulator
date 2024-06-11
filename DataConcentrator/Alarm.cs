using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
            if (!Activated) 
            {
                Activated = true;
                AlarmTriggered?.Invoke(this);
                LogAlarmHistory();
            }
        }

        private void LogAlarmHistory()
        {
            using (var context = IOContext.Instance) 
            {
                var alarmHistory = new AlarmHistory
                {
                    AlarmID = this.Id,
                    VarName = this.TagName,
                    Message = this.Message,
                    TimeStamp = DateTime.Now,
                    Acknowledged = false
                };
                context.AlarmHistories.Add(alarmHistory);
                context.SaveChanges();
            }
        }
    }
}
