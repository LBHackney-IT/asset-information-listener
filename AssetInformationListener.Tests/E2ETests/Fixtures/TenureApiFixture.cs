using AutoFixture;
using Hackney.Core.Sns;
using Hackney.Core.Testing.Shared.E2E;
using Hackney.Shared.Tenure.Boundary.Response;
using System;

namespace AssetInformationListener.Tests.E2ETests.Fixtures
{
    public class TenureApiFixture : BaseApiFixture<TenureResponseObject>
    {
        private readonly Fixture _fixture = new Fixture();
        private const string TenureApiRoute = "http://localhost:5678/api/v1/";
        private const string TenureApiToken = "sdjkhfgsdkjfgsdjfgh";

        public Guid RemovedPersonId { get; private set; }
        public EventData MessageEventData { get; private set; }

        public string ReceivedCorrelationId { get; private set; }

        public TenureApiFixture()
            : base(TenureApiRoute, TenureApiToken)
        {
            Environment.SetEnvironmentVariable("TenureApiUrl", TenureApiRoute);
            Environment.SetEnvironmentVariable("TenureApiToken", TenureApiToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                base.Dispose(disposing);
            }
        }

        public void GivenTheTenureDoesNotExist(Guid id)
        {
            // Nothing to do here
        }

        public TenureResponseObject GivenTheTenureExists(Guid id)
        {
            ResponseObject = _fixture.Build<TenureResponseObject>()
                                    .With(x => x.Id, id)
                                    .Create();
            return ResponseObject;
        }
    }
}
