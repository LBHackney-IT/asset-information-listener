using Amazon.DynamoDBv2.DataModel;
using AssetInformationListener.Infrastructure.Exceptions;
using FluentAssertions;
using Hackney.Shared.Asset.Domain;
using Hackney.Shared.Asset.Infrastructure;
using Hackney.Shared.Tenure.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Tests.E2ETests.Steps
{
    public class TenureCreatedOrUpdatedSteps : BaseSteps
    {
        public TenureCreatedOrUpdatedSteps()
        {
            _eventType = EventTypes.TenureCreatedEvent;
        }

        public async Task WhenTheFunctionIsTriggered(Guid id)
        {
            await WhenTheFunctionIsTriggered(id, EventTypes.TenureCreatedEvent);
        }
        public async Task WhenTheFunctionIsTriggered(Guid id, string eventType)
        {
            var msg = CreateMessage(id, eventType);
            await TriggerFunction(msg).ConfigureAwait(false);
        }

        public void ThenTheCorrleationIdWasUsedInTheApiCall(string receivedCorrelationId)
        {
            receivedCorrelationId.Should().Be(_correlationId.ToString());
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

        public async Task ThenTheAssetIsUpdatedWithTheTenureInfo(AssetDb beforeChange, TenureResponseObject tenure, IDynamoDBContext dbContext)
        {
            var assetInDb = await dbContext.LoadAsync<AssetDb>(beforeChange.Id);

            assetInDb.Should().BeEquivalentTo(beforeChange,
                config => config.Excluding(y => y.Tenure)
                                .Excluding(z => z.VersionNumber));

            var expectedTenureInfo = new AssetTenure
            {
                Id = tenure.Id.ToString(),
                PaymentReference = tenure.PaymentReference,
                Type = tenure.TenureType.Description,
                StartOfTenureDate = tenure.StartOfTenureDate,
                EndOfTenureDate = tenure.EndOfTenureDate
            };
            assetInDb.Tenure.Should().BeEquivalentTo(expectedTenureInfo,
                config => config.Excluding(y => y.IsActive));
            assetInDb.VersionNumber.Should().Be(beforeChange.VersionNumber + 1);
        }
    }
}
