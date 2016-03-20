using Enigma.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Rotors
{
    public class RotorList : List<Rotor>
    {
        public RotorList(MachineType m)
        {
            foreach (RotorName r in Enum.GetValues(typeof(RotorName)))
            {
                Add(new Rotor(r, Constants.Notches[(int)r]));
            }

            if (m == MachineType.M3)
            {
                RemoveAll(
                    r =>
                        r.RotorName == RotorName.Beta || r.RotorName == RotorName.Gamma ||
                        r.RotorName == RotorName.VI || r.RotorName == RotorName.VII || r.RotorName == RotorName.VIII);
            }
            else
            {
                if (m == MachineType.M3K)
                {
                    RemoveAll(
                        r =>
                            r.RotorName == RotorName.Beta || r.RotorName == RotorName.Gamma);
                }
            }
        }
        public Rotor this[RotorName rotName]
        {
            get { return this.FirstOrDefault(r => r.RotorName == rotName); }
        }
    }
}
