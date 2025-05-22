namespace UContentMapper.Core.Exceptions
{
    public class ConfigurationException : MappingException
    {
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