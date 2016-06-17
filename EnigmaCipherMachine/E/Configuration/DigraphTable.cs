using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Enigma.Util;

namespace Enigma.Configuration
{
    [Serializable]
    public class DigraphTable
    {
        private Digraph[,] cells = new Digraph[Constants.ALPHABET.Length, Constants.ALPHABET.Length];
        private List<Digraph> cell_list = new List<Digraph>();

        public DigraphTable()
        {

        }

        [XmlAttribute]
        public int Year { get; set; }
        [XmlAttribute]
        public int Month { get; set; }

        public List<Digraph> Cells
        {
            get { return cell_list; }
            set { cell_list = value; }
        }

        public void Save(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DigraphTable));
            using (FileStream stm = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ser.Serialize(stm, this);
            }
        }

        public static DigraphTable Open(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DigraphTable));
            using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return (DigraphTable)ser.Deserialize(stm);
            }
        }
        public static DigraphTable Random(int year , int month)
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
            foreach(var item in result.cell_list)
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
    }
}
