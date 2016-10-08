using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enigma.Configuration;
using Messaging.Util;

namespace Messaging.Navy
{
    public class NavyMessage : BaseMessage
    {
        public NavyMessage(string settingsFileName, string digraphFileName, string input, int day, string callSign)
        {
            MonthlySettingsFileName = settingsFileName;
            PlainText = input;
            Day = day;

            List<NavyMessagePart> _innerParts = new List<NavyMessagePart>();

            FileInfo fi = new FileInfo(MonthlySettingsFileName);

            if (fi.Exists)
            {
                if (fi.Extension.ToLower() == ".xml")
                {
                    _monthlySettings = MonthlySettings.Open(MonthlySettingsFileName);
                }
                else
                {
                    using (StreamReader rdr = fi.OpenText())
                    {
                        string fileContent = rdr.ReadToEnd();
                        _monthlySettings = MonthlySettings.Parse(fileContent);
                    }
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
                _innerParts.Add(new NavyMessagePart(s, callSign, parts[i], i, parts.Length, digs));
            }

            Parts = _innerParts;
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

            string[] lines = BreakMessageIntoGroups(message);
            string[] headers = GetHeaders(lines);
            List<string[]> parts = GetParts(headers, lines);

            string[,] digraphs = Utility.ParseDigraphTable(digraphFileName);

            List<string> decrypts = new List<string>();

            for (int i = 0; i < headers.Length; i++)
            {
                decrypts.Add(DecryptPart(headers[i], _monthlySettings, parts[i], digraphs));
            }

            return string.Concat(decrypts);
        }

        private static string[] BreakMessageIntoGroups(string input)
        {
            return input.Split(new string[] { "\r", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string[] GetHeaders(string[] lines)
        {
            return lines.Where(t => t.Contains("/")).ToArray();
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

            return parts.Select(s => s.ToArray()).ToList();
        }

        private static string DecryptPart(string header, MonthlySettings monSet, string[] groups, string[,] digraphs)
        {
            string[] headerTokens = header.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string[] timestampTokens = headerTokens[1].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            int day = int.Parse(timestampTokens[1]);

            Settings settings = monSet.DailySettings.FirstOrDefault(s => s.Day == day);

            string indicator1 = groups[0];
            string indicator2 = groups[1];

            string cipherText = string.Concat(groups.Skip(2).Take(groups.Length - 4));
            string messageKey, startPosition;
            DecryptDigraphs(indicator1, indicator2, digraphs, out startPosition, out messageKey);

            if (settings.Grund.Length == 3)
            {
                string trimmedStart = startPosition.Substring(1, 3);
                string trimmedKey = messageKey.Substring(0, 3);

                Enigma.Message msg = new Enigma.Message(settings);
                string actualStart = msg.Encrypt(trimmedKey, trimmedStart);

                return msg.Decrypt(cipherText, actualStart);
            }
            else
            {
                Enigma.Message msg = new Enigma.Message(settings);
                string actualStart = msg.Encrypt(messageKey, startPosition);

                return msg.Decrypt(cipherText, actualStart);
            }
        }

        private static void DecryptDigraphs(string indicator1, string indicator2, string[,] digraphs, out string startPosition, out string messageKey)
        {
            List<string> digs = new List<string>();
            digs.Add(indicator1.Substring(0, 2));
            digs.Add(indicator1.Substring(2, 2));
            digs.Add(indicator2.Substring(0, 2));
            digs.Add(indicator2.Substring(2, 2));

            List<string> encDigs = new List<string>();
            foreach (string s in digs)
            {
                int x = Utility.ALPHA.IndexOf(s[0]);
                int y = Utility.ALPHA.IndexOf(s[1]);
                encDigs.Add(digraphs[x, y]);
            }

            startPosition = string.Concat(encDigs.Select(s => s[0]));
            messageKey = string.Concat(encDigs.Select(s => s[1]));
        }
    }
}
