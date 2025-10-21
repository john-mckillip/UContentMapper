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

			// String to numeric conversions
			_configureStringToIntConversion();
			_configureStringToLongConversion();
			_configureStringToDecimalConversion();
			_configureStringToDoubleConversion();
			_configureStringToFloatConversion();
			_configureStringToGuidConversion();

			// Numeric to string conversions
			_configureNumericToStringConversions();

			// String to DateTime conversions
			_configureStringToDateTimeConversion();
			_configureStringToDateTimeOffsetConversion();
			_configureStringToDateOnlyConversion();
			_configureStringToTimeOnlyConversion();

			// DateTime to string conversions
			_configureDateTimeToStringConversions();

			// Boolean conversions
			_configureBooleanConversions();

			// Collection conversions
			_configureCollectionConversions();

			// Nullable conversions
			_configureStringToNullableInt();
			_stringToNullableDecimal();
			_stringToNullableDateTime();
			_stringToNullableBoolean();
			_stringToNullableGuid();
			_configureNullableToStringConversions();
		}

		/// <summary>
		/// Configures a mapping from strings to integers, with custom conversion logic.
		/// </summary>
		/// <remarks>The conversion logic treats null or empty strings as 0. If the string represents a valid integer,
		/// it is parsed and returned. Otherwise, non-numeric strings are also converted to 0.</remarks>
		private void _configureStringToIntConversion()
		{
			CreateMap<string, int>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0;

				if (int.TryParse(s, out var result))
					return result;

				return 0;
			});
		}

		/// <summary>
		/// Configures a mapping from strings to long integers, with custom conversion logic.
		/// </summary>
		/// <remarks>The conversion logic ensures that null or empty strings are mapped to <see langword="0"/>,  and
		/// valid numeric strings are parsed into their corresponding <see cref="long"/> values.  If the string cannot be
		/// parsed as a valid number, it defaults to <see langword="0"/>.</remarks>
		private void _configureStringToLongConversion()
		{
			CreateMap<string, long>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0L;

				if (long.TryParse(s, out var result))
					return result;

				return 0L;
			});
		}

		/// <summary>
		/// Configures a mapping from a string to a decimal value, with custom conversion logic.
		/// </summary>
		/// <remarks>The conversion logic interprets null or empty strings as <see langword="0m"/>.  If the string
		/// cannot be parsed as a decimal using invariant culture, it also defaults to <see langword="0m"/>.</remarks>
		private void _configureStringToDecimalConversion()
		{
			CreateMap<string, decimal>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0m;

				if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0m;
			});
		}

		/// <summary>
		/// Configures a mapping from strings to doubles, with custom conversion logic.
		/// </summary>
		/// <remarks>The conversion logic interprets null or empty strings as 0.0. If the string cannot be parsed  as
		/// a double using invariant culture, the value 0.0 is returned.</remarks>
		private void _configureStringToDoubleConversion()
		{
			CreateMap<string, double>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0.0;

				if (double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0.0;
			});
		}

		/// <summary>
		/// Configures a mapping from a string to a float, with custom conversion logic.
		/// </summary>
		/// <remarks>The conversion logic ensures that null or empty strings are mapped to 0. If the string represents
		/// a valid float in invariant culture, it is parsed and returned. Otherwise, the value defaults to 0.</remarks>
		private void _configureStringToFloatConversion()
		{
			CreateMap<string, float>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return 0f;

				if (float.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return 0f;
			});
		}

		/// <summary>
		/// Configures a mapping from strings to GUIDs, converting string values to their corresponding <see cref="Guid"/>
		/// representations.
		/// </summary>
		/// <remarks>This method sets up a conversion rule where valid GUID strings are parsed into <see cref="Guid"/>
		/// values.  If the input string is <see langword="null"/> or empty, or if it cannot be parsed as a valid GUID, the
		/// conversion returns <see cref="Guid.Empty"/>.</remarks>
		private void _configureStringToGuidConversion()
		{
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
		private void _configureNumericToStringConversions()
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
		/// Configures a mapping to convert strings to <see cref="DateTime"/> values.
		/// </summary>
		/// <remarks>This mapping converts a string to a <see cref="DateTime"/> using the invariant culture. If the
		/// input string is <see langword="null"/> or empty, or if the string cannot be parsed as a valid <see
		/// cref="DateTime"/>, the mapping returns <see cref="DateTime.MinValue"/>.</remarks>
		private void _configureStringToDateTimeConversion()
		{
			CreateMap<string, DateTime>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateTime.MinValue;

				if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateTime.MinValue;
			});
		}

		/// <summary>
		/// Configures a mapping from a <see cref="string"/> to a <see cref="DateTimeOffset"/>  with specific conversion
		/// rules.
		/// </summary>
		/// <remarks>The conversion returns <see cref="DateTimeOffset.MinValue"/> if the input string is null, empty, 
		/// or cannot be parsed as a valid <see cref="DateTimeOffset"/>. Parsing is performed using  <see
		/// cref="CultureInfo.InvariantCulture"/> and <see cref="DateTimeStyles.None"/>.</remarks>
		private void _configureStringToDateTimeOffsetConversion()
		{
			CreateMap<string, DateTimeOffset>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateTimeOffset.MinValue;

				if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateTimeOffset.MinValue;
			});
		}

		/// <summary>
		/// Configures a mapping from a <see cref="string"/> to a <see cref="DateOnly"/> value.
		/// </summary>
		/// <remarks>This mapping converts a string representation of a date to a <see cref="DateOnly"/> instance. If
		/// the input string is <see langword="null"/> or empty, or if parsing fails, the mapping returns <see
		/// cref="DateOnly.MinValue"/>. The conversion uses <see cref="CultureInfo.InvariantCulture"/> and does not allow
		/// additional date styles.</remarks>
		private void _configureStringToDateOnlyConversion()
		{
			CreateMap<string, DateOnly>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return DateOnly.MinValue;

				if (DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return DateOnly.MinValue;
			});
		}

		/// <summary>
		/// Configures a mapping from a <see cref="string"/> to a <see cref="TimeOnly"/> value.
		/// </summary>
		/// <remarks>This mapping interprets an empty or null string as <see cref="TimeOnly.MinValue"/>.  If the
		/// string cannot be parsed as a valid <see cref="TimeOnly"/> using the invariant culture,  <see
		/// cref="TimeOnly.MinValue"/> is also returned.</remarks>
		private void _configureStringToTimeOnlyConversion()
		{
			CreateMap<string, TimeOnly>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return TimeOnly.MinValue;

				if (TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return TimeOnly.MinValue;
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
		private void _configureDateTimeToStringConversions()
		{
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
		/// Configures a mapping from a <see cref="string"/> to a nullable <see cref="int"/>.
		/// </summary>
		/// <remarks>This mapping converts a string to a nullable integer. If the input string is null or empty,  the
		/// result is <see langword="null"/>. If the string represents a valid integer, the parsed  integer value is returned.
		/// Otherwise, the result is <see langword="null"/>.</remarks>
		private void _configureStringToNullableInt()
		{
			CreateMap<string, int?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (int.TryParse(s, out var result))
					return result;

				return null;
			});
		}

		/// <summary>
		/// Configures a mapping from a <see cref="string"/> to a nullable <see cref="decimal"/>.
		/// </summary>
		/// <remarks>This mapping converts a string to a nullable decimal using invariant culture.  If the input
		/// string is <see langword="null"/> or empty, the result is <see langword="null"/>.  If the string cannot be parsed
		/// as a valid decimal, the result is also <see langword="null"/>.</remarks>
		private void _stringToNullableDecimal()
		{
			CreateMap<string, decimal?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
					return result;

				return null;
			});
		}

		/// <summary>
		/// Configures a mapping from a <see cref="string"/> to a nullable <see cref="DateTime"/>.
		/// </summary>
		/// <remarks>This mapping converts a string to a <see cref="DateTime?"/> using the invariant culture. If the
		/// input string is null or empty, the result is <see langword="null"/>. If the string cannot be parsed as a valid
		/// <see cref="DateTime"/>, the result is also <see langword="null"/>.</remarks>
		private void _stringToNullableDateTime()
		{
			CreateMap<string, DateTime?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
					return result;

				return null;
			});
		}

		/// <summary>
		/// Configures a mapping from a string to a nullable boolean value.
		/// </summary>
		/// <remarks>This mapping interprets the input string as follows: <list type="bullet"> <item><description>If
		/// the string is <see langword="null"/> or empty, the result is <see langword="null"/>.</description></item>
		/// <item><description>If the string can be parsed as a boolean (e.g., "true" or "false"), the parsed value is
		/// returned.</description></item> <item><description>For all other strings, the result is <see
		/// langword="null"/>.</description></item> </list> This method is typically used to define custom conversion logic in
		/// mapping configurations.</remarks>
		private void _stringToNullableBoolean()
		{
			CreateMap<string, bool?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (bool.TryParse(s, out var result))
					return result;

				return null;
			});
		}

		/// <summary>
		/// Configures a mapping from a string to a nullable <see cref="Guid"/>.
		/// </summary>
		/// <remarks>This mapping converts a string to a <see cref="Guid?"/>. If the input string is null or empty, 
		/// the result is <see langword="null"/>. If the string is a valid <see cref="Guid"/> representation,  it is parsed
		/// and returned. Otherwise, the result is <see langword="null"/>.</remarks>
		private void _stringToNullableGuid()
		{
			CreateMap<string, Guid?>().ConvertUsing(s =>
			{
				if (string.IsNullOrEmpty(s))
					return null;

				if (Guid.TryParse(s, out var result))
					return result;

				return null;
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
		private void _configureNullableToStringConversions()
		{
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