using AssetInformationListener.Domain.Account;
using AssetInformationListener.Gateway.Interfaces;
using Hackney.Core.Http;
using Hackney.Core.Logging;
using System;
using System.Threading.Tasks;

namespace AssetInformationListener.Gateway
{
    public class AccountApi : IAccountApi
    {
        private const string ApiName = "Account";
        private const string AccountApiUrl = "AccountApiUrl";
        private const string AccountApiToken = "AccountApiToken";

        private readonly IApiGateway _apiGateway;

        public AccountApi(IApiGateway apiGateway)
        {
            _apiGateway = apiGateway;
            _apiGateway.Initialise(ApiName, AccountApiUrl, AccountApiToken);
        }

        [LogCall]
        public async Task<AccountResponseObject> GetAccountByIdAsync(Guid id, Guid correlationId)
        {
            var route = $"{_apiGateway.ApiRoute}/accounts/{id}";
            return await _apiGateway.GetByIdAsync<AccountResponseObject>(route, id, correlationId);
        }
    }
}
