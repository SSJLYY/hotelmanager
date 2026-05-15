using System.ComponentModel;

namespace EOM.TSHotelManagement.Common
{
    public enum PoliticalAffiliation
    {
        [Description("党员")]
        PoliticalPartyMember = 1,

        [Description("团员")]
        GroupMember = 2,

        [Description("群众")]
        TheMasses = 3,

        [Description("民主党派")]
        DemocraticParty = 4,

        [Description("无党派人士")]
        NonParty = 5,

        [Description("预备党员")]
        PoliticalPartyReserveMember = 6
    }
}
