using System.Text.Json.Serialization;

namespace UContentMapper.Core.Exceptions
{
    public class MappingException : Exception
    {
        [JsonConstructor]
        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}