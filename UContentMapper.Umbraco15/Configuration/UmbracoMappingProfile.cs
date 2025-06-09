using UContentMapper.Core.Configuration.Profiles;
using UContentMapper.Core.Models.Content;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Configuration
{
    /// <summary>
    /// Umbraco-specific mappings to base models
    /// </summary>
    public class UmbracoMappingProfile : BaseMappingProfile
    {
        public UmbracoMappingProfile()
        {
            ConfigureBaseContentMappings();
            ConfigureBaseElementMappings();
            ConfigureMediaMappings();
        }

        private void ConfigureBaseContentMappings()
        {
            // Map IPublishedContent to your base content model
            CreateMap<IPublishedContent, BaseContentModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ContentTypeAlias, opt => opt.MapFrom(src => src.ContentType.Alias))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url(null, UrlMode.Default)))
                .ForMember(dest => dest.AbsoluteUrl, opt => opt.MapFrom(src => src.Url(null, UrlMode.Absolute)))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder))
                .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.IsVisible()))
                .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId));

            // Map to SEO base model if you have SEO properties
            CreateMap<IPublishedContent, BaseSeoModel>()
                .ForMember(dest => dest.MetaTitle, opt => opt.MapFromProperty("metaTitle"))
                .ForMember(dest => dest.MetaDescription, opt => opt.MapFromProperty("metaDescription"))
                .ForMember(dest => dest.MetaKeywords, opt => opt.MapFromProperty("metaKeywords"))
                .ForMember(dest => dest.OgTitle, opt => opt.MapFromProperty("ogTitle"))
                .ForMember(dest => dest.OgDescription, opt => opt.MapFromProperty("ogDescription"))
                .ForMember(dest => dest.OgImage, opt => opt.MapFromProperty("ogImage"))
                .ForMember(dest => dest.NoIndex, opt => opt.MapFromProperty("noIndex"));
        }

        private void ConfigureBaseElementMappings()
        {
            // Map IPublishedElement to base element model (for compositions/nested content)
            CreateMap<IPublishedElement, BaseElementModel>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.ContentTypeAlias, opt => opt.MapFrom(src => src.ContentType.Alias));
        }

        private void ConfigureMediaMappings()
        {
            // Map IPublishedContent (media) to media model
            CreateMap<IPublishedContent, BaseMediaModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url(null, UrlMode.Default)))
                .ForMember(dest => dest.Extension, opt => opt.MapFromProperty("umbracoExtension"))
                .ForMember(dest => dest.Bytes, opt => opt.MapFromProperty("umbracoBytes"))
                .ForMember(dest => dest.Width, opt => opt.MapFromProperty("umbracoWidth"))
                .ForMember(dest => dest.Height, opt => opt.MapFromProperty("umbracoHeight"));

            // Map MediaWithCrops to image model
            CreateMap<MediaWithCrops, ImageModel>()
                .ForMember(dest => dest.Src, opt => opt.MapFrom(src => src.Content.Url(null, UrlMode.Default)))
                .ForMember(dest => dest.Alt, opt => opt.MapFrom(src => src.Name));
                // ToDo: Figure out where to map these from
                //.ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.Width))
                //.ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height));
        }
    }
}