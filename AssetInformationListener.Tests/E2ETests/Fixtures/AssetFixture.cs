using Amazon.DynamoDBv2.DataModel;
using AssetInformationListener.Infrastructure;
using AutoFixture;
using Hackney.Shared.Tenure.Boundary.Response;
using System;

namespace AssetInformationListener.Tests.E2ETests.Fixtures
{
    public class AssetFixture : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private const string DateFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffZ";

        private readonly IDynamoDBContext _dbContext;

        public AssetDb DbAsset { get; private set; }
        public Guid AssetDbId { get; private set; }

        public AssetFixture(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != DbAsset)
                    _dbContext.DeleteAsync<AssetDb>(DbAsset.Id).GetAwaiter().GetResult();

                _disposed = true;
            }
        }

        private AssetDb ConstructAndSaveAssetDb(Guid id)
        {
            var dbEntity = ConstructAssetDb(id);
            return SaveAssetDb(dbEntity);
        }

        private AssetDb SaveAssetDb(AssetDb dbEntity)
        {
            _dbContext.SaveAsync<AssetDb>(dbEntity).GetAwaiter().GetResult();
            dbEntity.VersionNumber = 0;
            return dbEntity;
        }

        private AssetDb ConstructAssetDb(Guid id)
        {
            return _fixture.Build<AssetDb>()
                                 .With(x => x.Id, id)
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
        }

        public void GivenAnAssetExists(Guid id)
        {
            if (null == DbAsset)
            {
                var asset = ConstructAndSaveAssetDb(id);
                DbAsset = asset;
                AssetDbId = asset.Id;
            }
        }

        public void GivenAnAssetExistsWithTenureInfo(TenureResponseObject tenure)
        {
            if (null == DbAsset)
            {
                var asset = ConstructAssetDb(tenure.TenuredAsset.Id);
                asset.Tenure = new AssetTenureDb()
                {
                    Id = tenure.Id.ToString(),
                    EndOfTenureDate = null,
                    PaymentReference = "something",
                    StartOfTenureDate = DateTime.UtcNow.AddYears(-2),
                    Type = tenure.TenureType.Description
                };
                asset = SaveAssetDb(asset);
                DbAsset = asset;
                AssetDbId = asset.Id;
            }
        }

        public void GivenAnAssetDoesNotExist(Guid id)
        {
            // Nothing to do here
        }
    }
}
