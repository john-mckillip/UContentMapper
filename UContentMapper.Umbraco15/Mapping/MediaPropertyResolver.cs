using UContentMapper.Core.Abstractions.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class MediaPropertyResolver<TValue>(string propertyAlias)
        : IValueResolver<IPublishedContent, TValue>
    {
        private readonly string _propertyAlias = propertyAlias;

        public TValue Resolve(IPublishedContent source)
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