using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeEmployees")]
    public abstract partial class TimeEmployees : CSObject<TimeEmployees, int>
    {
        public abstract int TimeEmployeeID { get; }
        public abstract string FirstName { get; set; }
        public abstract string LastName { get; set; }
        public abstract int CompanyID { get; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }
    }
}