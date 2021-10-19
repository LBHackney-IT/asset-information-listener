using AssetInformationListener.Domain;
using AssetInformationListener.Domain.Account;
using AssetInformationListener.Gateway.Interfaces;
using AssetInformationListener.Infrastructure.Exceptions;
using AssetInformationListener.UseCase.Interfaces;
using Hackney.Core.Logging;
using Hackney.Core.Sns;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Tenure.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.UseCase
{
    public class UpdateAccountDetailsOnAssetTenure : IUpdateAccountDetailsOnAssetTenure
    {
        private readonly IAssetGateway _gateway;
        private readonly IAccountApi _accountApi;
        private readonly ITenureInfoApiGateway _tenureInfoApi;

        public UpdateAccountDetailsOnAssetTenure(IAssetGateway gateway, IAccountApi accountApi,
            ITenureInfoApiGateway tenureInfoApi)
        {
            _gateway = gateway;
            _accountApi = accountApi;
            _tenureInfoApi = tenureInfoApi;
        }

        [LogCall]
        public async Task ProcessMessageAsync(EntityEventSns message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            // #1 - Get the account
            var account = await _accountApi.GetAccountByIdAsync(message.EntityId, message.CorrelationId)
                                             .ConfigureAwait(false);
            if (account is null) throw new EntityNotFoundException<AccountResponseObject>(message.EntityId);

            // #2 - Get the tenure
            var tenure = await _tenureInfoApi.GetTenureInfoByIdAsync(account.TargetId, message.CorrelationId)
                                             .ConfigureAwait(false);
            if (tenure is null) throw new EntityNotFoundException<TenureResponseObject>(account.TargetId);

            // #3 - Get the asset
            var asset = await _gateway.GetAssetByIdAsync(tenure.TenuredAsset.Id).ConfigureAwait(false);
            if (asset is null) throw new EntityNotFoundException<Asset>(tenure.TenuredAsset.Id);

            // Update the payment reference if different
            if (asset.Tenure.PaymentReference != account.PaymentReference)
            {
                asset.Tenure.PaymentReference = account.PaymentReference;
                await _gateway.SaveAssetAsync(asset).ConfigureAwait(false);
            }
        }
    }
}
