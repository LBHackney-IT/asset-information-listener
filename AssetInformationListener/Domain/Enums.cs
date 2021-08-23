using System.Text.Json.Serialization;

namespace AssetInformationListener.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AssetType
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
