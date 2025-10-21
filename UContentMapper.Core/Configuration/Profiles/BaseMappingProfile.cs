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

			_configureStringConversions();
			_configureNumericConversions();
			_configureDateTimeConversions();
			_configureBooleanConversions();
			_configureCollectionConversions();
			_configureNullableConversions();
		}

		/// <summary>
		/// Configures mappings for converting strings to various primitive types and <see cref="Guid"/>.
		/// </summary>
		/// <remarks>This method defines conversion rules for mapping strings to types such as <see
		/// cref="int"/>,  <see cref="long"/>, <see cref="decimal"/>, <see cref="double"/>, <see cref="float"/>, and 
		/// <see cref="Guid"/>. If the input string is null or empty, a default value is returned  (e.g., 0 for numeric
		/// types and <see cref="Guid.Empty"/> for <see cref="Guid"/>).  Invalid string formats are also handled by
		/// returning the default value for the target type.</remarks>
		private void _configureStringConversions()
		{
			// String to primitive types
			CreateMap<string, int>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0;

				if (int.TryParse(s, out var result))
					return result;

				return 0;
			});

			CreateMap<string, long>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0L;

				if (long.TryParse(s, out var result))
					return result;

				return 0L;
			});

			CreateMap<string, decimal>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0m;

				if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0m;
			});

			CreateMap<string, double>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0.0;

				if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0.0;
			});

			CreateMap<string, float>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0f;

				if (float.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0f;
			});

			CreateMap<string, Guid>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return Guid.Empty;

				if (Guid.TryParse(s, out var result))
					return result;

				return Guid.Empty;
			});
		}

		/// <summary>
		/// Configures mappings for converting between numeric types and from numeric types to string.
		/// </summary>
		/// <remarks>
		/// This method defines conversion rules for mapping between numeric types such as 
		/// <see cref="int"/>, <see cref="long"/>, <see cref="decimal"/>, and <see cref="double"/>,
		/// as well as conversions from these types to <see cref="string"/>.
		/// </remarks>
		private void _configureNumericConversions()
		{
			// Numeric to string
			CreateMap<int, string>().ConvertUsing(i =>
			{
				return i.ToString();
			});

			CreateMap<long, string>().ConvertUsing(l =>
			{
				return l.ToString();
			});

			CreateMap<decimal, string>().ConvertUsing(d =>
			{
				return d.ToString(CultureInfo.InvariantCulture);
			});

			CreateMap<double, string>().ConvertUsing(d =>
			{
				return d.ToString(CultureInfo.InvariantCulture);
			});

			CreateMap<float, string>().ConvertUsing(f =>
			{
				return f.ToString(CultureInfo.InvariantCulture);
			});

			// Numeric conversions
			CreateMap<int, long>().ConvertUsing(i =>
			{
				return (long)i;
			});

			CreateMap<int, decimal>().ConvertUsing(i =>
			{
				return (decimal)i;
			});

			CreateMap<int, double>().ConvertUsing(i =>
			{
				return (double)i;
			});

			CreateMap<long, decimal>().ConvertUsing(l =>
			{
				return (decimal)l;
			});

			CreateMap<decimal, double>().ConvertUsing(d =>
			{
				return (double)d;
			});
		}

		/// <summary>
		/// Configures mappings for converting between date and time types.
		/// </summary>
		/// <remarks>
		/// This method defines conversion rules for mapping between date/time types such as 
		/// <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, <see cref="DateOnly"/>, and <see cref="TimeOnly"/>,
		/// as well as conversions between these types and strings.
		/// </remarks>
		private void _configureDateTimeConversions()
		{
			// String to DateTime
			CreateMap<string, DateTime>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateTime.MinValue;

				if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateTime.MinValue;
			});

			CreateMap<string, DateTimeOffset>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateTimeOffset.MinValue;

				if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateTimeOffset.MinValue;
			});

			CreateMap<string, DateOnly>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateOnly.MinValue;

				if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateOnly.MinValue;
			});

			CreateMap<string, TimeOnly>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return TimeOnly.MinValue;

				if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return TimeOnly.MinValue;
			});

			// DateTime to string
			CreateMap<DateTime, string>().ConvertUsing(dt =>
			{
				return dt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
			});

			CreateMap<DateTimeOffset, string>().ConvertUsing(dto =>
			{
				return dto.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
			});

			CreateMap<DateOnly, string>().ConvertUsing(d =>
			{
				return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			});

			CreateMap<TimeOnly, string>().ConvertUsing(t =>
			{
				return t.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
			});

			// DateTime conversions
			CreateMap<DateTime, DateTimeOffset>().ConvertUsing(dt =>
			{
				return new DateTimeOffset(dt);
			});

			CreateMap<DateTimeOffset, DateTime>().ConvertUsing(dto =>
			{
				return dto.DateTime;
			});

			CreateMap<DateTime, DateOnly>().ConvertUsing(dt =>
			{
				return DateOnly.FromDateTime(dt);
			});

			CreateMap<DateTime, TimeOnly>().ConvertUsing(dt =>
			{
				return TimeOnly.FromDateTime(dt);
			});
		}

		/// <summary>
		/// Configures mappings for converting between boolean and other types.
		/// </summary>
		/// <remarks>
		/// This method defines conversion rules for mapping between <see cref="bool"/> and other types
		/// such as <see cref="string"/> and numeric types. It handles various string formats that can
		/// represent boolean values (e.g., "true", "yes", "1", etc.).
		/// </remarks>
		private void _configureBooleanConversions()
		{
			// String to bool
			CreateMap<string, bool>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return false;

				if (bool.TryParse(s, out var boolResult))
					return boolResult;

				return s.Equals("1", StringComparison.OrdinalIgnoreCase) ||
					   s.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
					   s.Equals("true", StringComparison.OrdinalIgnoreCase) ||
					   s.Equals("on", StringComparison.OrdinalIgnoreCase);
			});

			// Numeric to bool
			CreateMap<int, bool>().ConvertUsing(i =>
			{
				return i != 0;
			});

			CreateMap<long, bool>().ConvertUsing(l =>
			{
				return l != 0;
			});

			CreateMap<decimal, bool>().ConvertUsing(d =>
			{
				return d != 0;
			});

			// Bool to string/numeric
			CreateMap<bool, string>().ConvertUsing(b =>
			{
				return b.ToString().ToLowerInvariant();
			});

			CreateMap<bool, int>().ConvertUsing(b =>
			{
				return b ? 1 : 0;
			});
		}

		/// <summary>
		/// Configures mappings for converting between collection types.
		/// </summary>
		/// <remarks>
		/// This method defines conversion rules for mapping between different collection types
		/// such as arrays, lists, and enumerables, as well as conversions between collections
		/// and strings (using comma as a delimiter).
		/// </remarks>
		private void _configureCollectionConversions()
		{
			// String collections
			CreateMap<string, string[]>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return Array.Empty<string>();

				return s.Split(',', StringSplitOptions.RemoveEmptyEntries);
			});

			CreateMap<string, List<string>>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return new List<string>();

				return s.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
			});

			CreateMap<string[], string>().ConvertUsing(arr =>
			{
				if (arr == null)
					return string.Empty;

				return string.Join(",", arr);
			});

			CreateMap<List<string>, string>().ConvertUsing(list =>
			{
				if (list == null)
					return string.Empty;

				return string.Join(",", list);
			});

			CreateMap<IEnumerable<string>, string>().ConvertUsing(enumerable =>
			{
				if (enumerable == null)
					return string.Empty;

				return string.Join(",", enumerable);
			});

			// Generic collection conversions
			CreateMap<List<int>, int[]>().ConvertUsing(list =>
			{
				if (list == null)
					return Array.Empty<int>();

				return list.ToArray();
			});

			CreateMap<int[], List<int>>().ConvertUsing(arr =>
			{
				if (arr == null)
					return new List<int>();

				return arr.ToList();
			});
		}

		/// <summary>
		/// Configures mappings for converting between nullable and non-nullable types.
		/// </summary>
		/// <remarks>
		/// This method defines conversion rules for mapping between nullable value types and their non-nullable
		/// counterparts, as well as conversions between nullable types and strings. Default values are provided
		/// when converting from nullable to non-nullable types.
		/// </remarks>
		private void _configureNullableConversions()
		{
			// String to nullable types
			CreateMap<string, int?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (int.TryParse(s, out var result))
					return result;

				return null;
			});

			CreateMap<string, decimal?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return null;
			});

			CreateMap<string, DateTime?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return null;
			});

			CreateMap<string, bool?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (bool.TryParse(s, out var result))
					return result;

				return null;
			});

			CreateMap<string, Guid?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (Guid.TryParse(s, out var result))
					return result;

				return null;
			});

			// Nullable to string
			CreateMap<int?, string>().ConvertUsing(i =>
			{
				return i?.ToString() ?? string.Empty;
			});

			CreateMap<decimal?, string>().ConvertUsing(d =>
			{
				return d?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
			});

			CreateMap<DateTime?, string>().ConvertUsing(dt =>
			{
				return dt?.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;
			});

			CreateMap<bool?, string>().ConvertUsing(b =>
			{
				return b?.ToString().ToLowerInvariant() ?? string.Empty;
			});

			CreateMap<Guid?, string>().ConvertUsing(g =>
			{
				return g?.ToString() ?? string.Empty;
			});

			// Value to nullable
			CreateMap<int, int?>().ConvertUsing(i =>
			{
				return i;
			});

			CreateMap<decimal, decimal?>().ConvertUsing(d =>
			{
				return d;
			});

			CreateMap<DateTime, DateTime?>().ConvertUsing(dt =>
			{
				return dt;
			});

			CreateMap<bool, bool?>().ConvertUsing(b =>
			{
				return b;
			});

			CreateMap<Guid, Guid?>().ConvertUsing(g =>
			{
				return g;
			});

			// Nullable to value (with defaults)
			CreateMap<int?, int>().ConvertUsing(i =>
			{
				return i ?? 0;
			});

			CreateMap<decimal?, decimal>().ConvertUsing(d =>
			{
				return d ?? 0m;
			});

			CreateMap<DateTime?, DateTime>().ConvertUsing(dt =>
			{
				return dt ?? DateTime.MinValue;
			});

			CreateMap<bool?, bool>().ConvertUsing(b =>
			{
				return b ?? false;
			});

			CreateMap<Guid?, Guid>().ConvertUsing(g =>
			{
				return g ?? Guid.Empty;
			});
		}
	}
}