using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Messaging
{
    public class ArmyMessage
    {
        private MonthlySettings _monthlySettings;

        public string MonthlySettingsFileName { get; private set; }
        public string PlainText { get; private set; }
        public int Day { get; private set; }

        public List<ArmyMessagePart> Parts { get; private set; }

        public ArmyMessage(string settingsFileName, string input, int day)
        {
            MonthlySettingsFileName = settingsFileName;
            PlainText = input;
            Day = day;

            Parts = new List<ArmyMessagePart>();

            FileInfo fi = new FileInfo(MonthlySettingsFileName);

            if (fi.Exists)
            {
                if(fi.Extension.ToLower() == ".xml")
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
            string paddedInput = Utility.GetPaddedString(cleanInput, 5);

            string[] parts = Utility.GetMessageParts(paddedInput, 5, 49);

            Settings s = _monthlySettings.DailySettings.FirstOrDefault(ss => ss.Day == Day);
            if (s == null) s = _monthlySettings.DailySettings.Last();

            for(int i=0; i<parts.Length; i++)
            {
                Parts.Add(new ArmyMessagePart(s, parts[i], i, parts.Length));
            }
        }

        public override string ToString()
        {
            return string.Join("\r\n", Parts);
        }
    }
}
