using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class MediaItemResolver(string mediaPropertyAlias = "image")
                : UmbracoValueResolver<IPublishedContent, object, MediaWithCrops>
    {
        private readonly string _mediaPropertyAlias = mediaPropertyAlias;

        public override MediaWithCrops Resolve(IPublishedContent source, object destination, string memberName)
        {
            if (source is null || !source.HasProperty(_mediaPropertyAlias))
            {
                return null!;
            }

            var mediaItem = source.Value<IPublishedContent>(_mediaPropertyAlias);
            return mediaItem is not null ? new MediaWithCrops(mediaItem,null, null) : null!;
        }
    }
}
