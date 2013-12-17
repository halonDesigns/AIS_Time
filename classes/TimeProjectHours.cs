using System;
using Vici.CoolStorage;

namespace AIS_Time.classes
{

    [MapTo("TimeProjectHours")]
    public abstract partial class TimeProjectHours : CSObject<TimeProjectHours, int>
    {
        public abstract int TimeProjectHoursID { get; }
        public abstract int TimeEmployeeID { get; set; }
        public abstract int TimeDepartmentID { get; set; }
        public abstract int TimeProjectID { get; set; }
        public abstract DateTime DateOfWork { get; set; }
        public abstract int HoursOfWork { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}