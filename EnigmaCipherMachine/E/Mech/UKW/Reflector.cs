using WizardNet.Enigma.Base;
using WizardNet.Enigma.Enums;

namespace WizardNet.Enigma.UKW
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
