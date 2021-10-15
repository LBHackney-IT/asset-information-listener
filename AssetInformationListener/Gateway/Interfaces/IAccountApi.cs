using AssetInformationListener.Domain.Account;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway.Interfaces
{
    public interface IAccountApi
    {
        Task<AccountResponseObject> GetAccountByIdAsync(Guid id, Guid correlationId);
    }
}
