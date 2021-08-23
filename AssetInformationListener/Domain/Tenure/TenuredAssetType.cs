using System.Text.Json.Serialization;

namespace AssetInformationListener.Domain.Tenure
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TenuredAssetType
    {
        Block,
        Concierge,
        Dwelling,
        LettableNonDwelling,
        MediumRiseBlock,
        NA,
        TravellerSite
    }
}
