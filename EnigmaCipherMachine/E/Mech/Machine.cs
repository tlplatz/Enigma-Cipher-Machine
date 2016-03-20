using Enigma.Configuration;
using Enigma.Enums;
using Enigma.PlugBoard;
using Enigma.Rotors;
using Enigma.UKW;
using System.Collections.Generic;
using System.Linq;

namespace Enigma
{
    internal class Machine
    {
        private List<Rotor> _rotors;
        private Reflector _reflector;
        private Stecker _stecker;

        public Machine(MachineType type)
        {
            MachineType = type;

            AvailableReflectors = new ReflectorList(MachineType);
            AvailableRotors = new RotorList(MachineType);

            _stecker = new Stecker();
            _rotors = new List<Rotor>();

            if (type == MachineType.M3 || type == MachineType.M3K)
            {
                _reflector = new Reflector(ReflectorType.B);

                _rotors.Add(AvailableRotors[RotorName.III]);
                _rotors.Add(AvailableRotors[RotorName.II]);
                _rotors.Add(AvailableRotors[RotorName.I]);
            }
            else
            {
                _reflector = new Reflector(ReflectorType.B_Dunn);

                _rotors.Add(AvailableRotors[RotorName.III]);
                _rotors.Add(AvailableRotors[RotorName.II]);
                _rotors.Add(AvailableRotors[RotorName.I]);
                _rotors.Add(AvailableRotors[RotorName.Beta]);
            }
        }
        public Machine(Settings s)
            : this(s.MachineType)
        {
            _reflector = AvailableReflectors[s.ReflectorType];
            Rotors = s.Rotors.Select(r => r.Name).ToList();
            RingSettings = s.Rotors.Select(r => r.RingSetting).ToList();
            SteckerSettings = string.Join(" ", s.Plugs.Select(p => p.ToString()));
        }

        public MachineType MachineType { get; private set; }
        public ReflectorList AvailableReflectors { get; private set; }
        public RotorList AvailableRotors { get; private set; }

        public List<RotorName> Rotors
        {
            get { return _rotors.Select(r => r.RotorName).Reverse().ToList(); }
            set
            {
                _rotors.Clear();

                foreach (RotorName name in value.Select(r => r).Reverse())
                {
                    Rotor r = AvailableRotors[name];
                    if (r != null)
                    {
                        _rotors.Add(r);
                    }
                }
            }
        }
        public ReflectorType Reflector
        {
            get { return _reflector.ReflectorType; }
            set
            {
                _reflector = AvailableReflectors[value];
            }
        }
        public List<int> RingSettings
        {
            get { return _rotors.Select(r => r.RingSetting).Reverse().ToList(); }
            set
            {
                int index = 0;
                foreach (int i in value.Select(i => i).Reverse())
                {
                    _rotors[index].RingSetting = i;
                    index++;
                }
            }
        }
        public string RotorSettings
        {
            get { return string.Concat(_rotors.Select(r => r.Setting).Reverse()); }
            set
            {
                int index = 0;
                foreach (char c in value.Select(c => c).Reverse())
                {
                    _rotors[index].Setting = c.ToString();
                    index++;
                }
            }
        }
        public string SteckerSettings
        {
            get { return _stecker.PlugSettings; }
            set { _stecker.PlugSettings = value; }
        }

        public string Encipher(string input)
        {
            AdvanceRotors();

            int wire = StringToWire(input);

            wire = _stecker.WireLeft(wire);

            for (int i = 0; i < _rotors.Count; i++)
            {
                wire = _rotors[i].WireLeft(wire);
            }

            wire = _reflector.WireRight(wire);

            for (int i = _rotors.Count - 1; i >= 0; i--)
            {
                wire = _rotors[i].WireRight(wire);
            }

            wire = _stecker.WireRight(wire);

            return WireToString(wire);
        }

        public Settings Settings
        {
            get
            {
                Settings result = new Settings();

                result.Day = 1;
                result.MachineType = MachineType;
                result.ReflectorType = Reflector;
                result.Rotors = _rotors.Select(r => new RotorSetting { Name = r.RotorName, RingSetting = r.RingSetting }).Reverse().ToList();
                result.Plugs = _stecker.Plugs.Select(p => new PlugSetting { LetterA = p.LetterA, LetterB = p.LetterB }).ToList();

                return result;
            }
            set
            {
                MachineType = value.MachineType;

                AvailableReflectors = new ReflectorList(value.MachineType);
                AvailableRotors = new RotorList(value.MachineType);

                _stecker.Plugs.Clear();
                foreach (var item in value.Plugs)
                {
                    _stecker.Plugs.Add(new Plug(item.LetterA, item.LetterB));
                }

                _reflector = AvailableReflectors[value.ReflectorType];
                _rotors.Clear();

                for (int i = value.Rotors.Count - 1; i >= 0; i--)
                {
                    _rotors.Add(AvailableRotors[value.Rotors[i].Name]);
                }
                for (int i = value.Rotors.Count - 1; i >= 0; i--)
                {
                    _rotors[i].RingSetting = value.Rotors[i].RingSetting;
                }
            }
        }

        private void AdvanceRotors()
        {
            bool r1 = _rotors[0].OnNotch;
            bool r2 = _rotors[1].OnNotch;

            _rotors[0].Advance();

            if (r1 || r2)
            {
                _rotors[1].Advance();
            }

            if (r2)
            {
                _rotors[2].Advance();
            }
        }
        private int StringToWire(string s)
        {
            return Constants.ALPHABET.IndexOf(s);
        }
        private string WireToString(int wire)
        {
            return Constants.ALPHABET[wire].ToString();
        }
    }
}
