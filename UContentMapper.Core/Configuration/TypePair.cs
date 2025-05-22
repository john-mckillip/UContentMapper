namespace UContentMapper.Core.Configuration
{
    /// <summary>
    /// Represents a pair of source and destination types for mapping
    /// </summary>
    /// <remarks>
    /// Initializes a new TypePair
    /// </remarks>
    public readonly struct TypePair(Type sourceType, Type destinationType) : IEquatable<TypePair>
    {

        /// <summary>
        /// The source type in the mapping
        /// </summary>
        public Type SourceType { get; } = sourceType ?? throw new ArgumentNullException(nameof(sourceType));

        /// <summary>
        /// The destination type in the mapping
        /// </summary>
        public Type DestinationType { get; } = destinationType ?? throw new ArgumentNullException(nameof(destinationType));

        /// <summary>
        /// Determines if two TypePair instances are equal
        /// </summary>
        public bool Equals(TypePair other)
        {
            return SourceType == other.SourceType &&
                   DestinationType == other.DestinationType;
        }

        /// <summary>
        /// Determines if this TypePair equals another object
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TypePair other && Equals(other);
        }

        /// <summary>
        /// Gets the hash code for this TypePair
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(SourceType, DestinationType);
        }

        /// <summary>
        /// String representation of the TypePair
        /// </summary>
        public override string ToString()
        {
            return $"{SourceType.Name} -> {DestinationType.Name}";
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(TypePair left, TypePair right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(TypePair left, TypePair right)
        {
            return !left.Equals(right);
        }
    }
}