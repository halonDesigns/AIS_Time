using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;

namespace AIS_Time.classes
{
    public class WeeklyReportResult
    {
        public DateTime DateOfWork;
        public int HoursOfWork;
        public int TimeEmployeeID;
        public int TimeProjectID;
        public string CEAClassCode;
        public decimal HourlyRate;

        //[QueryExpression]
        //public DateTime DateStartWeek;

    }
}