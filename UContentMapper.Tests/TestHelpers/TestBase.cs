using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;

namespace UContentMapper.Tests.TestHelpers;

/// <summary>
/// Base class for all unit tests providing common test infrastructure
/// </summary>
public abstract class TestBase
{
    protected IFixture Fixture { get; private set; }
    protected MockRepository MockRepository { get; private set; }
    protected IServiceCollection Services { get; private set; }
    protected FakeLogger Logger { get; private set; }

    [SetUp]
    public virtual void SetUp()
    {
        Fixture = new Fixture();
        MockRepository = new MockRepository(MockBehavior.Strict);
        Services = new ServiceCollection();
        Logger = new FakeLogger();
        
        ConfigureFixture();
        ConfigureServices();
    }

    [TearDown]
    public virtual void TearDown()
    {
        MockRepository.VerifyAll();
    }

    protected virtual void ConfigureFixture()
    {
        // Configure AutoFixture customizations here
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    protected virtual void ConfigureServices()
    {
        Services.AddLogging(builder => builder.AddProvider(new FakeLoggerProvider()));
    }

    protected Mock<T> CreateMock<T>() where T : class
    {
        return MockRepository.Create<T>();
    }

    protected Mock<T> CreateLooseMock<T>() where T : class
    {
        return new Mock<T>(MockBehavior.Loose);
    }

    protected IServiceProvider BuildServiceProvider()
    {
        return Services.BuildServiceProvider();
    }
}