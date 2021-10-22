using AssetInformationListener.Tests.E2ETests.Fixtures;
using AssetInformationListener.Tests.E2ETests.Steps;
using Hackney.Core.Testing.DynamoDb;
using System;
using TestStack.BDDfy;
using Xunit;

namespace AssetInformationListener.Tests.E2ETests.Stories
{
    [Story(
        AsA = "SQS Entity Listener",
        IWant = "a function to process the TenureCreated message",
        SoThat = "the asset is updated with correct details fromn the new tenure")]
    [Collection("DynamoDb collection")]
    public class TenureCreatedOrUpdatedTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly AssetFixture _assetFixture;
        private readonly TenureApiFixture _tenureApiFixture;

        private readonly TenureCreatedOrUpdatedSteps _steps;

        public TenureCreatedOrUpdatedTests(MockApplicationFactory appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;

            _assetFixture = new AssetFixture(_dbFixture);
            _tenureApiFixture = new TenureApiFixture();

            _steps = new TenureCreatedOrUpdatedSteps();
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
                _assetFixture.Dispose();
                _tenureApiFixture.Dispose();

                _disposed = true;
            }
        }

        [Fact]
        public void NewTenureNotFound()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureDoesNotExist(tenureId))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenATenureNotFoundExceptionIsThrown(tenureId))
                .BDDfy();
        }

        [Fact]
        public void AssetNotFound()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetDoesNotExist(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenAnAssetNotFoundExceptionIsThrown(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .BDDfy();
        }

        [Fact]
        public void ListenerAddsTenureInfoToTheAsset()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetExists(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId, EventTypes.TenureCreatedEvent))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheAssetIsUpdatedWithTheTenureInfo(_assetFixture.DbAsset, _tenureApiFixture.ResponseObject,
                                                                         _dbFixture.DynamoDbContext))
                .BDDfy();
        }

        [Fact]
        public void ListenerUpdatesTheTenureInfoOnTheAsset()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetExistsWithTenureInfo(_tenureApiFixture.ResponseObject))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId, EventTypes.TenureUpdatedEvent))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheAssetIsUpdatedWithTheTenureInfo(_assetFixture.DbAsset, _tenureApiFixture.ResponseObject,
                                                                         _dbFixture.DynamoDbContext))
                .BDDfy();
        }
    }
}
