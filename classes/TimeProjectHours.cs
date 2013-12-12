using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vici.CoolStorage;

namespace AIS_Time.classes
{
    public class TimeProjectHours
    {
        //TimeProjectHoursID
        //EmployeeID
        //DepartmentID
        //ProjectID
        //DateOfWork
        //HoursOfWork
        //Status
        //Type
    }

    [MapTo("TimeProjectHours")]
    public abstract partial class TimeProjectHours : CSObject<TimeProjectHours, int>
    {
        public abstract int TimeProjectHoursID { get; }
        public abstract int TimeEmployeeID { get; }
        public abstract int TimeDepartmentID { get; }
        public abstract int TimeProjectID { get; }
        public abstract string ResourceName { get; set; }
        public abstract string AISCode { get; set; }
        public abstract string CEAClassCode { get; set; }
        public abstract decimal HourlyRate { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}