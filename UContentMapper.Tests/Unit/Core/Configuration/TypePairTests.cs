using FluentAssertions;
using UContentMapper.Core.Configuration;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Configuration;

[TestFixture]
public class TypePairTests : TestBase
{
    [Test]
    public void TypePair_ShouldInitializeWithBothTypes()
    {
        // Arrange
        var sourceType = typeof(string);
        var destinationType = typeof(int);

        // Act
        var typePair = new TypePair(sourceType, destinationType);

        // Assert
        typePair.SourceType.Should().Be(sourceType);
        typePair.DestinationType.Should().Be(destinationType);
    }

    [Test]
    public void TypePair_ShouldHandleNullSourceType()
    {
        // Arrange & Act
        var typePair = new TypePair(null!, typeof(int));

        // Assert
        typePair.SourceType.Should().BeNull();
        typePair.DestinationType.Should().Be(typeof(int));
    }

    [Test]
    public void TypePair_ShouldHandleNullDestinationType()
    {
        // Arrange & Act
        var typePair = new TypePair(typeof(string), null!);

        // Assert
        typePair.SourceType.Should().Be(typeof(string));
        typePair.DestinationType.Should().BeNull();
    }

    [Test]
    public void TypePair_ShouldHandleBothNullTypes()
    {
        // Arrange & Act
        var typePair = new TypePair(null!, null!);

        // Assert
        typePair.SourceType.Should().BeNull();
        typePair.DestinationType.Should().BeNull();
    }

    [Test]
    public void TypePair_ShouldBeEqualWhenTypesMatch()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(string), typeof(int));

        // Act & Assert
        typePair1.Should().Be(typePair2);
        typePair1.Equals(typePair2).Should().BeTrue();
        (typePair1 == typePair2).Should().BeTrue();
        (typePair1 != typePair2).Should().BeFalse();
    }

    [Test]
    public void TypePair_ShouldNotBeEqualWhenSourceTypesDiffer()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(double), typeof(int));

        // Act & Assert
        typePair1.Should().NotBe(typePair2);
        typePair1.Equals(typePair2).Should().BeFalse();
        (typePair1 == typePair2).Should().BeFalse();
        (typePair1 != typePair2).Should().BeTrue();
    }

    [Test]
    public void TypePair_ShouldNotBeEqualWhenDestinationTypesDiffer()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(string), typeof(double));

        // Act & Assert
        typePair1.Should().NotBe(typePair2);
        typePair1.Equals(typePair2).Should().BeFalse();
        (typePair1 == typePair2).Should().BeFalse();
        (typePair1 != typePair2).Should().BeTrue();
    }

    [Test]
    public void TypePair_ShouldNotBeEqualToNull()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));

        // Act & Assert
        typePair.Should().NotBe(null);
        typePair.Equals(null).Should().BeFalse();
        (typePair == null).Should().BeFalse();
        (typePair != null).Should().BeTrue();
    }

    [Test]
    public void TypePair_ShouldNotBeEqualToDifferentType()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));
        var differentObject = "not a type pair";

        // Act & Assert
        typePair.Equals(differentObject).Should().BeFalse();
    }

    [Test]
    public void TypePair_ShouldHaveConsistentHashCode()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(string), typeof(int));

        // Act & Assert
        typePair1.GetHashCode().Should().Be(typePair2.GetHashCode());
    }

    [Test]
    public void TypePair_ShouldHaveDifferentHashCodeForDifferentTypes()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(double), typeof(int));

        // Act & Assert
        typePair1.GetHashCode().Should().NotBe(typePair2.GetHashCode());
    }

    [Test]
    public void TypePair_ShouldHandleNullTypesInHashCode()
    {
        // Arrange & Act
        var typePair = new TypePair(null!, null!);
        var hashCode = typePair.GetHashCode();

        // Assert
        hashCode.Should().Be(0); // Expected behavior for null types
    }

    [Test]
    public void TypePair_ShouldProvideStringRepresentation()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));

        // Act
        var stringRepresentation = typePair.ToString();

        // Assert
        stringRepresentation.Should().Contain("String");
        stringRepresentation.Should().Contain("Int32");
    }

    [Test]
    public void TypePair_ShouldHandleGenericTypes()
    {
        // Arrange
        var sourceType = typeof(List<string>);
        var destinationType = typeof(IEnumerable<int>);

        // Act
        var typePair = new TypePair(sourceType, destinationType);

        // Assert
        typePair.SourceType.Should().Be(sourceType);
        typePair.DestinationType.Should().Be(destinationType);
    }

    [Test]
    public void TypePair_ShouldWorkAsHashSetKey()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(string), typeof(int));
        var typePair3 = new TypePair(typeof(double), typeof(int));
        
        var hashSet = new HashSet<TypePair>();

        // Act
        hashSet.Add(typePair1);
        hashSet.Add(typePair2); // Should not be added as it's equal to typePair1
        hashSet.Add(typePair3);

        // Assert
        hashSet.Should().HaveCount(2);
        hashSet.Should().Contain(typePair1);
        hashSet.Should().Contain(typePair3);
    }
}