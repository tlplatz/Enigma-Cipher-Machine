using Enigma.Enums;

namespace Enigma.Configuration
{
    public class BrokenRule
    {
        internal BrokenRule()
        {

        }

        public string Message { get; set; }
        public ValidationFailureType FailureType { get; set; }
    }
}
