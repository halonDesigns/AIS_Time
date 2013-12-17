using Vici.CoolStorage;

namespace AIS_Time.classes
{
   [MapTo("TimeCEAClassCodes")]
    public abstract partial class TimeCEAClassCodes : CSObject<TimeCEAClassCodes, int>
    {
       public abstract int TimeCEAClassCodeID { get; }
       public abstract string CEAClassCode { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

        //[OneToMany]
        //public abstract CSList<TimeResources> Resources { get; }

    }
}