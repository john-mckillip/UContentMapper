using System.Text.Json.Serialization;

namespace UContentMapper.Core.Exceptions
{
    public class ConfigurationException : MappingException
    {
        [JsonConstructor]
        public ConfigurationException(string message) : base(message)
        {
        }

        public ConfigurationException(
            string message,
            IEnumerable<string> validationErrors)
            : base(message)
        {
            ValidationErrors = validationErrors;
        }

        public IEnumerable<string>? ValidationErrors { get; }
    }
}