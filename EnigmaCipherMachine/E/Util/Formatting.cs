using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enigma.Configuration;
using Enigma.Enums;

namespace Enigma.Util
{
    internal static class Formatting
    {
        public static string SettingLine(Settings s, int day)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" {0:00} |", day);
            sb.AppendFormat(" {0} |", s.ReflectorType.ToString()[0]);
            sb.AppendFormat(" {0} |", string.Join(" ", s.Rotors.Select(r => r.Name.ToString().PadRight(6))).PadRight(27));
            sb.AppendFormat(" {0} |", string.Join(" ", s.Rotors.Select(r => (r.RingSetting + 1).ToString("00"))).PadRight(12));
            sb.AppendFormat(" {0} |", string.Join(" ", s.Plugs.Select(p => p.ToString())));

            if (s.MachineType == MachineType.M3)
            {
                sb.AppendFormat(" {0}", string.Join(" ", s.Kenngruppen));
            }
            else
            {
                if (s.MachineType == MachineType.M3K)
                {
                    sb.Append(" ");
                }

                sb.AppendFormat(" {0}", s.Grund);
            }

            return sb.ToString();
        }

        public static string MonthlySettings(string title, int year, int month, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(TitleLine(t, title, year, month));
            sb.AppendLine();
            sb.AppendLine(DashedLine(t));
            sb.AppendLine(HeaderLine(t));
            sb.AppendLine(DashedLine(t));

            for (int i = DateTime.DaysInMonth(year, month); i >= 1; i--)
            {
                Settings s = Settings.Random(t);
                s.ReflectorType = r;
                if (t == MachineType.M4K)
                {
                    if (s.ReflectorType == ReflectorType.B_Dunn)
                    {
                        s.Rotors[0].Name = RotorName.Beta;

                        if (compatibilityMode)
                        {
                            s.Rotors[0].RingSetting = 0;
                        }
                    }
                    else
                    {
                        s.Rotors[0].Name = RotorName.Gamma;

                        if (compatibilityMode)
                        {
                            s.Rotors[0].RingSetting = 0;
                        }
                    }
                }

                sb.AppendLine(SettingLine(s, i));
            }

            sb.AppendLine(DashedLine(t));

            return sb.ToString();
        }
        public static string MonthlySettings(string title, int year, int month, MachineType t, ReflectorType r, IEnumerable<Settings> settings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(TitleLine(t, title, year, month));
            sb.AppendLine();
            sb.AppendLine(DashedLine(t));
            sb.AppendLine(HeaderLine(t));
            sb.AppendLine(DashedLine(t));

            foreach (var s in settings)
            {
                sb.AppendLine(SettingLine(s, s.Day));

            }
            sb.AppendLine(DashedLine(t));

            return sb.ToString();
        }
        public static string MonthlySettings(string title, int year, int month, MachineType t, IEnumerable<Settings> settings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(TitleLine(t, title, year, month));
            sb.AppendLine();
            sb.AppendLine(DashedLine(t));
            sb.AppendLine(HeaderLine(t));
            sb.AppendLine(DashedLine(t));

            foreach (var s in settings)
            {
                sb.AppendLine(SettingLine(s, s.Day));

            }
            sb.AppendLine(DashedLine(t));

            return sb.ToString();
        }

        public static string HeaderLine(MachineType mt)
        {
            if (mt == MachineType.M3)
            {
                return "Tag |UKW|         Walzenlage          | Ringstellung |      Steckerverbindungen      |   Kenngruppen";
            }
            else
            {
                return "Tag |UKW|         Walzenlage          | Ringstellung |      Steckerverbindungen      | Grund";
            }

        }
        public static string TitleLine(MachineType mt, string title, int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);


            const int width_m3 = 103;
            const int width_m4 = 92;

            int width = mt == MachineType.M3 ? width_m3 : width_m4;

            int left, right;

            int i = width - 8 - title.Length - 8;
            left = i / 2;
            right = i - left;

            return string.Format(" GEHEIM!{0}{1}{2}{3:MMM} {4:yyyy}",
                new string(' ', left),
                title,
                new string(' ', right),
                dt,
                dt);
        }
        public static string DashedLine(MachineType mt)
        {
            if (mt == MachineType.M3)
            {
                return "----+---+-----------------------------+--------------+-------------------------------+-----------------";
            }
            else
            {
                return "----+---+-----------------------------+--------------+-------------------------------+------";
            }
        }

        public static string OutputGroups(string input, int groupSize = 5, int perRow = 5, string delimiter = "  ")
        {
            StringBuilder sb = new StringBuilder();

            int groupCounter = 0;
            int rowCounter = 0;

            foreach (char c in input)
            {
                sb.Append(c);

                groupCounter++;

                if (groupCounter == groupSize)
                {
                    groupCounter = 0;
                    rowCounter++;

                    if (rowCounter != perRow)
                    {
                        sb.Append(delimiter);
                    }
                }

                if (rowCounter == perRow)
                {
                    sb.Append("\r\n");
                    rowCounter = 0;
                }
            }

            return sb.ToString().Trim();
        }
        public static string CleanInput(string input)
        {
            return string.Concat(input.ToUpper().Where(c => Constants.ALPHABET.Contains(c)));
        }

        public static Settings ParseSettingLine(string line)
        {
            string[] tokens = line.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

            Settings result = new Settings();

            string ukw = tokens[1].Trim();
            string rotors = tokens[2].Trim();
            string rings = tokens[3].Trim();
            string plugs = tokens[4].Trim();
            string indicator = null;

            if (tokens.Length == 6)
                indicator = tokens[5].Trim();

            string[] rotorNames = rotors.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] ringSettings = rings.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] plugSettings = plugs.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] indicatorValues = new string[0];

            if (tokens.Length == 6)
                indicatorValues = indicator.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (rotorNames.Length == 4)
            {
                result.MachineType = MachineType.M4K;

                result.Grund = indicator;

                if (ukw == "B")
                {
                    result.ReflectorType = ReflectorType.B_Dunn;
                }
                else
                {
                    result.ReflectorType = ReflectorType.C_Dunn;
                }
            }
            else
            {
                result.MachineType = MachineType.M3;

                if (indicatorValues.Any())
                    result.Kenngruppen.AddRange(indicatorValues);

                if (ukw == "B")
                {
                    result.ReflectorType = ReflectorType.B;
                }
                else
                {
                    result.ReflectorType = ReflectorType.C;
                }
            }

            result.Rotors.Clear();
            result.Rotors.AddRange(rotorNames.Select(n => new RotorSetting((RotorName)Enum.Parse(typeof(RotorName), n), 0)));

            for (int i = 0; i < ringSettings.Length; i++)
            {
                result.Rotors[i].RingSetting = int.Parse(ringSettings[i]) - 1;
            }

            foreach (string s in plugSettings)
            {
                result.Plugs.Add(new PlugSetting(s));
            }

            if (result.Rotors.Any(r => r.Name == RotorName.VI ||
                r.Name == RotorName.VII ||
                r.Name == RotorName.VIII ||
                r.Name == RotorName.Beta ||
                r.Name == RotorName.Gamma) && result.Rotors.Count == 3)
            {
                result.MachineType = MachineType.M3K;
            }

            return result;
        }
    }
}
