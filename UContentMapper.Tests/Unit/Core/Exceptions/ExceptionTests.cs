using FluentAssertions;
using UContentMapper.Core.Exceptions;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Exceptions;

[TestFixture]
public class ExceptionTests : TestBase
{
    [Test]
    public void ConfigurationException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new ConfigurationException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Test]
    public void ConfigurationException_ShouldHaveParameterlessConstructor()
    {
        // Arrange & Act
        var exception = new ConfigurationException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void ConfigurationException_ShouldHaveMessageConstructor()
    {
        // Arrange
        var message = "Configuration error occurred";

        // Act
        var exception = new ConfigurationException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Test]
    public void ConfigurationException_ShouldHaveMessageAndInnerExceptionConstructor()
    {
        // Arrange
        var message = "Configuration error occurred";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new ConfigurationException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Test]
    public void MappingException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new MappingException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Test]
    public void MappingException_ShouldHaveParameterlessConstructor()
    {
        // Arrange & Act
        var exception = new MappingException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void MappingException_ShouldHaveMessageConstructor()
    {
        // Arrange
        var message = "Mapping error occurred";

        // Act
        var exception = new MappingException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Test]
    public void MappingException_ShouldHaveMessageAndInnerExceptionConstructor()
    {
        // Arrange
        var message = "Mapping error occurred";
        var innerException = new ArgumentException("Inner error");

        // Act
        var exception = new MappingException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Test]
    public void PropertyMappingException_ShouldInheritFromMappingException()
    {
        // Arrange & Act
        var exception = new PropertyMappingException();

        // Assert
        exception.Should().BeAssignableTo<MappingException>();
    }

    [Test]
    public void PropertyMappingException_ShouldHaveParameterlessConstructor()
    {
        // Arrange & Act
        var exception = new PropertyMappingException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void PropertyMappingException_ShouldHaveMessageConstructor()
    {
        // Arrange
        var message = "Property mapping error occurred";

        // Act
        var exception = new PropertyMappingException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Test]
    public void PropertyMappingException_ShouldHaveMessageAndInnerExceptionConstructor()
    {
        // Arrange
        var message = "Property mapping error occurred";
        var innerException = new FormatException("Inner error");

        // Act
        var exception = new PropertyMappingException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Test]
    public void ConfigurationException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new ConfigurationException("Test message");

        // Act & Assert
        exception.Should().BeBinarySerializable();
    }

    [Test]
    public void MappingException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new MappingException("Test message");

        // Act & Assert
        exception.Should().BeBinarySerializable();
    }

    [Test]
    public void PropertyMappingException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new PropertyMappingException("Test message");

        // Act & Assert
        exception.Should().BeBinarySerializable();
    }

    [Test]
    public void ConfigurationException_ShouldHandleNullMessage()
    {
        // Arrange & Act
        var exception = new ConfigurationException(null!);

        // Assert
        exception.Message.Should().NotBeNull();
    }

    [Test]
    public void ConfigurationException_ShouldHandleEmptyMessage()
    {
        // Arrange
        var message = "";

        // Act
        var exception = new ConfigurationException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Test]
    public void MappingException_ShouldHandleNullInnerException()
    {
        // Arrange
        var message = "Test message";

        // Act
        var exception = new MappingException(message, null);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }
}