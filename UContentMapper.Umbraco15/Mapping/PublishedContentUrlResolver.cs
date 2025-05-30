using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="urlMode"></param>
    public class PublishedContentUrlResolver(UrlMode urlMode = UrlMode.Default)
                : UmbracoValueResolver<IPublishedContent, object, string>
    {
        private readonly UrlMode _urlMode = urlMode;

        public override string Resolve(IPublishedContent source, object destination, string memberName)
        {
            return source?.Url(null, _urlMode) ?? string.Empty;
        }
    }
}