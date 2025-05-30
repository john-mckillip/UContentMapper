using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class MediaWithCropsToUrlConverter : UmbracoTypeConverter<MediaWithCrops, string>
    {
        public override string Convert(MediaWithCrops source)
        {
            return source?.Url() ?? string.Empty;
        }
    }
}