using System.Globalization;

namespace UContentMapper.Core.Configuration
{
    public class MapperConfigurationOptions
    {
        public bool EnableAttributeMapping { get; set; } = true;
        public bool EnablePropertyCache { get; set; } = true;
        public bool AutoMapUnmatchedProperties { get; set; } = true;
        public CultureInfo DefaultCulture { get; set; } = CultureInfo.CurrentCulture;
    }
}