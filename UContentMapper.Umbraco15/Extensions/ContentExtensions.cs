using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Core.Strings;

namespace UContentMapper.Umbraco15.Extensions
{
    public static class ContentExtensions
    {
        public static IHtmlContent ToHtmlContent(this HtmlEncodedString htmlEncodedString)
        {
            return new HtmlString(htmlEncodedString.ToHtmlString());
        }
    }
}