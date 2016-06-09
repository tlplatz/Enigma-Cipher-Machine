using Enigma.Enums;

namespace Enigma
{
    internal class Reflector : FixedRotor
    {
        public Reflector(ReflectorType r)
            : base(r.ToString(), Constants.Reflectors[(int)r])
        {
            ReflectorType = r;

        }
        public ReflectorType ReflectorType { get; private set; }
    }
}
