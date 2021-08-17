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
        public void CanMapADatabaseEntityToADomainObject()
        {
            var databaseEntity = _fixture.Create<DbEntity>();
            var entity = databaseEntity.ToDomain();

            databaseEntity.Should().BeEquivalentTo(entity);
        }

        [Fact]
        public void CanMapADomainEntityToADatabaseObject()
        {
            var entity = _fixture.Create<DomainEntity>();
            var databaseEntity = entity.ToDatabase();

            databaseEntity.Should().BeEquivalentTo(entity);
        }
    }
}
