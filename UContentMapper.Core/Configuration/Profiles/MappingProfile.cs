using UContentMapper.Core.Abstractions.Configuration;

namespace UContentMapper.Core.Configuration.Profiles
{
    public abstract class MappingProfile
    {
        protected IMappingConfiguration Configuration { get; private set; }

        public void Initialize(IMappingConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            return Configuration.CreateMap<TSource, TDestination>();
        }
    }
}