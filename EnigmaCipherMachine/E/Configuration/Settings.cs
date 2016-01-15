using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WizardNet.Enigma.Enums;
using WizardNet.Enigma.Util;

namespace WizardNet.Enigma.Configuration
{
    [Serializable]
    public class Settings
    {
        private static Random _rnd = new Random();

        public int Day { get; set; }
        public MachineType MachineType { get; set; }
        public ReflectorType ReflectorType { get; set; }

        public List<RotorSetting> Rotors { get; set; }
        public List<PlugSetting> Plugs { get; set; }

        public Settings()
        {
            Rotors = new List<RotorSetting>();
            Plugs = new List<PlugSetting>();
        }
        public Settings(MachineType typ, ReflectorType refTyp)
            : this()
        {
            MachineType = typ;
            ReflectorType = refTyp;
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

            return result;
        }

        public override string ToString()
        {
            return Formatting.SettingLine(this, Day);
        }
    }
}
