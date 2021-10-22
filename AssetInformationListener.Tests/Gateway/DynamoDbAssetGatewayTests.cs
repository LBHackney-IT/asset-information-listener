using Amazon.DynamoDBv2.DataModel;
using AssetInformationListener.Gateway;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Asset.Factories;
using Hackney.Shared.Asset.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AssetInformationListener.Tests.Gateway
{
    [Collection("DynamoDb collection")]
    public class DynamoDbAssetGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<ILogger<DynamoDbAssetGateway>> _logger;
        private readonly DynamoDbAssetGateway _classUnderTest;
        private readonly IDynamoDbFixture _dbFixture;
        private IDynamoDBContext DynamoDb => _dbFixture.DynamoDbContext;

        public DynamoDbAssetGatewayTests(MockApplicationFactory appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _logger = new Mock<ILogger<DynamoDbAssetGateway>>();
            _classUnderTest = new DynamoDbAssetGateway(DynamoDb, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }
        }

        private async Task InsertDatatoDynamoDB(Asset entity)
        {
            await _dbFixture.SaveEntityAsync(entity.ToDatabase()).ConfigureAwait(false);
        }

        private Asset ConstructAsset()
        {
            var entity = _fixture.Build<Asset>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            return entity;
        }

        [Fact]
        public async Task GetAssetByIdAsyncTestReturnsRecord()
        {
            var domainEntity = ConstructAsset();
            await InsertDatatoDynamoDB(domainEntity).ConfigureAwait(false);

            var result = await _classUnderTest.GetAssetByIdAsync(domainEntity.Id).ConfigureAwait(false);

            result.Should().BeEquivalentTo(domainEntity, (e) => e.Excluding(y => y.VersionNumber));
            result.VersionNumber.Should().Be(0);

            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {domainEntity.Id}", Times.Once());
        }

        [Fact]
        public async Task GetAssetByIdAsyncTestReturnsNullWhenNotFound()
        {
            var id = Guid.NewGuid();
            var result = await _classUnderTest.GetAssetByIdAsync(id).ConfigureAwait(false);

            result.Should().BeNull();

            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id {id}", Times.Once());
        }

        [Fact]
        public async Task SaveAssetAsyncTestUpdatesDatabase()
        {
            var domainEntity = ConstructAsset();
            await InsertDatatoDynamoDB(domainEntity).ConfigureAwait(false);

            domainEntity.RootAsset = Guid.NewGuid().ToString();
            domainEntity.Tenure = _fixture.Create<AssetTenure>();
            domainEntity.AssetAddress = _fixture.Create<AssetAddress>();
            domainEntity.VersionNumber = 0;
            await _classUnderTest.SaveAssetAsync(domainEntity).ConfigureAwait(false);

            var updatedInDB = await DynamoDb.LoadAsync<AssetDb>(domainEntity.Id).ConfigureAwait(false);
            updatedInDB.ToDomain().Should().BeEquivalentTo(domainEntity, (e) => e.Excluding(y => y.VersionNumber));
            updatedInDB.VersionNumber.Should().Be(domainEntity.VersionNumber + 1);

            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync for id {domainEntity.Id}", Times.Once());
        }
    }
}
