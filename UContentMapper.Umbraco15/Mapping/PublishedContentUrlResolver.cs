using UContentMapper.Core.Abstractions.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="urlMode"></param>
    public class PublishedContentUrlResolver(UrlMode urlMode = UrlMode.Default)
                : IValueResolver<IPublishedContent, string>
    {
        private readonly UrlMode _urlMode = urlMode;

        public string Resolve(IPublishedContent source)
        {
            return source?.Url(null, _urlMode) ?? string.Empty;
        }
    }
}