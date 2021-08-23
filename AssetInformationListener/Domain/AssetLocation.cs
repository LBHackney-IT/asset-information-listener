using System.Collections.Generic;

namespace AssetInformationListener.Domain
{
    public class AssetLocation
    {
        public string FloorNo { get; set; }
        public int TotalBlockFloors { get; set; }
        public IEnumerable<ParentAsset> ParentAssets { get; set; }
    }
}
