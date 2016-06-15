using Enigma.Enums;
using Enigma.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class Settings : IEquatable<Settings>
    {
        private static Random _rnd = new Random();

        [XmlAttribute]
        public int Day { get; set; }
        [XmlAttribute]
        public MachineType MachineType { get; set; }
        [XmlAttribute]
        public ReflectorType ReflectorType { get; set; }

        [XmlAttribute]
        public string Grund { get; set; }
        [XmlAttribute]
        public List<string> Kenngruppen { get; set; }

        public List<RotorSetting> Rotors { get; set; }
        public List<PlugSetting> Plugs { get; set; }

        public Settings()
        {
            Rotors = new List<RotorSetting>();
            Plugs = new List<PlugSetting>();

            Kenngruppen = new List<string>();
        }
        public Settings(MachineType typ, ReflectorType refTyp)
            : this()
        {
            MachineType = typ;
            ReflectorType = refTyp;
        }
        public Settings(MachineType typ, ReflectorType refTyp, RotorName[] rotors, int[] ringPos, string[] plugs)
            : this(typ, refTyp)
        {
            MachineType = typ;
            ReflectorType = refTyp;

            if (MachineType == MachineType.M4K)
            {
                if (refTyp == ReflectorType.B_Dunn)
                {
                    Rotors.Add(new RotorSetting(RotorName.Beta, 0));
                }
                else
                {
                    Rotors.Add(new RotorSetting(RotorName.Gamma, 0));
                }
            }

            Rotors.Add(new RotorSetting(RotorName.I, 0));
            Rotors.Add(new RotorSetting(RotorName.II, 0));
            Rotors.Add(new RotorSetting(RotorName.III, 0));

            Plugs.Clear();

            for (int i = 0; i < rotors.Length; i++)
            {
                if (i <= Rotors.Count)
                {
                    Rotors[i].Name = rotors[i];
                    if (i <= ringPos.Length)
                    {
                        Rotors[i].RingSetting = ringPos[i];
                    }
                }
            }

            for(int i=0; i<10; i++)
            {
                if (i <= plugs.Length)
                {
                    Plugs.Add(new PlugSetting(plugs[i]));
                }
            }
        }
        public Settings(MachineType typ, ReflectorType refTyp, params object[] args)
            : this(typ, refTyp)
        {
            MachineType = typ;
            ReflectorType = refTyp;

            if (MachineType == MachineType.M4K)
            {
                if (refTyp == ReflectorType.B_Dunn)
                {
                    Rotors.Add(new RotorSetting(RotorName.Beta, 0));
                }
                else
                {
                    Rotors.Add(new RotorSetting(RotorName.Gamma, 0));
                }
            }

            Rotors.Add(new RotorSetting(RotorName.I, 0));
            Rotors.Add(new RotorSetting(RotorName.II, 0));
            Rotors.Add(new RotorSetting(RotorName.III, 0));

            Plugs.Clear();

            List<RotorName> rotorNames = new List<RotorName>();
            List<int> ringSettings = new List<int>();
            List<string> plugSettings = new List<string>();

            foreach(var item in args)
            {
                if(item is RotorName)
                {
                    rotorNames.Add((RotorName)item);
                    continue;
                }

                if(item is int)
                {
                    ringSettings.Add((int)item);
                    continue;
                }

                if(item is string)
                {
                    plugSettings.Add((string)item);
                    continue;
                }
            }

            for (int i = 0; i < rotorNames.Count; i++)
            {
                if (i <= Rotors.Count)
                {
                    Rotors[i].Name = rotorNames[i];
                    if (i <= ringSettings.Count)
                    {
                        Rotors[i].RingSetting = ringSettings[i];
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (i <= plugSettings.Count)
                {
                    Plugs.Add(new PlugSetting(plugSettings[i]));
                }
            }

        }

        public void Save(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (FileStream stm = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ser.Serialize(stm, this);
            }
        }

        public static Settings Open(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Settings));
            using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return (Settings)ser.Deserialize(stm);
            }
        }
        public static Settings Empty()
        {
            Settings s = new Settings();

            s.MachineType = MachineType.M3;
            s.ReflectorType = ReflectorType.B;

            s.Rotors.Add(new RotorSetting { Name = RotorName.I, RingSetting = 0 });
            s.Rotors.Add(new RotorSetting { Name = RotorName.II, RingSetting = 0 });
            s.Rotors.Add(new RotorSetting { Name = RotorName.III, RingSetting = 0 });

            s.Grund = "AAAA";

            s.Kenngruppen.Add("AAA");
            s.Kenngruppen.Add("BBB");
            s.Kenngruppen.Add("CCC");
            s.Kenngruppen.Add("DDD");

            return s;
        }
        public static Settings Random(MachineType m)
        {
            Machine mach = new Machine(m);

            Settings result = new Settings();

            result.MachineType = m;
            result.ReflectorType = mach.AvailableReflectors[_rnd.Next(mach.AvailableReflectors.Count)].ReflectorType;

            List<RotorName> availableNames = mach.AvailableRotors.Select(r => r.RotorName).ToList();

            if (m == MachineType.M4K)
            {
                availableNames.Remove(RotorName.Beta);
                availableNames.Remove(RotorName.Gamma);

                if (_rnd.Next(2) == 1)
                {
                    result.Rotors.Add(new RotorSetting { Name = RotorName.Beta, RingSetting = _rnd.Next(Constants.ALPHABET.Length) });
                }
                else
                {
                    result.Rotors.Add(new RotorSetting { Name = RotorName.Gamma, RingSetting = _rnd.Next(Constants.ALPHABET.Length) });
                }
            }

            for (int i = 0; i < 3; i++)
            {
                RotorName name = availableNames[_rnd.Next(availableNames.Count)];
                result.Rotors.Add(new RotorSetting { Name = name, RingSetting = _rnd.Next(Constants.ALPHABET.Length) });
                availableNames.Remove(name);
            }

            List<Tuple<string, double>> letters = new List<Tuple<string, double>>();
            foreach (char c in Constants.ALPHABET)
            {
                letters.Add(new Tuple<string, double>(c.ToString(), _rnd.NextDouble()));
            }
            letters.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            for (int i = 0; i < 10; i++)
            {
                var f = letters.First();
                var l = letters.Last();

                string a = f.Item1;
                string b = l.Item1;

                if (a.CompareTo(b) > 0)
                {
                    result.Plugs.Add(new PlugSetting { LetterA = b, LetterB = a });
                }
                else
                {
                    result.Plugs.Add(new PlugSetting { LetterA = a, LetterB = b });
                }

                letters.Remove(f);
                letters.Remove(l);
            }

            result.Plugs.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

            if (result.MachineType != MachineType.M3)
            {
                result.Grund = RandomUtil.GenerateSequence(4, Constants.ALPHABET, false, false);
                result.Kenngruppen.Clear();
            }
            else
            {
                result.Grund = string.Empty;
                result.Kenngruppen.Clear();

                for (int i=0; i<4; i++)
                {
                    result.Kenngruppen.Add(RandomUtil.GenerateSequence(3, Constants.ALPHABET, false, false));
                }
            }

            return result;
        }
        public static Settings ParseSettingLine(string line)
        {
            return Formatting.ParseSettingLine(line);
        }

        public override string ToString()
        {
            return Formatting.SettingLine(this, Day);
        }

        public void Validate()
        {
            List<BrokenRule> brokenRules = new List<BrokenRule>();
            if (!Validation.Validate(this, out brokenRules))
            {
                throw new ValidationException(string.Join("\r\n", brokenRules.Select(r => r.Message))) { BrokenRules = brokenRules };
            }
        }

        public bool Equals(Settings other)
        {
            if (other == null) return false;

            if (object.ReferenceEquals(this, other)) return true;

            if (other.MachineType != MachineType) return false;
            if (other.ReflectorType != ReflectorType) return false;

            if (other.Rotors.Count != Rotors.Count) return false;

            for(int i=0; i<Rotors.Count; i++)
            {
                if (other.Rotors[i].Name != Rotors[i].Name) return false;
                if (other.Rotors[i].RingSetting != Rotors[i].RingSetting) return false;
            }

            if (other.Plugs.Count != Plugs.Count) return false;

            other.Plugs.Sort((p1, p2) => p1.ToString().CompareTo(p2.ToString()));
            Plugs.Sort((p1, p2) => p1.ToString().CompareTo(p2.ToString()));

            for(int i=0; i<Plugs.Count; i++)
            {
                if (Plugs[i].ToString() != other.Plugs[i].ToString()) return false;
            }

            return true;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (object.ReferenceEquals(this, obj)) return true;

            Settings s = obj as Settings;
            if (s == null) return false;

            return Equals(s);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Settings a, Settings b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }
        public static bool operator !=(Settings a, Settings b)
        {
            return !(a == b);
        }
    }
}
