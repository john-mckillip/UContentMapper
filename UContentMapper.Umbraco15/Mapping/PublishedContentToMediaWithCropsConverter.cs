using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Umbraco15.Mapping
{
    public class PublishedContentToMediaWithCropsConverter : UmbracoTypeConverter<IPublishedContent, MediaWithCrops>
    {
        public override MediaWithCrops Convert(IPublishedContent source)
        {
            return source != null ? new MediaWithCrops(source, null, null) : null!;
        }
    }
}