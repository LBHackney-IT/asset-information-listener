using System.Text.Json.Serialization;

namespace AssetInformationListener.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AssetType
    {
        AdministrativeBuilding,
        Block,
        BoilerHouse,
        BoosterPump,
        CleanersFacilities,
        CombinedHeatAndPowerUnit,
        CommunityHall,
        Concierge,
        Dwelling,
        Estate,
        HighRiseBlock,
        LettableNonDwelling,
        Lift,
        LowRiseBlock,
        MediumRiseBlock,
        NA,
        NBD,
        OutBuilding,
        TerracedBlock,
        TravellerSite,
        WalkUpBlock
    }
}
