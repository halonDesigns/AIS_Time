using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeDepartments")]
    public abstract partial class TimeDepartments : CSObject<TimeDepartments, int>
    {
        public abstract int TimeDepartmentID { get; }
        public abstract string DepartmentName { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}