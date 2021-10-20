using AssetInformationListener.Domain;
using AssetInformationListener.Domain.Account;
using AssetInformationListener.Gateway.Interfaces;
using AssetInformationListener.Infrastructure.Exceptions;
using AssetInformationListener.UseCase;
using AutoFixture;
using FluentAssertions;
using Hackney.Core.Sns;
using Hackney.Shared.Tenure.Boundary.Response;
using Hackney.Shared.Tenure.Domain;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AssetInformationListener.Tests.UseCase
{
    [Collection("LogCall collection")]
    public class UpdateAccountDetailsOnAssetTenureTests
    {
        private readonly Mock<IAssetGateway> _mockGateway;
        private readonly Mock<ITenureInfoApiGateway> _mockTenureApi;
        private readonly Mock<IAccountApi> _mockAccountApi;
        private readonly UpdateAccountDetailsOnAssetTenure _sut;

        private readonly EntityEventSns _message;
        private readonly AccountResponseObject _account;
        private readonly TenureResponseObject _tenure;
        private readonly Asset _asset;

        private readonly Fixture _fixture;
        private static readonly Guid _correlationId = Guid.NewGuid();
        private const string DateTimeFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffZ";

        public UpdateAccountDetailsOnAssetTenureTests()
        {
            _fixture = new Fixture();

            _mockGateway = new Mock<IAssetGateway>();
            _mockTenureApi = new Mock<ITenureInfoApiGateway>();
            _mockAccountApi = new Mock<IAccountApi>();
            _sut = new UpdateAccountDetailsOnAssetTenure(_mockGateway.Object, _mockAccountApi.Object, _mockTenureApi.Object);

            _account = CreateAccount();
            _tenure = CreateTenure(_account.TargetId);
            _asset = CreateAsset(_tenure.TenuredAsset.Id);
            _message = CreateMessage(_account.Id);

            _mockAccountApi.Setup(x => x.GetAccountByIdAsync(_message.EntityId, _message.CorrelationId))
                           .ReturnsAsync(_account);

            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_account.TargetId, _message.CorrelationId))
                                       .ReturnsAsync(_tenure);

            _mockGateway.Setup(x => x.GetAssetByIdAsync(_asset.Id)).ReturnsAsync(_asset);
        }

        private AccountResponseObject CreateAccount()
        {
            return _fixture.Build<AccountResponseObject>()
                           .With(x => x.StartDate, DateTime.UtcNow.AddMonths(-1).ToString(DateTimeFormat))
                           .Create();
        }

        private TenureResponseObject CreateTenure(Guid entityId)
        {
            return _fixture.Build<TenureResponseObject>()
                           .With(x => x.Id, entityId)
                           .With(x => x.HouseholdMembers, _fixture.Build<HouseholdMembers>()
                                                                  .With(x => x.PersonTenureType, PersonTenureType.Tenant)
                                                                  .CreateMany(3)
                                                                  .ToList())
                           .Create();
        }

        private Asset CreateAsset(Guid id)
        {
            return _fixture.Build<Asset>()
                           .With(x => x.Id, id)
                           .Create();
        }

        private EntityEventSns CreateMessage(Guid tenureId, string eventType = EventTypes.AccountCreatedEvent)
        {
            return _fixture.Build<EntityEventSns>()
                           .With(x => x.EventType, eventType)
                           .With(x => x.EntityId, tenureId)
                           .With(x => x.CorrelationId, _correlationId)
                           .Create();
        }

        [Fact]
        public void ProcessMessageAsyncTestNullMessageThrows()
        {
            Func<Task> func = async () => await _sut.ProcessMessageAsync(null).ConfigureAwait(false);
            func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAccountExceptionThrown()
        {
            var exMsg = "This is an error";
            _mockAccountApi.Setup(x => x.GetAccountByIdAsync(_message.EntityId, _message.CorrelationId))
                           .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAccountReturnsNullThrows()
        {
            _mockAccountApi.Setup(x => x.GetAccountByIdAsync(_message.EntityId, _message.CorrelationId))
                           .ReturnsAsync((AccountResponseObject) null);

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<EntityNotFoundException<AccountResponseObject>>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGetTenureExceptionThrown()
        {
            var exMsg = "This is an error";
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId, _message.CorrelationId))
                          .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public void ProcessMessageAsyncTestGetTenureReturnsNullThrows()
        {
            _mockTenureApi.Setup(x => x.GetTenureInfoByIdAsync(_message.EntityId, _message.CorrelationId))
                          .ReturnsAsync((TenureResponseObject) null);

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<EntityNotFoundException<TenureResponseObject>>();
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAssetExceptionThrow()
        {
            var exMsg = "This is an error";
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_asset.Id))
                        .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);
        }

        [Fact]
        public void ProcessMessageAsyncTestGetAssetReturnsNullThrows()
        {
            _mockGateway.Setup(x => x.GetAssetByIdAsync(_asset.Id))
                        .ReturnsAsync((Asset) null);

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<EntityNotFoundException<Asset>>();
        }

        [Fact]
        public async Task ProcessMessageAsyncTestNoChangesNeededDoesNothing()
        {
            _asset.Tenure.PaymentReference = _account.PaymentReference;
            await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);

            _mockGateway.Verify(x => x.SaveAssetAsync(It.IsAny<Asset>()), Times.Never());
        }

        [Fact]
        public void ProcessMessageAsyncTestSaveAssetExceptionThrown()
        {
            var exMsg = "Some error";
            _mockGateway.Setup(x => x.SaveAssetAsync(It.IsAny<Asset>()))
                        .ThrowsAsync(new Exception(exMsg));

            Func<Task> func = async () => await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);
            func.Should().ThrowAsync<Exception>().WithMessage(exMsg);

            _mockGateway.Verify(x => x.SaveAssetAsync(It.IsAny<Asset>()), Times.Once());
        }

        [Fact]
        public async Task ProcessMessageAsyncTestAssetUpdated()
        {
            await _sut.ProcessMessageAsync(_message).ConfigureAwait(false);

            _mockGateway.Verify(x => x.SaveAssetAsync(It.Is<Asset>(y => VerifyPersonTenureUpdated(y, _asset, _account))),
                                Times.Once());
        }

        private bool VerifyPersonTenureUpdated(Asset updated, Asset original, AccountResponseObject account)
        {
            updated.Should().BeEquivalentTo(original, c => c.Excluding(x => x.Tenure));
            updated.Tenure.Should().BeEquivalentTo(original.Tenure, c => c.Excluding(x => x.PaymentReference));
            updated.Tenure.PaymentReference.Should().Be(account.PaymentReference);

            return true;
        }
    }

}
