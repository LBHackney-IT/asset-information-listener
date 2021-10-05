using AssetInformationListener.Tests.E2ETests.Fixtures;
using AssetInformationListener.Tests.E2ETests.Steps;
using System;
using TestStack.BDDfy;
using Xunit;

namespace AssetInformationListener.Tests.E2ETests.Stories
{
    [Story(
        AsA = "SQS Entity Listener",
        IWant = "a function to process the TenureCreated message",
        SoThat = "the asset is updated with correct details fromn the new tenure")]
    [Collection("Aws collection")]
    public class TenureCreatedOrUpdatedTests : IDisposable
    {
        private readonly AwsIntegrationTests _dbFixture;
        private readonly AssetFixture _assetFixture;
        private readonly TenureApiFixture _tenureApiFixture;

        private readonly TenureCreatedOrUpdatedSteps _steps;

        public TenureCreatedOrUpdatedTests(AwsIntegrationTests dbFixture)
        {
            _dbFixture = dbFixture;

            _assetFixture = new AssetFixture(_dbFixture.DynamoDbContext);
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
                .Then(t => _steps.ThenTheCorrleationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationId))
                .Then(t => _steps.ThenATenureNotFoundExceptionIsThrown(tenureId))
                .BDDfy();
        }

        [Fact]
        public void AssetNotFound()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetDoesNotExist(TenureApiFixture.TenureResponse.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId))
                .Then(t => _steps.ThenTheCorrleationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationId))
                .Then(t => _steps.ThenAnAssetNotFoundExceptionIsThrown(TenureApiFixture.TenureResponse.TenuredAsset.Id))
                .BDDfy();
        }

        [Fact]
        public void ListenerAddsTenureInfoToTheAsset()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetExists(TenureApiFixture.TenureResponse.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId, EventTypes.TenureCreatedEvent))
                .Then(t => _steps.ThenTheCorrleationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationId))
                .Then(t => _steps.ThenTheAssetIsUpdatedWithTheTenureInfo(_assetFixture.DbAsset, TenureApiFixture.
                                                                         TenureResponse, _dbFixture.DynamoDbContext))
                .BDDfy();
        }

        [Fact]
        public void ListenerUpdatesTheTenureInfoOnTheAsset()
        {
            var tenureId = Guid.NewGuid();
            this.Given(g => _tenureApiFixture.GivenTheTenureExists(tenureId))
                .And(h => _assetFixture.GivenAnAssetExistsWithTenureInfo(TenureApiFixture.TenureResponse))
                .When(w => _steps.WhenTheFunctionIsTriggered(tenureId, EventTypes.TenureUpdatedEvent))
                .Then(t => _steps.ThenTheCorrleationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationId))
                .Then(t => _steps.ThenTheAssetIsUpdatedWithTheTenureInfo(_assetFixture.DbAsset, TenureApiFixture.
                                                                         TenureResponse, _dbFixture.DynamoDbContext))
                .BDDfy();
        }
    }
}
