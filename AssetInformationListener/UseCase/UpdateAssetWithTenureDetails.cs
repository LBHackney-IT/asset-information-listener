using AssetInformationListener.Domain;
using AssetInformationListener.Domain.Account;
using AssetInformationListener.Gateway.Interfaces;
using AssetInformationListener.Infrastructure.Exceptions;
using AssetInformationListener.UseCase.Interfaces;
using Hackney.Core.Sns;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Tenure.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.UseCase
{
    public class UpdateAssetWithTenureDetails : IUpdateAssetWithTenureDetails
    {
        private readonly IAssetGateway _gateway;
        private readonly ITenureInfoApiGateway _tenureInfoApi;

        public UpdateAssetWithTenureDetails(IAssetGateway gateway, ITenureInfoApiGateway tenureInfoApi)
        {
            _gateway = gateway;
            _tenureInfoApi = tenureInfoApi;
        }

        public async Task ProcessMessageAsync(EntityEventSns message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            // 1. Get Tenure from Tenure service API
            var tenure = await _tenureInfoApi.GetTenureInfoByIdAsync(message.EntityId, message.CorrelationId)
                                             .ConfigureAwait(false);
            if (tenure is null) throw new EntityNotFoundException<TenureResponseObject>(message.EntityId);

            // 2. Get the asset
            var assetId = tenure.TenuredAsset.Id;
            var asset = await _gateway.GetAssetByIdAsync(assetId)
                                      .ConfigureAwait(false);
            if (asset is null) throw new EntityNotFoundException<Asset>(assetId);

            // 3. Update the asset with the tenure details
            asset.Tenure = new AssetTenure
            {
                Id = tenure.Id.ToString(),
                PaymentReference = tenure.PaymentReference,
                Type = tenure.TenureType.Description,
                StartOfTenureDate = tenure.StartOfTenureDate,
                EndOfTenureDate = tenure.EndOfTenureDate
            };

            // 4. Save updated asset
            await _gateway.SaveAssetAsync(asset).ConfigureAwait(false);
        }
    }
}
