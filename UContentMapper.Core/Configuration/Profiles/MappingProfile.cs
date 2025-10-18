using UContentMapper.Core.Abstractions.Configuration;

namespace UContentMapper.Core.Configuration.Profiles
{
    public abstract class MappingProfile
    {
        protected IMappingConfiguration Configuration { get; private set; } = null!;

        public void Initialize(IMappingConfiguration configuration)
        {
            Configuration = configuration;
            Configure(); // Call Configure() after setting Configuration
        }

        public virtual void Configure()
        {
            // Override in derived classes to configure mappings
        }

        protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            return Configuration.CreateMap<TSource, TDestination>();
        }
    }
}