using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class Digraph
    {
        [XmlAttribute]
        public char PlainTextX { get; set; }
        [XmlAttribute]
        public char PlainTextY { get; set; }

        [XmlAttribute]
        public char CipherTextX { get; set; }
        [XmlAttribute]
        public char CipherTextY { get; set; }

        [XmlIgnore]
        public string PlainText { get { return string.Concat(PlainTextX, PlainTextY); } }
        [XmlIgnore]
        public string CipherText { get { return string.Concat(CipherTextX, CipherTextY); } }

    }
}
