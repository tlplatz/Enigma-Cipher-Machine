using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Messaging
{
    public class NavyMessage
    {
        private MonthlySettings _monthlySettings;

        public string MonthlySettingsFileName { get; private set; }
        public string PlainText { get; private set; }
        public int Day { get; private set; }

        public List<NavyMessagePart> Parts { get; private set; }

        public NavyMessage(string settingsFileName, string digraphFileName, string input, int day, string callSign)
        {
            MonthlySettingsFileName = settingsFileName;
            PlainText = input;
            Day = day;

            Parts = new List<NavyMessagePart>();

            FileInfo fi = new FileInfo(MonthlySettingsFileName);

            if (fi.Exists)
            {
                if (fi.Extension.ToLower() == ".xml")
                {
                    _monthlySettings = MonthlySettings.Open(MonthlySettingsFileName);
                }
                else
                {
                    string fileContent = fi.OpenText().ReadToEnd();
                    _monthlySettings = MonthlySettings.Parse(fileContent);
                }
            }

            string cleanInput = Utility.CleanString(input);
            string paddedInput = Utility.GetPaddedString(cleanInput, 4);

            string[] parts = Utility.GetMessageParts(paddedInput, 4, 96);
            string[,] digs = Utility.ParseDigraphTable(digraphFileName);


            Settings s = _monthlySettings.DailySettings.FirstOrDefault(ss => ss.Day == Day);
            if (s == null) s = _monthlySettings.DailySettings.Last();

            for (int i = 0; i < parts.Length; i++)
            {
                Parts.Add(new NavyMessagePart(s, callSign, parts[i], i, parts.Length, digs));
            }
        }

        public override string ToString()
        {
            return string.Join("\r\n", Parts);
        }
    }
}
