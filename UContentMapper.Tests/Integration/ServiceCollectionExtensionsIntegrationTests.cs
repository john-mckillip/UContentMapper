using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Configuration;
using UContentMapper.Umbraco15.Extensions;
using UContentMapper.Umbraco15.Mapping;

namespace UContentMapper.Tests.Integration;

[TestFixture]
public class ServiceCollectionExtensionsIntegrationTests : TestBase
{
    private IServiceCollection _services;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _services = new ServiceCollection();
        _services.AddLogging();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterAllRequiredServices()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IMappingConfiguration>().Should().NotBeNull();
        serviceProvider.GetService<IMappingConfiguration>().Should().BeOfType<UmbracoMappingConfiguration>();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterGenericContentMapper()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<IContentMapper<TestPageModel>>();
        mapper.Should().NotBeNull();
        mapper.Should().BeOfType<UmbracoContentMapper<TestPageModel>>();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterDifferentGenericMappers()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var pageMapper = serviceProvider.GetService<IContentMapper<TestPageModel>>();
        var simpleMapper = serviceProvider.GetService<IContentMapper<SimpleTestModel>>();

        pageMapper.Should().NotBeNull();
        simpleMapper.Should().NotBeNull();
        pageMapper.Should().NotBeSameAs(simpleMapper);
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterTypeConverters()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var urlConverter = serviceProvider.GetService<PublishedContentToUrlConverter>();
        var cropConverter = serviceProvider.GetService<MediaWithCropsToUrlConverter>();

        urlConverter.Should().NotBeNull();
        cropConverter.Should().NotBeNull();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterValueResolvers()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var urlResolver = serviceProvider.GetService<PublishedContentUrlResolver>();

        urlResolver.Should().NotBeNull();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterMappingProfile()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var profile = serviceProvider.GetService<UmbracoMappingProfile>();
        profile.Should().NotBeNull();
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterSingletonConfiguration()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var config1 = serviceProvider.GetService<IMappingConfiguration>();
        var config2 = serviceProvider.GetService<IMappingConfiguration>();

        config1.Should().BeSameAs(config2);
    }

    [Test]
    public void AddUContentMapper_ShouldRegisterTransientMappers()
    {
        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var mapper1 = serviceProvider.GetService<IContentMapper<TestPageModel>>();
        var mapper2 = serviceProvider.GetService<IContentMapper<TestPageModel>>();

        mapper1.Should().NotBeSameAs(mapper2);
    }

    [Test]
    public void AddUContentMapper_WithExistingServices_ShouldNotConflict()
    {
        // Arrange
        _services.AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<ServiceCollectionExtensionsIntegrationTests>>());

        // Act
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<IContentMapper<TestPageModel>>();
        var logger = serviceProvider.GetService<ILogger>();

        mapper.Should().NotBeNull();
        logger.Should().NotBeNull();
    }

    [Test]
    public void AddUContentMapper_CalledMultipleTimes_ShouldNotCauseDuplicates()
    {
        // Act
        _services.AddUContentMapper();
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var configurations = serviceProvider.GetServices<IMappingConfiguration>();
        configurations.Should().HaveCount(1);
    }

    [Test]
    public void IntegrationTest_FullMappingWorkflow_ShouldWork()
    {
        // Arrange
        _services.AddUContentMapper();
        var serviceProvider = _services.BuildServiceProvider();
        
        var mapper = serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        var content = TestDataBuilder.CreatePublishedContentWithBuiltInProperties();

        // Act
        var canMap = mapper.CanMap(content);
        var result = mapper.Map(content);

        // Assert
        canMap.Should().BeTrue();
        result.Should().NotBeNull();
        result.Should().BeOfType<TestPageModel>();
        result.Id.Should().Be(content.Id);
        result.Name.Should().Be(content.Name);
    }
}