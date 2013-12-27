using System;

namespace AIS_Time.classes
{
    public class WeeklyReportResult
    {
        public DateTime DateOfWork;
        public int HoursOfWork;
        public int TimeEmployeeID;
        public int TimeProjectID;
        public int TimeDepartmentID;
        public string CEAClassCode;
        public decimal HourlyRate;

        //[QueryExpression]
        //public DateTime DateStartWeek;

    }
}