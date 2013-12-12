using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeResources")]
    public abstract partial class TimeResources : CSObject<TimeResources, int>
    {
        public abstract int ResourceID { get; }
        public abstract string ResourceName { get; set; }
        public abstract string AISCode { get; set; }
        public abstract string CEAClassCode { get; set; }
        public abstract decimal HourlyRate { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}