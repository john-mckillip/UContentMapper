using System.Globalization;

namespace UContentMapper.Core.Configuration
{
    /// <summary>
    /// Provides configuration options for controlling the behavior of a mapping operation.
    /// </summary>
    /// <remarks>These options allow customization of how mappings are performed, including enabling or
    /// disabling attribute-based mapping, property caching, and automatic handling of unmatched properties.
    /// Additionally, the default culture for mapping operations can be specified.</remarks>
    public class MapperConfigurationOptions
    {
        public bool EnableAttributeMapping { get; set; } = true;
        public bool EnablePropertyCache { get; set; } = true;
        public bool AutoMapUnmatchedProperties { get; set; } = true;
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;
    }
}