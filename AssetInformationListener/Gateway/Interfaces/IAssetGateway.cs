using Hackney.Shared.Asset.Domain;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway.Interfaces
{
    public interface IAssetGateway
    {
        Task<Asset> GetAssetByIdAsync(Guid id);
        Task SaveAssetAsync(Asset entity);
    }
}
