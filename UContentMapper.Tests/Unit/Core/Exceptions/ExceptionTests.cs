using FluentAssertions;
using UContentMapper.Core.Exceptions;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Exceptions;

[TestFixture]
public class ExceptionTests : TestBase
{
    #region ConfigurationException Tests
    
    [Test]
    public void ConfigurationException_ShouldInheritFromMappingException()
    {
        // Arrange & Act
        var exception = new ConfigurationException("Test message");

        // Assert
        exception.Should().BeAssignableTo<MappingException>();
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
    public void ConfigurationException_ShouldHaveMessageAndValidationErrorsConstructor()
    {
        // Arrange
        var message = "Configuration error occurred";
        var validationErrors = new List<string> { "Error 1", "Error 2" };

        // Act
        var exception = new ConfigurationException(message, validationErrors);

        // Assert
        exception.Message.Should().Be(message);
        exception.ValidationErrors.Should().NotBeNull();
        exception.ValidationErrors.Should().BeEquivalentTo(validationErrors);
    }

    [Test]
    public void ConfigurationException_ValidationErrorsCanBeNull()
    {
        // Arrange
        var message = "Configuration error occurred";
        var validationErrors = (IEnumerable<string>)null!;

        // Act
        var exception = new ConfigurationException(message, validationErrors);

        // Assert
        exception.Message.Should().Be(message);
        exception.ValidationErrors.Should().BeNull();
    }
    
    #endregion

    #region MappingException Tests

    [Test]
    public void MappingException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new MappingException("Test message");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
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
    
    #endregion

    #region PropertyMappingException Tests

    [Test]
    public void PropertyMappingException_ShouldInheritFromMappingException()
    {
        // Arrange & Act
        var exception = new PropertyMappingException(
            "Property mapping error", 
            "testProperty", 
            typeof(string), 
            "TestMember");

        // Assert
        exception.Should().BeAssignableTo<MappingException>();
    }

    [Test]
    public void PropertyMappingException_ShouldHaveAllRequiredParameters()
    {
        // Arrange
        var message = "Property mapping error occurred";
        var propertyAlias = "testProperty";
        var destinationType = typeof(string);
        var memberName = "TestMember";

        // Act
        var exception = new PropertyMappingException(message, propertyAlias, destinationType, memberName);

        // Assert
        exception.Message.Should().Be(message);
        exception.PropertyAlias.Should().Be(propertyAlias);
        exception.DestinationType.Should().Be(destinationType);
        exception.DestinationTypeName.Should().Be(destinationType.FullName);
        exception.MemberName.Should().Be(memberName);
    }

    [Test]
    public void PropertyMappingException_ShouldSupportStringBasedConstructor()
    {
        // Arrange
        var message = "Property mapping error occurred";
        var propertyAlias = "testProperty";
        var destinationTypeName = "System.String";
        var memberName = "TestMember";

        // Act
        var exception = new PropertyMappingException(message, propertyAlias, destinationTypeName, memberName);

        // Assert
        exception.Message.Should().Be(message);
        exception.PropertyAlias.Should().Be(propertyAlias);
        exception.DestinationTypeName.Should().Be(destinationTypeName);
        exception.DestinationType.Should().BeNull();
        exception.MemberName.Should().Be(memberName);
    }

    #endregion

    #region Serialization Tests

    [Test]
    public void ConfigurationException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new ConfigurationException("Test message");

        // Act & Assert
        exception.Should().BeJsonSerializable<ConfigurationException>();
    }

    [Test]
    public void MappingException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new MappingException("Test message");

        // Act & Assert
        exception.Should().BeJsonSerializable<MappingException>();
    }

    [Test]
    public void PropertyMappingException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new PropertyMappingException(
            "Test message",
            "testProperty",
            "System.String",
            "TestMember");

        // Act & Assert
        exception.Should().BeJsonSerializable<PropertyMappingException>();
    }

    #endregion

    #region Edge Cases

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
    
    #endregion
}