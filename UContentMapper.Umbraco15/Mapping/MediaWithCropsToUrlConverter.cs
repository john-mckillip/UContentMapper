using UContentMapper.Core.Abstractions.Configuration;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class MediaWithCropsToUrlConverter : ITypeConverter<MediaWithCrops, string>
    {
        public string Convert(MediaWithCrops source)
        {
            return source?.Url() ?? string.Empty;
        }
    }
}