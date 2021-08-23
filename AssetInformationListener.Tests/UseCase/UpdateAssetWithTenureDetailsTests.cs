using AssetInformationListener.Boundary;
using AssetInformationListener.Domain;
using AssetInformationListener.Domain.Tenure;
using AssetInformationListener.Gateway.Interfaces;
using AssetInformationListener.Infrastructure.Exceptions;
using AssetInformationListener.UseCase;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AssetInformationListener.Tests.UseCase
{
    [Collection("LogCall collection")]
    public class UpdateAssetWithTenureDetailsTests
    {
        private readonly Mock<IAssetGateway> _mockGateway;
        private readonly Mock<ITenureInfoApiGateway> _mockTenureApi;
        private readonly UpdateAssetWithTenureDetails _sut;

        private readonly EntityEventSns _message;
        private readonly TenureResponseObject _tenure;
        private readonly Asset _asset;

        private readonly Fixture _fixture;

        public UpdateAssetWithTenureDetailsTests()
        {
            _fixture = new Fixture();

            _mockGateway = new Mock<IAssetGateway>();
            _mockTenureApi = new Mock<ITenureInfoApiGateway>();
            _sut = new UpdateAssetWithTenureDetails(_mockGateway.Object, _mockTenureApi.Object);

            _message = CreateMessage();
            _tenure = CreateTenure(_message.EntityId);
            _asset = CreateAsset(_tenure.TenuredAsset.Id);
        }

        private EntityEventSns CreateMessage(string eventType = EventTypes.TenureCreatedEvent)
        {
            return _fixture.Build<EntityEventSns>()
                           .With(x => x.EventType, eventType)
                           .Create();
        }

        private TenureResponseObject CreateTenure(Guid entityId)
        {
            return _fixture.Build<TenureResponseObject>()
                           .With(x => x.Id, entityId)
                           .Create();
        }

        private Asset CreateAsset(Guid entityId)
        {
            return _fixture.Build<Asset>()
                           .With(x => x.Id, entityId)
                           .Create();
        }

        [Fact]
        public void ProcessMessageAsyncTestNullMessageThrows()
        {
            Func<Task> func = async () => await _sut.ProcessMessageAsync(null).ConfigureAwait(false);
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGetTenureExceptionThrown()
        {
            var exMsg = "This is an error";
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public void ProcessMessageAsyncTestGetTenureReturnsNullThrows()
        {
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ReturnsAsync((TenureResponseObject) null);

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<TenureNotFoundException>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAssetExceptionThrows()
        {
            var exMsg = "This is an new error";
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ReturnsAsync(_tenure);
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_tenure.TenuredAsset.Id))
                        .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAssetReturnsNullExceptionThrows()
        {
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ReturnsAsync(_tenure);
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_tenure.TenuredAsset.Id))
                        .ReturnsAsync((Asset) null);

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<TenureNotFoundException>();
        }

        [Fact]
        public void ProcessMessageAsyncTestUpdateAssetExceptionThrows()
        {
            var exMsg = "This is the last error";
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ReturnsAsync(_tenure);
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_tenure.TenuredAsset.Id))
                        .ReturnsAsync(_asset);
            _mockGateway.Setup(x => x.SaveAssetAsync(It.IsAny<Asset>()))
                        .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public async Task ProcessMessageAsyncTestSuccess()
        {
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId))
                                       .ReturnsAsync(_tenure);
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_tenure.TenuredAsset.Id))
                        .ReturnsAsync(_asset);

            await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);

            _mockGateway.Verify(x => x.SaveAssetAsync(It.Is<Asset>(y => VerifyUpdatedAsset(y, _tenure))),
                                Times.Once);
        }

        private bool VerifyUpdatedAsset(Asset updatedAsset, TenureResponseObject tenure)
        {
            var expectedTenureInfo = new AssetTenure
            {
                Id = tenure.Id.ToString(),
                PaymentReference = tenure.PaymentReference,
                Type = tenure.TenureType.Description,
                StartOfTenureDate = tenure.StartOfTenureDate,
                EndOfTenureDate = tenure.EndOfTenureDate
            };
            updatedAsset.Tenure.Should().BeEquivalentTo(expectedTenureInfo);
            return true;
        }
    }
}
