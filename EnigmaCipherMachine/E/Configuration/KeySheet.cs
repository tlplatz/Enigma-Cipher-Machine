using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class KeySheet
    {
        public List<KeySheetValue> Values { get; set; }

        public KeySheet()
        {
            Values = new List<KeySheetValue>();
        }
        internal KeySheet(int groupSize)
        {
            Values = new List<KeySheetValue>();

            int i = 0, j = 0, k = 0, l = 0, m = 0;
            int counter = 0;

            List<string> keyValues = new List<string>();

            i = -1;

            while (counter < Math.Pow(6, 5))
            {
                counter += 1;

                i += 1;

                if (i == 6)
                {
                    i = 0;
                    j += 1;

                    if (j == 6)
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
                Values.Add(new KeySheetValue { Key = string.Format("{0}{1}{2}{3}{4}", i + 1, j + 1, k + 1, l + 1, m + 1) });
            }

            Values.Sort((v1, v2) => v1.Key.CompareTo(v2.Key));

            while (keyValues.Count < Math.Pow(6, 5))
            {
                string temp = Enigma.Util.RandomUtil.GenerateSequence(groupSize, Constants.ALPHABET, false, false);
                if (!keyValues.Contains(temp))
                {
                    keyValues.Add(temp);
                }
            }
            keyValues.Sort((v1, v2) => v1.CompareTo(v2));

            for (int x = 0; x < keyValues.Count; x++)
            {
                Values[x].Value = keyValues[x];
            }
        }

        public string GetKeyValue(string lookupKey)
        {
            return Values.FirstOrDefault(v => v.Key == lookupKey).Value;
        }
        public string GetRandomValue()
        {
            return GetKeyValue(Enigma.Util.RandomUtil.GenerateSequence(5, "123456", false, false));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int steps = (int)(Math.Pow(6.0, 5.0) / 6.0);

            for (int x = 0; x < steps; x++)
            {
                sb.AppendLine(string.Join("      ", Values.Skip(x * 6).Take(6).Select(t => t.ToString())));
                if ((x + 1) % 36 == 0)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public void Save(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(KeySheet));
            using (StreamWriter sw = File.CreateText(fileName))
            {
                ser.Serialize(sw, this);
            }
        }

        public static KeySheet Open(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(KeySheet));
            using (StreamReader rdr = File.OpenText(fileName))
            {
                return (KeySheet)ser.Deserialize(rdr);
            }
        }
        public static KeySheet Random(int groupSize)
        {
            return new KeySheet(groupSize);
        }
    }
}
