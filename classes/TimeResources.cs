using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeResources")]
    public abstract partial class TimeResources : CSObject<TimeResources, int>
    {
        public abstract int TimeResourceID { get; }
        public abstract string ResourceName { get; set; }
        public abstract int TimeAISCodeID { get; set; }
        //public abstract int TimeCEAClassCodeID { get; set; }
        public abstract decimal HourlyRate { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

        //[ManyToOne]
        //public abstract TimeAISCodes AISCodes { get; set; }

        //[ManyToOne]
        //public abstract TimeCEAClassCodes CEAClassCodes { get; set; }

    }
}