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

        private ArmyMessage()
        {

        }
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

        public static string Decrypt(string settingsFileName, string message)
        {
            MonthlySettings _monthlySettings = null;

            FileInfo fi = new FileInfo(settingsFileName);

            if (fi.Exists)
            {
                if (fi.Extension.ToLower() == ".xml")
                {
                    _monthlySettings = MonthlySettings.Open(settingsFileName);
                }
                else
                {
                    string fileContent = fi.OpenText().ReadToEnd();
                    _monthlySettings = MonthlySettings.Parse(fileContent);
                }
            }

            string[] tokens = message.Split(new string[] { "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            string[] headers = tokens.Where(t => t.Contains("=")).ToArray();

            List<List<string>> parts = new List<List<string>>();

            for(int i=0; i<headers.Length; i++)
            {
                parts.Add(new List<string>());
            }

            int partIndex = 0;
            bool first = true;

            foreach(string s in tokens)
            {
                if (s.Contains("="))
                {
                    //header
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    else
                    {
                        partIndex++;
                        continue;
                    }
                }

                if (partIndex >= parts.Count) break;
                parts[partIndex].Add(s);
            }


            List<string> decrypted = new List<string>();

            for(int i=0; i<headers.Length; i++)
            {
                string hdr = headers[i];
                string[] header_tokens = hdr.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                string last = header_tokens.Last().Trim();
                string[] messageRotorSettings = last.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                string rotorPos = messageRotorSettings[0];
                string indicator = messageRotorSettings[1];

                Settings partSettings = GetSettingsByKenngruppen(_monthlySettings, parts[i][0]);

                Enigma.Message msg = new Enigma.Message(partSettings);
                string msgKey = msg.Encrypt(indicator, rotorPos);

                string partBody = string.Concat(parts[i].Skip(1).Take(parts[i].Count - 1));

                decrypted.Add(msg.Decrypt(partBody, msgKey));

            }

            return string.Concat(decrypted);
        }

        private static Settings GetSettingsByKenngruppen(MonthlySettings monSet, string kg)
        {
            string fh = kg.Substring(0, 3);
            string lh = kg.Substring(2, 3);

            foreach(var s in monSet.DailySettings)
            {
                foreach(var k in s.Kenngruppen)
                {
                    if(k == fh || k == lh)
                    {
                        return s;
                    }
                }
            }

            return null;
        }


    }
}
