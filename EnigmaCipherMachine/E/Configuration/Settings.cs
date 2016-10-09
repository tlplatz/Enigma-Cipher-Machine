using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Enigma.Enums;
using Enigma.Util;

namespace Enigma.Configuration
{
    [Serializable]
    public class Settings : IEquatable<Settings>, ICloneable
    {
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

            for (int i = 0; i < 10; i++)
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

            foreach (var item in args)
            {
                if (item is RotorName)
                {
                    rotorNames.Add((RotorName)item);
                    continue;
                }

                if (item is int)
                {
                    ringSettings.Add((int)item);
                    continue;
                }

                if (item is string)
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
            result.ReflectorType = mach.AvailableReflectors[RandomUtil._rand.Next(mach.AvailableReflectors.Count)].ReflectorType;

            List<RotorName> availableNames = mach.AvailableRotors.Select(r => r.RotorName).ToList();

            if (m == MachineType.M4K)
            {
                availableNames.Remove(RotorName.Beta);
                availableNames.Remove(RotorName.Gamma);

                if (RandomUtil._rand.Next(2) == 1)
                {
                    result.Rotors.Add(new RotorSetting { Name = RotorName.Beta, RingSetting = RandomUtil._rand.Next(Constants.ALPHABET.Length) });
                }
                else
                {
                    result.Rotors.Add(new RotorSetting { Name = RotorName.Gamma, RingSetting = RandomUtil._rand.Next(Constants.ALPHABET.Length) });
                }
            }

            for (int i = 0; i < 3; i++)
            {
                RotorName name = availableNames[RandomUtil._rand.Next(availableNames.Count)];
                result.Rotors.Add(new RotorSetting { Name = name, RingSetting = RandomUtil._rand.Next(Constants.ALPHABET.Length) });
                availableNames.Remove(name);
            }

            List<Tuple<string, double>> letters = new List<Tuple<string, double>>();
            foreach (char c in Constants.ALPHABET)
            {
                letters.Add(new Tuple<string, double>(c.ToString(), RandomUtil._rand.NextDouble()));
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

                if (result.MachineType == MachineType.M3K)
                {
                    result.Grund = result.Grund.Substring(0, 3);
                }

                result.Kenngruppen.Clear();
            }
            else
            {
                result.Grund = string.Empty;
                result.Kenngruppen.Clear();

                for (int i = 0; i < 4; i++)
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
            if (!Validator.Validate(this, out brokenRules))
            {
                throw new ValidationException(string.Join("\r\n", brokenRules.Select(r => r.Message))) { BrokenRules = brokenRules };
            }
        }

        #region IEquatable Interface
        public bool Equals(Settings other)
        {
            if (other == null) return false;

            if (object.ReferenceEquals(this, other)) return true;

            if (other.MachineType != MachineType) return false;
            if (other.ReflectorType != ReflectorType) return false;

            if (other.Rotors.Count != Rotors.Count) return false;

            for (int i = 0; i < Rotors.Count; i++)
            {
                if (other.Rotors[i].Name != Rotors[i].Name) return false;
                if (other.Rotors[i].RingSetting != Rotors[i].RingSetting) return false;
            }

            if (other.Plugs.Count != Plugs.Count) return false;

            other.Plugs.Sort((p1, p2) => p1.ToString().CompareTo(p2.ToString()));
            Plugs.Sort((p1, p2) => p1.ToString().CompareTo(p2.ToString()));

            for (int i = 0; i < Plugs.Count; i++)
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
        #endregion

        #region ICloneable Interface
        public object Clone()
        {
            return Settings.ParseSettingLine(this.ToString());
        }
        #endregion

        #region Operator Implementation
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
        #endregion

        [XmlIgnore]
        public string EnigmaSimFormat
        {
            get
            {
                Stecker s = new Stecker();
                s.PlugSettings = string.Join(" ", Plugs);

                string plugWiring = s.Wiring;

                string reflector = null;

                if (MachineType == MachineType.M4K)
                {
                    if (ReflectorType == ReflectorType.B_Dunn)
                    {
                        reflector = "05";
                    }
                    else
                    {
                        reflector = "06";
                    }
                }

                if (MachineType == MachineType.M3K)
                {
                    if (ReflectorType == ReflectorType.B)
                    {
                        reflector = "03";
                    }
                    else
                    {
                        reflector = "04";
                    }
                }

                if(MachineType == MachineType.M3)
                {
                    if (ReflectorType == ReflectorType.B)
                    {
                        reflector = "01";
                    }
                    else
                    {
                        reflector = "02";
                    }
                }

                string rotors = string.Concat(Rotors.Select(r => r.Name).Reverse().Select(r => ((int)r + 1).ToString("00")));
                string rings = string.Concat(Rotors.Select(r => r.RingSetting).Reverse().Select(r => r.ToString("00")));

                if(MachineType == MachineType.M4K)
                {
                    return string.Format("ENIGMASIM{0}{1}{2}{3}", plugWiring, reflector, rotors, rings);
                }
                return string.Format("ENIGMASIM{0}{1}{2}00{3}00", plugWiring, reflector, rotors, rings);
            }
            set
            {
                string plugWiring = value.Substring(9, 26);
                string reflector = value.Substring(35, 2);
                string rotors = value.Substring(37, 8);
                string rings = value.Substring(45, 8);

                List<string> plugs = new List<string>();

                List<char> check = Constants.ALPHABET.Select(c => c).ToList(); 

                for(int i=0; i<Constants.ALPHABET.Length; i++)
                {
                    if(plugWiring[i] == Constants.ALPHABET[i])
                    {
                        check.Remove(plugWiring[i]);
                    }
                    else
                    {
                        if(check.Contains(plugWiring[i]) && check.Contains(Constants.ALPHABET[i]))
                        {
                            if (plugWiring[i] < Constants.ALPHABET[i])
                            {
                                plugs.Add(string.Format("{0}{1}", plugWiring[i], Constants.ALPHABET[i]));
                            }
                            else
                            {
                                plugs.Add(string.Format("{0}{1}", Constants.ALPHABET[i], plugWiring[i]));
                            }                            

                            check.Remove(plugWiring[i]);
                            check.Remove(Constants.ALPHABET[i]);
                        }
                    }
                }


                Plugs.Clear();
                Plugs.AddRange(plugs.OrderBy(p=>p).Select(s => new PlugSetting(s)));

                switch (reflector)
                {
                    case "01":
                        MachineType = MachineType.M3;
                        ReflectorType = ReflectorType.B;
                        break;
                    case "02":
                        MachineType = MachineType.M3;
                        ReflectorType = ReflectorType.C;
                        break;
                    case "03":
                        MachineType = MachineType.M3K;
                        ReflectorType = ReflectorType.B;
                        break;
                    case "04":
                        MachineType = MachineType.M3K;
                        ReflectorType = ReflectorType.C;
                        break;
                    case "05":
                        MachineType = MachineType.M4K;
                        ReflectorType = ReflectorType.B_Dunn;
                        break;
                    case "06":
                        MachineType = MachineType.M4K;
                        ReflectorType = ReflectorType.C_Dunn;
                        break;
                }

                List<RotorName> rotorList = new List<RotorName>();
                for(int i=0; i<8; i += 2)
                {
                    string s = rotors.Substring(i, 2);
                    int rn = int.Parse(s) - 1;
                    rotorList.Add((RotorName)rn);
                }
                rotorList.Reverse();
                if(MachineType!= MachineType.M4K)
                {
                    rotorList.RemoveAt(0);
                }

                List<int> ringList = new List<int>();
                for (int i = 0; i < 8; i += 2)
                {
                    string s = rings.Substring(i, 2);
                    int rn = int.Parse(s);
                    ringList.Add(rn);
                }
                ringList.Reverse();
                if (MachineType != MachineType.M4K)
                {
                    ringList.RemoveAt(0);
                }

                Rotors.Clear();

                for(int i=0; i<rotorList.Count; i++)
                {
                    Rotors.Add(new RotorSetting(rotorList[i], ringList[i]));
                }
            }
        }
    }
}
