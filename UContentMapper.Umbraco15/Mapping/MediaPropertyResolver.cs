using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class MediaPropertyResolver<TValue>(string propertyAlias)
        : UmbracoValueResolver<IPublishedContent, object, TValue>
    {
        private readonly string _propertyAlias = propertyAlias;

        public override TValue Resolve(IPublishedContent source, object destination, string memberName)
        {
            return source is null
                ? default!
                : source.HasProperty(_propertyAlias)
                    ? source.Value<TValue>(_propertyAlias) ?? default!
                    : default!;
        }
    }
}