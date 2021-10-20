using AutoFixture;
using AssetInformationListener.Domain;
using AssetInformationListener.Factories;
using AssetInformationListener.Infrastructure;
using FluentAssertions;
using Xunit;

namespace AssetInformationListener.Tests.Factories
{
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void CanMapANullAssetDbToAnAsset()
        {
            AssetDb databaseEntity = null;
            var entity = databaseEntity.ToDomain();
            entity.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanMapAnAssetDbToAnAsset(bool nullEndDate)
        {
            var databaseEntity = _fixture.Create<AssetDb>();
            if (nullEndDate)
                databaseEntity.Tenure.EndOfTenureDate = null;

            var entity = databaseEntity.ToDomain();
            databaseEntity.Should().BeEquivalentTo(entity, config => config.Excluding(x => x.Tenure.IsActive));
        }

        [Fact]
        public void CanMapANullAssetToAnAssetDb()
        {
            Asset entity = null;
            var databaseEntity = entity.ToDatabase();
            databaseEntity.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanMapAnAssetToAnAssetDb(bool nullEndDate)
        {
            var entity = _fixture.Create<Asset>();
            if (nullEndDate)
                entity.Tenure.EndOfTenureDate = null;

            var databaseEntity = entity.ToDatabase();

            entity.Should().BeEquivalentTo(databaseEntity);
        }


        [Fact]
        public void CanMapANullAssetTenureDbToAnAssetTenure()
        {
            AssetTenureDb databaseEntity = null;
            var entity = databaseEntity.ToDomain();
            entity.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanMapAnAssetTenureDbToAnAssetTenure(bool nullEndDate)
        {
            var databaseEntity = _fixture.Create<AssetTenureDb>();
            if (nullEndDate)
                databaseEntity.EndOfTenureDate = null;

            var entity = databaseEntity.ToDomain();
            databaseEntity.Should().BeEquivalentTo(entity, config => config.Excluding(x => x.IsActive));
        }

        [Fact]
        public void CanMapANullAssetTenureToAnAssetTenureDb()
        {
            AssetTenure entity = null;
            var databaseEntity = entity.ToDatabase();
            databaseEntity.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void CanMapAnAssetTenureToAnAssetTenureDb(bool nullEndDate)
        {
            var entity = _fixture.Create<AssetTenure>();
            if (nullEndDate)
                entity.EndOfTenureDate = null;

            var databaseEntity = entity.ToDatabase();

            entity.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
