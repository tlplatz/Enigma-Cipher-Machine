using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Util
{
    internal static class RandomUtil
    {
        internal static Random _rand = new Random();

        public static string GenerateSequence(int length, string values, bool unique, bool sorted)
        {
            List<string> result = new List<string>();

            if (!unique)
            {
                while (result.Count < length)
                {
                    result.Add(values[_rand.Next(values.Length)].ToString());
                }
            }
            else
            {
                List<Tuple<string, double>> letters = new List<Tuple<string, double>>();

                foreach (char c in values)
                {
                    letters.Add(new Tuple<string, double>(c.ToString(), _rand.NextDouble()));
                }
                letters.Sort((l1, l2) => l1.Item2.CompareTo(l2.Item2));

                while (result.Any() && result.Count < length)
                {
                    var letter = letters.First();
                    result.Add(letter.Item1);
                    letters.Remove(letter);
                }
            }

            if (sorted)
            {
                return string.Concat(result.Select(r => r).OrderBy(r => r));
            }
            else
            {
                return string.Concat(result);
            }
        }
        public static string ReciprocalAlphabet()
        {
            string[] result = new string[Constants.ALPHABET.Length];

            List<Tuple<string, double>> letters = new List<Tuple<string, double>>();
            foreach (char c in Constants.ALPHABET)
            {
                letters.Add(new Tuple<string, double>(c.ToString(), _rand.NextDouble()));
            }
            letters.Sort((l1, l2) => l1.Item2.CompareTo(l2.Item2));

            while (letters.Any())
            {
                var f = letters.First();
                var l = letters.Last();

                result[Constants.ALPHABET.IndexOf(f.Item1)] = l.Item1;
                result[Constants.ALPHABET.IndexOf(l.Item1)] = f.Item1;

                letters.Remove(f);
                letters.Remove(l);
            }

            return string.Concat(result);
        }
        public static string KeySheet(int groupSize)
        {
            int i = 0, j = 0, k = 0, l = 0, m = 0;
            int counter = 0;
            List<KeySheetValue> values = new List<KeySheetValue>();
            List<string> keyValues = new List<string>();

            while(counter<Math.Pow(6, 5))
            {
                counter += 1;

                i += 1;

                if (i == 6)
                {
                    i = 0;
                    j += 1;

                    if(j == 6)
                    {
                        j = 0;
                        k += 1;

                        if (k == 6)
                        {
                            k = 0;
                            l += 1;

                            if (l == 6)
                            {
                                l = 0;
                                m += 1;
                            }
                        }
                    }
                }

                values.Add(new KeySheetValue { Key = string.Format("{0}{1}{2}{3}{4}", i + 1, j + 1, k + 1, l + 1, m + 1) });
            }

            values.Sort((v1, v2) => v1.Key.CompareTo(v2.Key));

            while (keyValues.Count<Math.Pow(6, 5))
            {
                string temp = GenerateSequence(groupSize, Constants.ALPHABET, false, false);
                if (!keyValues.Contains(temp))
                {
                    keyValues.Add(temp);
                }
            }
            keyValues.Sort((v1, v2) => v1.CompareTo(v2));

            for(int x=0; x<keyValues.Count; x++)
            {
                values[x].Value = keyValues[x];
            }

            StringBuilder sb = new StringBuilder();
            int steps = (int)(Math.Pow(6.0, 5.0) / 6.0);

            for (int x = 0; x < steps; x++)
            {
                sb.AppendLine(string.Join("\t", values.Skip(x * 6).Take(6).Select(t => t.ToString())));
                if ((x + 1) % 36 == 0)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }

    internal class KeySheetValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}  {1}", Key, Value);
        }
    }
}
