using Vici.CoolStorage;

namespace AIS_Time.classes
{
    [MapTo("TimeCompanies")]
    public abstract partial class TimeCompanies : CSObject<TimeCompanies, int>
    {
        public abstract int TimeCompanyID { get; }
        public abstract string CompanyName { get; set; }
        public abstract string Description { get; set; }
        public abstract int Status { get; set; }
        public abstract int Type { get; set; }

    }
}