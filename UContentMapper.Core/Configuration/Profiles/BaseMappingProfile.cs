using System.Globalization;

namespace UContentMapper.Core.Configuration.Profiles
{
    /// <summary>
    /// Provides common mappings that most applications will need
    /// </summary>
    public class BaseMappingProfile : MappingProfile
    {
        private bool _initialized = false;

        public BaseMappingProfile()
        {
            // Don't configure mappings in constructor
        }

        public override void Configure()
        {
            if (_initialized)
                return;

            _initialized = true;

            ConfigureStringConversions();
            ConfigureNumericConversions();
            ConfigureDateTimeConversions();
            ConfigureBooleanConversions();
            ConfigureCollectionConversions();
            ConfigureNullableConversions();
        }

        /// <summary>
        /// String conversion mappings
        /// </summary>
        private void ConfigureStringConversions()
        {
            // String to primitive types
            CreateMap<string, int>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? 0 : int.TryParse(s, out var result) ? result : 0);

            CreateMap<string, long>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? 0L : long.TryParse(s, out var result) ? result : 0L);

            CreateMap<string, decimal>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? 0m :
                    decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : 0m);

            CreateMap<string, double>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? 0.0 :
                    double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : 0.0);

            CreateMap<string, float>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? 0f :
                    float.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : 0f);

            CreateMap<string, Guid>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? Guid.Empty :
                    Guid.TryParse(s, out var result) ? result : Guid.Empty);
        }

        /// <summary>
        /// Numeric conversion mappings
        /// </summary>
        private void ConfigureNumericConversions()
        {
            // Numeric to string
            CreateMap<int, string>().ConvertUsing(i => i.ToString());
            CreateMap<long, string>().ConvertUsing(l => l.ToString());
            CreateMap<decimal, string>().ConvertUsing(d => d.ToString(CultureInfo.InvariantCulture));
            CreateMap<double, string>().ConvertUsing(d => d.ToString(CultureInfo.InvariantCulture));
            CreateMap<float, string>().ConvertUsing(f => f.ToString(CultureInfo.InvariantCulture));

            // Numeric conversions
            CreateMap<int, long>().ConvertUsing(i => (long)i);
            CreateMap<int, decimal>().ConvertUsing(i => (decimal)i);
            CreateMap<int, double>().ConvertUsing(i => (double)i);
            CreateMap<long, decimal>().ConvertUsing(l => (decimal)l);
            CreateMap<decimal, double>().ConvertUsing(d => (double)d);
        }

        /// <summary>
        /// DateTime conversion mappings
        /// </summary>
        private void ConfigureDateTimeConversions()
        {
            // String to DateTime
            CreateMap<string, DateTime>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? DateTime.MinValue :
                    DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : DateTime.MinValue);

            CreateMap<string, DateTimeOffset>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? DateTimeOffset.MinValue :
                    DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : DateTimeOffset.MinValue);

            CreateMap<string, DateOnly>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? DateOnly.MinValue :
                    DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : DateOnly.MinValue);

            CreateMap<string, TimeOnly>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? TimeOnly.MinValue :
                    TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : TimeOnly.MinValue);

            // DateTime to string
            CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture));
            CreateMap<DateTimeOffset, string>().ConvertUsing(dto => dto.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture));
            CreateMap<DateOnly, string>().ConvertUsing(d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            CreateMap<TimeOnly, string>().ConvertUsing(t => t.ToString("HH:mm:ss", CultureInfo.InvariantCulture));

            // DateTime conversions
            CreateMap<DateTime, DateTimeOffset>().ConvertUsing(dt => new DateTimeOffset(dt));
            CreateMap<DateTimeOffset, DateTime>().ConvertUsing(dto => dto.DateTime);
            CreateMap<DateTime, DateOnly>().ConvertUsing(dt => DateOnly.FromDateTime(dt));
            CreateMap<DateTime, TimeOnly>().ConvertUsing(dt => TimeOnly.FromDateTime(dt));
        }

        /// <summary>
        /// Boolean conversion mappings
        /// </summary>
        private void ConfigureBooleanConversions()
        {
            // String to bool
            CreateMap<string, bool>()
                .ConvertUsing(s => !string.IsNullOrEmpty(s) &&
                    (bool.TryParse(s, out var boolResult) ? boolResult :
                     (s.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                      s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                      s.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                      s.Equals("on", StringComparison.OrdinalIgnoreCase))));

            // Numeric to bool
            CreateMap<int, bool>().ConvertUsing(i => i != 0);
            CreateMap<long, bool>().ConvertUsing(l => l != 0);
            CreateMap<decimal, bool>().ConvertUsing(d => d != 0);

            // Bool to string/numeric
            CreateMap<bool, string>().ConvertUsing(b => b.ToString().ToLowerInvariant());
            CreateMap<bool, int>().ConvertUsing(b => b ? 1 : 0);
        }

        /// <summary>
        /// Collection conversion mappings
        /// </summary>
        private void ConfigureCollectionConversions()
        {
            // String collections
            CreateMap<string, string[]>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? Array.Empty<string>() : s.Split(',', StringSplitOptions.RemoveEmptyEntries));

            CreateMap<string, List<string>>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? new List<string>() :
                    s.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            CreateMap<string[], string>()
                .ConvertUsing(arr => arr == null ? string.Empty : string.Join(",", arr));

            CreateMap<List<string>, string>()
                .ConvertUsing(list => list == null ? string.Empty : string.Join(",", list));

            CreateMap<IEnumerable<string>, string>()
                .ConvertUsing(enumerable => enumerable == null ? string.Empty : string.Join(",", enumerable));

            // Generic collection conversions
            CreateMap<List<int>, int[]>().ConvertUsing(list => list?.ToArray() ?? Array.Empty<int>());
            CreateMap<int[], List<int>>().ConvertUsing(arr => arr?.ToList() ?? new List<int>());
        }

        /// <summary>
        /// Nullable type conversion mappings
        /// </summary>
        private void ConfigureNullableConversions()
        {
            // String to nullable types
            CreateMap<string, int?>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null : int.TryParse(s, out var result) ? result : null);

            CreateMap<string, decimal?>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null :
                    decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : null);

            CreateMap<string, DateTime?>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null :
                    DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : null);

            CreateMap<string, bool?>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null : bool.TryParse(s, out var result) ? result : null);

            CreateMap<string, Guid?>()
                .ConvertUsing(s => string.IsNullOrEmpty(s) ? null : Guid.TryParse(s, out var result) ? result : null);

            // Nullable to string
            CreateMap<int?, string>().ConvertUsing(i => i?.ToString() ?? string.Empty);
            CreateMap<decimal?, string>().ConvertUsing(d => d?.ToString(CultureInfo.InvariantCulture) ?? string.Empty);
            CreateMap<DateTime?, string>().ConvertUsing(dt => dt?.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty);
            CreateMap<bool?, string>().ConvertUsing(b => b?.ToString().ToLowerInvariant() ?? string.Empty);
            CreateMap<Guid?, string>().ConvertUsing(g => g?.ToString() ?? string.Empty);

            // Value to nullable
            CreateMap<int, int?>().ConvertUsing(i => i);
            CreateMap<decimal, decimal?>().ConvertUsing(d => d);
            CreateMap<DateTime, DateTime?>().ConvertUsing(dt => dt);
            CreateMap<bool, bool?>().ConvertUsing(b => b);
            CreateMap<Guid, Guid?>().ConvertUsing(g => g);

            // Nullable to value (with defaults)
            CreateMap<int?, int>().ConvertUsing(i => i ?? 0);
            CreateMap<decimal?, decimal>().ConvertUsing(d => d ?? 0m);
            CreateMap<DateTime?, DateTime>().ConvertUsing(dt => dt ?? DateTime.MinValue);
            CreateMap<bool?, bool>().ConvertUsing(b => b ?? false);
            CreateMap<Guid?, Guid>().ConvertUsing(g => g ?? Guid.Empty);
        }
    }
}