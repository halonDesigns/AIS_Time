using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeCustomers")]
    public abstract partial class TimeCustomers : CSObject<TimeCustomers, int>
    {
        public abstract int TimeCustomerID { get; }
        public abstract string CustomerName { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}