using Hackney.Core.Sns;
using System.Threading.Tasks;

namespace AssetInformationListener.UseCase.Interfaces
{
    public interface IMessageProcessing
    {
        Task ProcessMessageAsync(EntityEventSns message);
    }
}
