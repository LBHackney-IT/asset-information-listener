using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.SQSEvents;
using AssetInformationListener.Domain;
using AssetInformationListener.Domain.Account;
using AssetInformationListener.Infrastructure;
using AssetInformationListener.Infrastructure.Exceptions;
using FluentAssertions;
using Hackney.Shared.Tenure.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Tests.E2ETests.Steps
{
    public class AccountCreatedSteps : BaseSteps
    {
        public AccountCreatedSteps()
        {
            _eventType = EventTypes.AccountCreatedEvent;
        }

        public async Task WhenTheFunctionIsTriggered(Guid id)
        {
            await TriggerFunction(id).ConfigureAwait(false);
        }

        public async Task WhenTheFunctionIsTriggered(SQSEvent.SQSMessage message)
        {
            await TriggerFunction(message).ConfigureAwait(false);
        }

        public void TheNoExceptionIsThrown()
        {
            _lastException.Should().BeNull();
        }

        public void ThenAnAccountNotFoundExceptionIsThrown(Guid id)
        {
            _lastException.Should().NotBeNull();
            _lastException.Should().BeOfType(typeof(EntityNotFoundException<AccountResponseObject>));
            (_lastException as EntityNotFoundException<AccountResponseObject>).Id.Should().Be(id);
        }

        public void ThenATenureNotFoundExceptionIsThrown(Guid id)
        {
            _lastException.Should().NotBeNull();
            _lastException.Should().BeOfType(typeof(EntityNotFoundException<TenureResponseObject>));
            (_lastException as EntityNotFoundException<TenureResponseObject>).Id.Should().Be(id);
        }

        public void ThenAnAssetNotFoundExceptionIsThrown(Guid id)
        {
            _lastException.Should().NotBeNull();
            _lastException.Should().BeOfType(typeof(EntityNotFoundException<Asset>));
            (_lastException as EntityNotFoundException<Asset>).Id.Should().Be(id);
        }

        public async Task ThenTheAssetIsUpdated(AssetDb originalAssetDb,
            AccountResponseObject account, IDynamoDBContext dbContext)
        {
            var updatedAssetInDb = await dbContext.LoadAsync<AssetDb>(originalAssetDb.Id);

            updatedAssetInDb.Should().BeEquivalentTo(originalAssetDb,
                config => config.Excluding(y => y.Tenure)
                                .Excluding(z => z.VersionNumber));
            updatedAssetInDb.Tenure.Should().BeEquivalentTo(originalAssetDb.Tenure,
                config => config.Excluding(y => y.PaymentReference));

            updatedAssetInDb.Tenure.PaymentReference.Should().Be(account.PaymentReference);
            updatedAssetInDb.VersionNumber.Should().Be(originalAssetDb.VersionNumber + 1);
        }
    }
}
