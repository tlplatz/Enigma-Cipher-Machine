using System;
using System.Collections.Generic;
using System.Linq;

namespace WizardNet.Enigma.Util
{
    public class RandomUtil
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
    }
}
