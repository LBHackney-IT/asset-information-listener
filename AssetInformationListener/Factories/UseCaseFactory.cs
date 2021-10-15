using AssetInformationListener.UseCase.Interfaces;
using Hackney.Core.Sns;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AssetInformationListener.Factories
{
    public static class UseCaseFactory
    {
        public static IMessageProcessing CreateUseCaseForMessage(this EntityEventSns entityEvent, IServiceProvider serviceProvider)
        {
            if (entityEvent is null) throw new ArgumentNullException(nameof(entityEvent));
            if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));

            IMessageProcessing processor = null;
            switch (entityEvent.EventType)
            {
                case EventTypes.TenureCreatedEvent:
                case EventTypes.TenureUpdatedEvent:
                    {
                        processor = serviceProvider.GetService<IUpdateAssetWithTenureDetails>();
                        break;
                    }
                case EventTypes.AccountCreatedEvent:
                    {
                        processor = serviceProvider.GetService<IUpdateAccountDetailsOnAssetTenure>();
                        break;
                    }

                default:
                    throw new ArgumentException($"Unknown event type: {entityEvent.EventType}");
            }

            return processor;
        }
    }
}
