using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeAISCodes")]
    public abstract partial class TimeAISCodes : CSObject<TimeAISCodes, int>
    {
        public abstract int TimeAISCodeID { get; }
        public abstract string AISCode { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

        //[OneToMany]
        //public abstract CSList<TimeResources> Resources { get; }

    }
}