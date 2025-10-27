using Microsoft.AspNetCore.Html;
using UContentMapper.Core.Models.Attributes;
using UContentMapper.Core.Models.Content;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Fixtures;

/// <summary>
/// Test model for unit tests
/// </summary>
[MapperConfiguration(SourceType = typeof(IPublishedContent), ContentTypeAlias = "testPage")]
public class TestPageModel : BaseContentModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishDate { get; set; }
    public List<string> Tags { get; set; } = [];
    public IHtmlContent? MainBody { get; set; }
}

/// <summary>
/// Test model without mapper configuration attribute
/// </summary>
public class SimpleTestModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Test model with complex properties
/// </summary>
[MapperConfiguration(SourceType = typeof(IPublishedContent), ContentTypeAlias = "complexPage")]
public class ComplexTestModel : BaseContentModel
{
    public string HeaderTitle { get; set; } = string.Empty;
    public TestImageModel? FeaturedImage { get; set; }
    public List<TestCategoryModel> Categories { get; set; } = new();
    public TestMetadataModel? Metadata { get; set; }
}

/// <summary>
/// Test image model
/// </summary>
public class TestImageModel
{
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// Test category model
/// </summary>
public class TestCategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}

/// <summary>
/// Test metadata model
/// </summary>
public class TestMetadataModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Keywords { get; set; } = new();
}

/// <summary>
/// Model with all supported primitive types for testing type conversion
/// </summary>
public class TypeConversionTestModel
{
    public string StringValue { get; set; } = string.Empty;
    public int IntValue { get; set; }
    public bool BoolValue { get; set; }
    public DateTime DateTimeValue { get; set; }
    public Guid GuidValue { get; set; }
    public double DoubleValue { get; set; }
    public decimal DecimalValue { get; set; }
    public float FloatValue { get; set; }
    public long LongValue { get; set; }
    public short ShortValue { get; set; }
    
    // Nullable types
    public int? NullableIntValue { get; set; }
    public bool? NullableBoolValue { get; set; }
    public DateTime? NullableDateTimeValue { get; set; }
    public Guid? NullableGuidValue { get; set; }
    public IHtmlContent? NullableHtmlContentValue { get; set; }
}

/// <summary>
/// Model with read-only properties to test mapping behavior
/// </summary>
public class ReadOnlyPropertiesTestModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReadOnlyProperty { get; } = "ReadOnly";
    public string WriteOnlyProperty { set => _writeOnlyValue = value; }
    
    private string _writeOnlyValue = string.Empty;
    
    public string GetWriteOnlyValue() => _writeOnlyValue;
}

/// <summary>
/// Model with indexer properties to test mapping behavior
/// </summary>
public class IndexerPropertiesTestModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    private readonly Dictionary<string, object> _properties = new();
    
    public object this[string key]
    {
        get => _properties.ContainsKey(key) ? _properties[key] : null!;
        set => _properties[key] = value;
    }
}

/// <summary>
/// Model for testing attribute-based configuration
/// </summary>
[MapperConfiguration(SourceType = typeof(IPublishedContent), ContentTypeAlias = "*")]
public class WildcardContentTypeModel : BaseContentModel
{
    public string CustomProperty { get; set; } = string.Empty;
}

/// <summary>
/// Model for testing wrong source type in attribute
/// </summary>
[MapperConfiguration(SourceType = typeof(string), ContentTypeAlias = "testPage")]
public class WrongSourceTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}