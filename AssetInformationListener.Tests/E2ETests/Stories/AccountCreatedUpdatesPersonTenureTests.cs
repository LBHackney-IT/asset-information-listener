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
        IWant = "a function to process the AccountCreated message",
        SoThat = "The correct details are set on the appropriate asset")]
    [Collection("DynamoDb collection")]
    public class AccountCreatedUpdatesPersonTenureTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly AssetFixture _assetFixture;
        private readonly TenureApiFixture _tenureApiFixture;
        private readonly AccountApiFixture _accountApiFixture;

        private readonly AccountCreatedSteps _steps;

        public AccountCreatedUpdatesPersonTenureTests(MockApplicationFactory appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;

            _assetFixture = new AssetFixture(_dbFixture);
            _tenureApiFixture = new TenureApiFixture();
            _accountApiFixture = new AccountApiFixture();

            _steps = new AccountCreatedSteps();
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
                _accountApiFixture.Dispose();

                _disposed = true;
            }
        }

        [Fact]
        public void ListenerUpdatesTheAsset()
        {
            var accountId = Guid.NewGuid();
            this.Given(g => _accountApiFixture.GivenTheAccountExists(accountId))
                .And(h => _tenureApiFixture.GivenTheTenureExists(_accountApiFixture.ResponseObject.TargetId))
                .And(i => _assetFixture.GivenAnAssetExists(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(accountId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_accountApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheAssetIsUpdated(_assetFixture.DbAsset, _accountApiFixture.ResponseObject,
                                                        _dbFixture.DynamoDbContext))
                .BDDfy();
        }

        [Fact]
        public void AccountNotFound()
        {
            var accountId = Guid.NewGuid();
            this.Given(g => _accountApiFixture.GivenTheAccountDoesNotExist(accountId))
                .When(w => _steps.WhenTheFunctionIsTriggered(accountId))
                .Then(t => _steps.ThenAnAccountNotFoundExceptionIsThrown(accountId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_accountApiFixture.ReceivedCorrelationIds))
                .BDDfy();
        }

        [Fact]
        public void TenureNotFound()
        {
            var accountId = Guid.NewGuid();
            this.Given(g => _accountApiFixture.GivenTheAccountExists(accountId))
                .And(h => _tenureApiFixture.GivenTheTenureDoesNotExist(_accountApiFixture.ResponseObject.TargetId))
                .When(w => _steps.WhenTheFunctionIsTriggered(accountId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_accountApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenATenureNotFoundExceptionIsThrown(_accountApiFixture.ResponseObject.TargetId))
                .BDDfy();
        }

        [Fact]
        public void AssetNotFound()
        {
            var accountId = Guid.NewGuid();
            this.Given(g => _accountApiFixture.GivenTheAccountExists(accountId))
                .And(h => _tenureApiFixture.GivenTheTenureExists(_accountApiFixture.ResponseObject.TargetId))
                .And(h => _assetFixture.GivenAnAssetDoesNotExist(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .When(w => _steps.WhenTheFunctionIsTriggered(accountId))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_accountApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenTheCorrelationIdWasUsedInTheApiCall(_tenureApiFixture.ReceivedCorrelationIds))
                .Then(t => _steps.ThenAnAssetNotFoundExceptionIsThrown(_tenureApiFixture.ResponseObject.TenuredAsset.Id))
                .BDDfy();
        }
    }
}
