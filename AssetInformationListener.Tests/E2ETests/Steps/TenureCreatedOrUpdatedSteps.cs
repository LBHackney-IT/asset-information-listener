using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using AssetInformationListener.Domain;
using AssetInformationListener.Infrastructure;
using AssetInformationListener.Infrastructure.Exceptions;
using AutoFixture;
using FluentAssertions;
using Hackney.Shared.Tenure.Boundary.Response;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

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
            var mockLambdaLogger = new Mock<ILambdaLogger>();
            ILambdaContext lambdaContext = new TestLambdaContext()
            {
                Logger = mockLambdaLogger.Object
            };

            var sqsEvent = _fixture.Build<SQSEvent>()
                                   .With(x => x.Records, new List<SQSEvent.SQSMessage> { CreateMessage(id, eventType) })
                                   .Create();

            Func<Task> func = async () =>
            {
                var fn = new SqsFunction();
                await fn.FunctionHandler(sqsEvent, lambdaContext).ConfigureAwait(false);
            };

            _lastException = await Record.ExceptionAsync(func);
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
