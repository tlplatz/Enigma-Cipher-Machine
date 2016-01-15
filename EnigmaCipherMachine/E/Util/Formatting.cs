using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WizardNet.Enigma.Configuration;
using WizardNet.Enigma.Enums;

namespace WizardNet.Enigma.Util
{
    public class Formatting
    {
        public static string SettingLine(Settings s, int day)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" {0:00} |", day);
            sb.AppendFormat(" {0} |", s.ReflectorType.ToString()[0]);
            sb.AppendFormat(" {0} |", string.Join(" ", s.Rotors.Select(r => r.Name.ToString().PadRight(6))).PadRight(27));
            sb.AppendFormat(" {0} |", string.Join(" ", s.Rotors.Select(r => (r.RingSetting + 1).ToString("00"))).PadRight(12));
            sb.AppendFormat(" {0}", string.Join(" ", s.Plugs.Select(p => p.ToString())));

            return sb.ToString();
        }
        public static string MonthlySettings(string title, int year, int month, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(TitleLine(title, year, month));
            sb.AppendLine();
            sb.AppendLine(DashedLine());
            sb.AppendLine(HeaderLine());
            sb.AppendLine(DashedLine());

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

            sb.AppendLine(DashedLine());

            return sb.ToString();
        }
        public static string MonthlySettings(string title, int year, int month, MachineType t, ReflectorType r, IEnumerable<Settings> settings)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(TitleLine(title, year, month));
            sb.AppendLine();
            sb.AppendLine(DashedLine());
            sb.AppendLine(HeaderLine());
            sb.AppendLine(DashedLine());

            foreach (var s in settings)
            {
                sb.AppendLine(SettingLine(s, s.Day));

            }
            sb.AppendLine(DashedLine());

            return sb.ToString();
        }
        public static string HeaderLine()
        {
            return "Tag |UKW|         Walzenlage          | Ringstellung |     Steckerverbindungen";
        }
        public static string TitleLine(string title, int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);

            const int width = 84;
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
        public static string DashedLine()
        {
            return "------------------------------------------------------------------------------------";
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
    }
}
