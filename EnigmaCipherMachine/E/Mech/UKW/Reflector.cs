using Enigma.Base;
using Enigma.Enums;

namespace Enigma.UKW
{
    public class Reflector : FixedRotor
    {
        public Reflector(ReflectorType r)
            : base(r.ToString(), Constants.Reflectors[(int)r])
        {
            ReflectorType = r;

        }
        public ReflectorType ReflectorType { get; private set; }
    }
}
