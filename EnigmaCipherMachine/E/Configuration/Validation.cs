using System.Collections.Generic;
using System.Linq;
using WizardNet.Enigma.Enums;
using WizardNet.Enigma.Rotors;
using WizardNet.Enigma.UKW;

namespace WizardNet.Enigma.Configuration
{
    public static class Validation
    {
        public static bool Validate(Settings s, out List<BrokenRule> rules)
        {
            rules = new List<BrokenRule>();

            RotorList availableRotors = new RotorList(s.MachineType);
            ReflectorList availableReflectors = new ReflectorList(s.MachineType);

            if (!availableReflectors.Any(r => r.ReflectorType == s.ReflectorType))
            {
                rules.Add(new BrokenRule { FailureType = ValidationFailureType.InvalidReflectorTypeForMachine, Message = string.Format("Reflector {0} not valid with machine type {1}", s.ReflectorType, s.MachineType) });
            }

            if (s.MachineType == MachineType.M4K)
            {
                if (s.Rotors.Count != 4)
                {
                    rules.Add(new BrokenRule { FailureType = ValidationFailureType.InvalidRotorCount, Message = string.Format("4 rotors are required for machine type {0}", s.MachineType) });
                }
            }
            else
            {
                if (s.Rotors.Count != 3)
                {
                    rules.Add(new BrokenRule { FailureType = ValidationFailureType.InvalidRotorCount, Message = string.Format("3 rotors are required for machine type {0}", s.MachineType) });
                }
            }

            foreach (var rotor in s.Rotors)
            {
                if (!availableRotors.Any(r => r.RotorName == rotor.Name))
                {
                    rules.Add(new BrokenRule { FailureType = ValidationFailureType.InvalidRotorTypeForMachine, Message = string.Format("Rotor {0} not valid with machine type {1}", rotor, s.MachineType) });
                }

                if (rotor.RingSetting < 0 || rotor.RingSetting > 25)
                {
                    rules.Add(new BrokenRule { FailureType = ValidationFailureType.RingSettingOutOfRange, Message = string.Format("Rotor {0} ring setting not between 0 and 25", rotor) });
                }
            }

            if (s.MachineType == MachineType.M4K)
            {
                if (s.Rotors[0].Name != RotorName.Beta && s.Rotors[0].Name != RotorName.Gamma)
                {
                    rules.Add(new BrokenRule { FailureType = ValidationFailureType.ThinRotorMissing, Message = string.Format("A thin Gamma or Beta rotor needs to be next to the reflector for machine type {0}", s.MachineType) });
                }
            }

            if (s.Plugs.Count > 13)
            {
                rules.Add(new BrokenRule { FailureType = ValidationFailureType.TooManyPlugs, Message = "The maximum number of plugs is 13" });
            }

            if (s.Plugs.Any(p => p.LetterA == p.LetterB))
            {
                rules.Add(new BrokenRule { FailureType = ValidationFailureType.PlugsLinksNotUnique, Message = "All plugs must link 2 different letters" });
            }

            string duplicatePlugs = string.Join(" ", s.Plugs.GroupBy(p => p.ToString()).Where(g => g.Count() > 1).Select(g => g.Key)).Trim();
            if (!string.IsNullOrEmpty(duplicatePlugs))
            {
                rules.Add(new BrokenRule { FailureType = ValidationFailureType.DuplicatePlugs, Message = string.Format("Plugs {0} are duplicated", duplicatePlugs) });
            }

            return !rules.Any();

        }
    }
}
