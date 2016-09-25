using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    internal static class Utility
    {
        public const string ALPHA = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static Random _rand = new Random();

        public static char GetRandomCharacter()
        {
            return ALPHA[_rand.Next(ALPHA.Length)];
        }
        public static string GetRandomString(int length)
        {
            char[] result = new char[length];
            for(int i=0; i<length; i++)
            {
                result[i] = GetRandomCharacter();
            }
            return string.Concat(result);
        }
        public static string CleanString(string input)
        {
            return string.Concat(input.ToUpper().Where(c => ALPHA.Contains(c)));
        }
        public static string[] GetMessageParts(string input, int groupSize, int maxGroupsPerPart)
        {
            int maxSize = groupSize * maxGroupsPerPart;
            int count = input.Length / maxSize;

            if (input.Length % maxSize != 0) count += 1;

            string[] result = new string[count];

            for(int i=0; i<count; i++)
            {
                if (i != count - 1)
                {
                    result[i] = input.Substring(i * maxSize, maxSize);
                }
                else
                {
                    result[i] = input.Substring(i * maxSize);
                }
            }

            return result;
        }
        public static int GetPaddingCount(string input, int groupSize)
        {
            if (input.Length % groupSize == 0) return 0;
            return groupSize - (input.Length % groupSize);
        }
        public static string GetPaddedString(string input, int groupSize)
        {
            int pads = GetPaddingCount(input, groupSize);
            return string.Concat(input, new string('X', pads));
        }
        public static string GetGroups(string input, int groupSize, int perRow)
        {
            StringBuilder sb = new StringBuilder();

            int groupCounter = 0;
            int rowCounter = 0;

            foreach(char c  in input)
            {
                sb.Append(c);

                groupCounter++;

                if(groupCounter == groupSize)
                {
                    sb.Append("\t");
                    groupCounter = 0;

                    rowCounter++;

                    if(rowCounter == perRow)
                    {
                        sb.Append("\r\n");
                        rowCounter = 0;
                    }
                }
            }

            return sb.ToString();
        }

        public static string[,] ParseDigraphTable(string fileName)
        {
            string[,] digrams = new string[26, 26];

            using (StreamReader rdr = File.OpenText(fileName))
            {
                string content = rdr.ReadToEnd();

                string[] lines = content.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] alphaRows = lines.Skip(2).Take(26).ToArray();

                for (int row = 0; row < 26; row++)
                {
                    string[] tokens = alphaRows[row].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    string[] cells = tokens.Skip(2).Take(26).ToArray();

                    for (int col = 0; col < 26; col++)
                    {
                        digrams[col, row] = cells[col];
                    }
                }
            }
            return digrams;
        }
    }
}
