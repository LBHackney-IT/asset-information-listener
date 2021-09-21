using AssetInformationListener.Boundary;
using AssetInformationListener.Factories;
using AssetInformationListener.UseCase.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace AssetInformationListener.Tests.Factories
{
    public class UseCaseFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();
        private EntityEventSns _event;
        private readonly Mock<IServiceProvider> _mockServiceProvider;

        public UseCaseFactoryTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();

            _event = ConstructEvent(EventTypes.TenureUpdatedEvent);
        }

        private EntityEventSns ConstructEvent(string eventType)
        {
            return _fixture.Build<EntityEventSns>()
                           .With(x => x.EventType, eventType)
                           .Create();
        }

        [Fact]
        public void CreateUseCaseForMessageTestNullEventThrows()
        {
            Action act = () => UseCaseFactory.CreateUseCaseForMessage(null, _mockServiceProvider.Object);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateUseCaseForMessageTestNullServiceProviderThrows()
        {
            Action act = () => UseCaseFactory.CreateUseCaseForMessage(_event, null);
            act.Should().Throw<ArgumentNullException>();
        }

        private void TestMessageProcessingCreation<T>(EntityEventSns eventObj) where T : class, IMessageProcessing
        {
            var mockProcessor = new Mock<T>();
            _mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(mockProcessor.Object);

            var result = UseCaseFactory.CreateUseCaseForMessage(eventObj, _mockServiceProvider.Object);
            result.Should().NotBeNull();
            _mockServiceProvider.Verify(x => x.GetService(typeof(T)), Times.Once);
        }

        [Fact]
        public void CreateUseCaseForMessageTestUnknownEventThrows()
        {
            _event = ConstructEvent("UnknownEvent");

            Action act = () => UseCaseFactory.CreateUseCaseForMessage(_event, _mockServiceProvider.Object);
            act.Should().Throw<ArgumentException>().WithMessage($"Unknown event type: {_event.EventType}");
            _mockServiceProvider.Verify(x => x.GetService(It.IsAny<Type>()), Times.Never);
        }

        [Theory]
        [InlineData(EventTypes.TenureCreatedEvent)]
        [InlineData(EventTypes.TenureUpdatedEvent)]
        public void CreateUseCaseForMessageTestTenureEvents(string eventType)
        {
            _event = ConstructEvent(eventType);
            TestMessageProcessingCreation<IUpdateAssetWithTenureDetails>(_event);
        }
    }
}
