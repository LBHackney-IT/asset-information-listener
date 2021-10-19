using Hackney.Shared.Tenure.Boundary.Response;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway.Interfaces
{
    public interface ITenureInfoApiGateway
    {
        Task<TenureResponseObject> GetTenureInfoByIdAsync(Guid id, Guid correlationId);
    }
}
