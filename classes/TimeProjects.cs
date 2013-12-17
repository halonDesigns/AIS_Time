using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeProjects")]
    public abstract partial class TimeProjects : CSObject<TimeProjects, int>
    {
        public abstract int TimeProjectID { get; }
        public abstract int TimeCustomerID { get; set; }
        public abstract string ProjectName { get; set; }
        public abstract string ProjectNumber { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}