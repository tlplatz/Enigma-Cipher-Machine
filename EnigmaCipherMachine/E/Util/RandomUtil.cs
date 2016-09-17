using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Util
{
    internal static class RandomUtil
    {
        internal static Random _rand = new Random();
        private const string KeySheetNames = "ABCDEFGHJKLM";

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
        public static string GenerateKeySheet(int year)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(GenerateKeyTable(year));
            sb.AppendLine();

            for (int i = 0; i < 12; i++)
            {
                sb.Append(GenerateKeySheet(KeySheetNames[i].ToString()));
                sb.AppendLine();
            }

            return sb.ToString();
        }
        public static string GenerateDigraphTable(int year, int month)
        {
            return DigraphTable.Random(year, month).ToString();
        }

        internal static string GenerateKeySheet(string name)
        {
            List<KeySheetEntry> entries = new List<KeySheetEntry>();
            List<string> values = new List<string>();
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("                                     TABLE {0}\r\n", name);
            sb.AppendLine("-------------------------------------------------------------------------------------");

            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    for (int k = 1; k <= 6; k++)
                    {
                        entries.Add(new KeySheetEntry() { Key = string.Format("{0}{1}{2}", i, j, k) });
                    }
                }
            }

            foreach (var item in entries)
            {
                values.Add(GenerateSequence(4, Constants.ALPHABET, false, false));
            }
            values.Sort();

            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Value = values[i];
            }

            int x = 0;

            for (int row = 0; row < 36; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    sb.Append(string.Format("{0}   {1}", entries[x].Key, entries[x].Value));
                    sb.Append("     ");
                    x++;
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }
        internal static string GenerateKeyTable(int year)
        {
            List<KeyTableEntry> entries = new List<KeyTableEntry>();


            for (int m = 1; m <= 12; m++)
            {
                for (int d = 1; d <= DateTime.DaysInMonth(year, m); d++)
                {
                    entries.Add(new KeyTableEntry { Date = new DateTime(year, m, d), SheetName = KeySheetNames[_rand.Next(12)].ToString() });
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("    | JAN | FEB | MAR | APR | MAY | JUN | JUL | AUG | SEP | OCT | NOV | DEC |");
            sb.AppendLine("----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+");

            for (int d = 1; d <= 31; d++)
            {
                sb.Append(d.ToString(" 00"));
                sb.Append(" |");
                for (int m = 1; m <= 12; m++)
                {
                    var item = entries.FirstOrDefault(e => e.Date.Month == m && e.Date.Day == d);
                    if (item != null)
                    {
                        sb.AppendFormat("  {0}  ", item.SheetName);
                    }
                    else
                    {
                        sb.AppendFormat("     ");
                    }
                    sb.Append("|");
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        private class KeyTableEntry
        {
            public DateTime Date { get; set; }
            public string SheetName { get; set; }
        }

        private class KeySheetEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private class Digraph
        {           
            public char PlainTextX { get; set; }            
            public char PlainTextY { get; set; }
            
            public char CipherTextX { get; set; }            
            public char CipherTextY { get; set; }

            public string PlainText { get { return string.Concat(PlainTextX, PlainTextY); } }
            public string CipherText { get { return string.Concat(CipherTextX, CipherTextY); } }
        }

        private class DigraphTable
        {
            private Digraph[,] cells = new Digraph[Constants.ALPHABET.Length, Constants.ALPHABET.Length];
            private List<Digraph> cell_list = new List<Digraph>();

            public DigraphTable()
            {

            }

            public int Year { get; set; }
            public int Month { get; set; }

            public List<Digraph> Cells
            {
                get { return cell_list; }
                set { cell_list = value; }
            }
            public static DigraphTable Random(int year, int month)
            {
                DigraphTable result = new DigraphTable();

                for (int y = 0; y < Constants.ALPHABET.Length; y++)
                {
                    for (int x = 0; x < Constants.ALPHABET.Length; x++)
                    {
                        Digraph newDigraph = new Digraph
                        {
                            PlainTextX = Constants.ALPHABET[x],
                            PlainTextY = Constants.ALPHABET[y]
                        };

                        result.cell_list.Add(newDigraph);
                        result.cells[x, y] = newDigraph;
                    }
                }

                List<Tuple<Digraph, double>> values = new List<Tuple<Digraph, double>>();
                foreach (var item in result.cell_list)
                {
                    values.Add(new Tuple<Digraph, double>(item, RandomUtil._rand.NextDouble()));
                }
                values.Sort((v1, v2) => v1.Item2.CompareTo(v2.Item2));

                while (values.Any())
                {
                    var f = values.First();
                    var l = values.Last();

                    f.Item1.CipherTextX = l.Item1.PlainTextX;
                    f.Item1.CipherTextY = l.Item1.PlainTextY;

                    l.Item1.CipherTextX = f.Item1.PlainTextX;
                    l.Item1.CipherTextY = f.Item1.PlainTextY;

                    values.Remove(f);
                    values.Remove(l);
                }

                return result;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("     ");
                sb.AppendLine(string.Join("  ", Constants.ALPHABET.Select(c => c)));
                sb.AppendLine(new string('-', 81));

                for (int y = 0; y < Constants.ALPHABET.Length; y++)
                {
                    sb.Append(Constants.ALPHABET[y]);
                    sb.Append(" | ");

                    for (int x = 0; x < Constants.ALPHABET.Length; x++)
                    {
                        sb.Append(cells[x, y].CipherText);
                        sb.Append(" ");
                    }
                    sb.Append("\r\n");
                }

                return sb.ToString();
            }

            public string Encrypt(string plainText)
            {
                return cell_list.FirstOrDefault(c => c.PlainText == plainText).CipherText;
            }
            public string Decrypt(string cipherText)
            {
                return cell_list.FirstOrDefault(c => c.CipherText == cipherText).PlainText;
            }
        }
    }
}
