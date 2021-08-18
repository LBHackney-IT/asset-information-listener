using AssetInformationListener.Infrastructure.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace AssetInformationListener.Tests.Infrastructure.Exceptions
{
    public class AssetNotFoundExceptionTests
    {
        [Fact]
        public void AssetNotFoundExceptionConstructorTest()
        {
            var id = Guid.NewGuid();

            var ex = new AssetNotFoundException(id);
            ex.Id.Should().Be(id);
            ex.EntityName.Should().Be("Asset");
            ex.Message.Should().Be($"Asset with id {id} not found.");
        }
    }
}
