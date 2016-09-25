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

        public static string Decrypt(string settingsFileName, string digraphFileName, string message)
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

            string[] headers = tokens.Where(t => t.Contains("/")).ToArray();

            List<List<string>> parts = new List<List<string>>();

            for (int i = 0; i < headers.Length; i++)
            {
                parts.Add(new List<string>());
            }

            int partIndex = 0;
            bool first = true;

            foreach (string s in tokens)
            {
                if (s.Contains("/"))
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

            string[,] digraphs = Utility.ParseDigraphTable(digraphFileName);

            List<string> decrypts = new List<string>();


            for (int i = 0; i < headers.Length; i++)
            {
                string[] headerTokens = headers[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string[] timestampTokens = headerTokens[1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                int day = int.Parse(timestampTokens[1]);

                Settings settings = _monthlySettings.DailySettings.FirstOrDefault(s => s.Day == day);

                string indicator1 = parts[i][0];
                string indicator2 = parts[i][1];

                string cipherText = string.Concat(parts[i].Skip(2).Take(parts[i].Count - 4));

                List<string> digs = new List<string>();
                digs.Add(indicator1.Substring(0, 2));
                digs.Add(indicator1.Substring(2, 2));
                digs.Add(indicator2.Substring(0, 2));
                digs.Add(indicator2.Substring(2, 2));

                List<string> encDigs = new List<string>();
                foreach(string s in digs)
                {
                    int x = Utility.ALPHA.IndexOf(s[0]);
                    int y = Utility.ALPHA.IndexOf(s[1]);
                    encDigs.Add(digraphs[x, y]);
                }

                string startPosition = string.Concat(encDigs.Select(s => s[0]));
                string messageKey = string.Concat(encDigs.Select(s => s[1]));

                if (settings.Grund.Length == 3)
                {
                    string trimmedStart = startPosition.Substring(0, 3);
                    string trimmedKey = messageKey.Substring(1, 3);

                    Enigma.Message msg = new Enigma.Message(settings);
                    string actualStart = msg.Encrypt(trimmedKey, trimmedStart);

                    decrypts.Add(msg.Decrypt(cipherText, actualStart));
                }
                else
                {
                    Enigma.Message msg = new Enigma.Message(settings);
                    string actualStart = msg.Encrypt(messageKey, startPosition);

                    decrypts.Add(msg.Decrypt(cipherText, actualStart));
                }
            }

            return string.Concat(decrypts);
        }
    }
}
