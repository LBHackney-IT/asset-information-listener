using System;

namespace AssetInformationListener.Domain.Account
{
    public class ConsolidatedCharge
    {
        public string Type { get; set; }
        public string Frequency { get; set; }
        public Decimal Amount { get; set; }
    }
}
