using Amazon.DynamoDBv2.DataModel;
using AssetInformationListener.Infrastructure;
using AutoFixture;
using System;

namespace AssetInformationListener.Tests.E2ETests.Fixtures
{
    public class AssetFixture : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();

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

        private AssetDb ConstructAndSaveEntity(Guid id)
        {
            var dbEntity = _fixture.Build<AssetDb>()
                                 .With(x => x.Id, id)
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();

            _dbContext.SaveAsync<AssetDb>(dbEntity).GetAwaiter().GetResult();
            dbEntity.VersionNumber = 0;
            return dbEntity;
        }

        public void GivenAnAssetExists(Guid id)
        {
            if (null == DbAsset)
            {
                var tenure = ConstructAndSaveEntity(id);
                DbAsset = tenure;
                AssetDbId = tenure.Id;
            }
        }

        public void GivenAnAssetDoesNotExist(Guid id)
        {
            // Nothing to do here
        }
    }
}
