using System;

namespace AssetInformationListener.Domain.Tenure
{
    public class TenureResponseObject
    {
        public Guid Id { get; set; }
        public string PaymentReference { get; set; }
        public TenuredAsset TenuredAsset { get; set; }
        public DateTime StartOfTenureDate { get; set; }
        public DateTime? EndOfTenureDate { get; set; }
        public TenureType TenureType { get; set; }
    }
}
