using AssetInformationListener.Domain;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway.Interfaces
{
    public interface IDbEntityGateway
    {
        Task<DomainEntity> GetEntityAsync(Guid id);
        Task SaveEntityAsync(DomainEntity entity);
    }
}
