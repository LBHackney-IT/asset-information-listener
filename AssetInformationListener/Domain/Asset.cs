using System;

namespace AssetInformationListener.Domain
{
    public class Asset
    {
        public Guid Id { get; set; }
        public string AssetId { get; set; }
        public AssetType AssetType { get; set; }
        public string RootAsset { get; set; }
        public string ParentAssetIds { get; set; }

        public AssetLocation AssetLocation { get; set; }
        public AssetAddress AssetAddress { get; set; }
        public AssetManagement AssetManagement { get; set; }
        public AssetCharacteristics AssetCharacteristics { get; set; }
        public AssetTenure Tenure { get; set; }

        public int? VersionNumber { get; set; }
    }
}
