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
            if (source is null)
            {
                return default!;
            }

            TValue propertyValue = default!;
            if (source.HasProperty(_propertyAlias))
            {
                propertyValue = source.Value<TValue>(_propertyAlias) ?? default!;
            }

            return propertyValue;
        }
    }
}