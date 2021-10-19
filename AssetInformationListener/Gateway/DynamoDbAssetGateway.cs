using Amazon.DynamoDBv2.DataModel;
using AssetInformationListener.Gateway.Interfaces;
using Hackney.Core.Logging;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Asset.Factories;
using Hackney.Shared.Asset.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway
{
    public class DynamoDbAssetGateway : IAssetGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<DynamoDbAssetGateway> _logger;

        public DynamoDbAssetGateway(IDynamoDBContext dynamoDbContext, ILogger<DynamoDbAssetGateway> logger)
        {
            _logger = logger;
            _dynamoDbContext = dynamoDbContext;
        }

        [LogCall]
        public async Task<Asset> GetAssetByIdAsync(Guid id)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {id}");
            var dbEntity = await _dynamoDbContext.LoadAsync<AssetDb>(id).ConfigureAwait(false);
            return dbEntity?.ToDomain();
        }

        [LogCall]
        public async Task SaveAssetAsync(Asset entity)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync for id {entity.Id}");
            await _dynamoDbContext.SaveAsync(entity.ToDatabase()).ConfigureAwait(false);
        }
    }
}
