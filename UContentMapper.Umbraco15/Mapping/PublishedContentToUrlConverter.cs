using UContentMapper.Core.Abstractions.Configuration;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class PublishedContentToUrlConverter(UrlMode urlMode = UrlMode.Default) : ITypeConverter<IPublishedContent, string>
    {
        private readonly UrlMode _urlMode = urlMode;

        public string Convert(IPublishedContent source)
        {
            return source?.Url(null, _urlMode) ?? string.Empty;
        }
    }
}