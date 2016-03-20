using Enigma.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.UKW
{
    internal class ReflectorList : List<Reflector>
    {
        public ReflectorList(MachineType m)
        {
            foreach (ReflectorType item in Enum.GetValues(typeof(ReflectorType)))
            {
                Add(new Reflector(item));
            }

            if (m == MachineType.M3 || m == MachineType.M3K)
            {
                RemoveAll(r => r.ReflectorType == ReflectorType.B_Dunn || r.ReflectorType == ReflectorType.C_Dunn);
            }
            else
            {
                RemoveAll(r => r.ReflectorType == ReflectorType.B || r.ReflectorType == ReflectorType.C);
            }
        }
        public Reflector this[ReflectorType refTyp]
        {
            get { return this.FirstOrDefault(r => r.ReflectorType == refTyp); }
        }
    }
}
