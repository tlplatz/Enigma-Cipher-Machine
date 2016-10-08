using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enigma.Configuration;
using Messaging.Util;

namespace Messaging.Army
{
    public class ArmyMessage : BaseMessage
    {

        public ArmyMessage(string settingsFileName, string input, int day)
        {
            MonthlySettingsFileName = settingsFileName;
            PlainText = input;
            Day = day;

            List<ArmyMessagePart> _innerParts = new List<ArmyMessagePart>();

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
            string paddedInput = Utility.GetPaddedString(cleanInput, 5);

            string[] parts = Utility.GetMessageParts(paddedInput, 5, 49);

            Settings s = _monthlySettings.DailySettings.FirstOrDefault(ss => ss.Day == Day);
            if (s == null) s = _monthlySettings.DailySettings.Last();

            for (int i = 0; i < parts.Length; i++)
            {
                _innerParts.Add(new ArmyMessagePart(s, parts[i], i, parts.Length));
            }

            Parts = _innerParts;
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

            string[] tokens = BreakMessageIntoGroups(message);
            string[] headers = GetHeaders(tokens);
            List<string[]> parts = GetParts(headers, tokens);

            List<string> decrypted = new List<string>();

            for (int i = 0; i < headers.Length; i++)
            {
                decrypted.Add(DecryptPart(headers[i], parts[i], _monthlySettings));
            }

            return string.Concat(decrypted);
        }

        private static string[] BreakMessageIntoGroups(string input)
        {
            return input.Split(new string[] { "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries); ;
        }

        private static string[] GetHeaders(string[] lines)
        {
            return lines.Where(t => t.Contains("=")).ToArray();
        }

        private static List<string[]> GetParts(string[] headers, string[] lines)
        {
            List<List<string>> parts = new List<List<string>>();

            for (int i = 0; i < headers.Length; i++)
            {
                parts.Add(new List<string>());
            }

            int partIndex = 0;
            bool first = true;

            foreach (string s in lines)
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

            return parts.Select(p => p.ToArray()).ToList();
        }

        private static string DecryptPart(string header, string[] groups, MonthlySettings settings)
        {
            string[] headerTokens = header.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

            string last = headerTokens.Last().Trim();
            string[] messageRotorSettings = last.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            string rotorPos = messageRotorSettings[0];
            string indicator = messageRotorSettings[1];

            Settings partSettings = GetSettingsByKenngruppen(settings, groups[0]);

            Enigma.Message msg = new Enigma.Message(partSettings);
            string msgKey = msg.Encrypt(indicator, rotorPos);

            string partBody = string.Concat(groups.Skip(1).Take(groups.Length - 1));

            return msg.Decrypt(partBody, msgKey);
        }

        private static Settings GetSettingsByKenngruppen(MonthlySettings monSet, string kg)
        {
            string fh = kg.Substring(0, 3);
            string lh = kg.Substring(2, 3);

            foreach (var s in monSet.DailySettings)
            {
                foreach (var k in s.Kenngruppen)
                {
                    if (k == fh || k == lh)
                    {
                        return s;
                    }
                }
            }

            return null;
        }
    }
}
