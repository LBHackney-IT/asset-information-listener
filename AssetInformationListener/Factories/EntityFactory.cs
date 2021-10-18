using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Asset.Infrastructure;

namespace AssetInformationListener.Factories
{
    //public static class EntityFactory
    //{
    //    public static Asset ToDomain(this AssetDb databaseEntity)
    //    {
    //        if (databaseEntity == null) return null;
    //        return new Asset
    //        {
    //            Id = databaseEntity.Id,
    //            AssetId = databaseEntity.AssetId,
    //            AssetType = databaseEntity.AssetType,
    //            RootAsset = databaseEntity.RootAsset,
    //            ParentAssetIds = databaseEntity.ParentAssetIds,
    //            AssetLocation = databaseEntity.AssetLocation,
    //            AssetAddress = databaseEntity.AssetAddress,
    //            AssetManagement = databaseEntity.AssetManagement,
    //            AssetCharacteristics = databaseEntity.AssetCharacteristics,
    //            Tenure = databaseEntity.Tenure.ToDomain(),
    //            VersionNumber = databaseEntity.VersionNumber
    //        };
    //    }

    //    public static AssetTenure ToDomain(this AssetTenureDb databaseEntity)
    //    {
    //        if (databaseEntity == null) return null;
    //        return new AssetTenure
    //        {
    //            Id = databaseEntity.Id,
    //            Type = databaseEntity.Type,
    //            PaymentReference = databaseEntity.PaymentReference,
    //            StartOfTenureDate = databaseEntity.StartOfTenureDate,
    //            EndOfTenureDate = databaseEntity.EndOfTenureDate
    //        };
    //    }

    //    public static AssetDb ToDatabase(this Asset domain)
    //    {
    //        if (domain == null) return null;
    //        return new AssetDb
    //        {
    //            Id = domain.Id,
    //            AssetId = domain.AssetId,
    //            AssetType = domain.AssetType,
    //            RootAsset = domain.RootAsset,
    //            ParentAssetIds = domain.ParentAssetIds,
    //            AssetLocation = domain.AssetLocation,
    //            AssetAddress = domain.AssetAddress,
    //            AssetManagement = domain.AssetManagement,
    //            AssetCharacteristics = domain.AssetCharacteristics,
    //            Tenure = domain.Tenure.ToDatabase(),
    //            VersionNumber = domain.VersionNumber
    //        };
    //    }

    //    public static AssetTenureDb ToDatabase(this AssetTenure domain)
    //    {
    //        if (domain == null) return null;
    //        return new AssetTenureDb
    //        {
    //            Id = domain.Id,
    //            Type = domain.Type,
    //            PaymentReference = domain.PaymentReference,
    //            StartOfTenureDate = domain.StartOfTenureDate,
    //            EndOfTenureDate = domain.EndOfTenureDate
    //        };
    //    }
    //}
}
