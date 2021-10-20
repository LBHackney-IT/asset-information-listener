using System;

namespace AssetInformationListener.Domain
{
    public class AssetManagement
    {
        public string Agent { get; set; }
        public string AreaOfficeName { get; set; }
        public bool IsCouncilProperty { get; set; }
        public string ManagingOrganisation { get; set; }
        public Guid ManagingOrganisationId { get; set; }
        public string Owner { get; set; }
        public bool IsTMOManaged { get; set; }
    }
}
