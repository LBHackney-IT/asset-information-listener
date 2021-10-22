using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace AssetInformationListener.Tests
{
    public class MockApplicationFactory
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef
            {
                Name = "Assets",
                KeyName = "id",
                KeyType = ScalarAttributeType.S,
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>(new[]
                {
                    new GlobalSecondaryIndex
                    {
                        IndexName = "AssetParentsAndChilds",
                        KeySchema = new List<KeySchemaElement>(new[]
                        {
                            new KeySchemaElement("rootAsset", KeyType.HASH),
                            new KeySchemaElement("parentAssetIds", KeyType.RANGE)
                        }),
                        Projection = new Projection { ProjectionType = ProjectionType.ALL },
                        ProvisionedThroughput = new ProvisionedThroughput(10 , 10)
                    }
                })
            }
        };

        public IDynamoDbFixture DynamoDbFixture { get; private set; }

        public MockApplicationFactory()
        {
            CreateHostBuilder(null).Build();
        }

        public IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
           .ConfigureServices((hostContext, services) =>
           {
               EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
               EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

               services.ConfigureDynamoDB();
               services.ConfigureDynamoDbFixture();

               var serviceProvider = services.BuildServiceProvider();

               LogCallAspectFixture.SetupLogCallAspect();

               DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
               DynamoDbFixture.EnsureTablesExist(_tables);
           });

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }
    }
}
