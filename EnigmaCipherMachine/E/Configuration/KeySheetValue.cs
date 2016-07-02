using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class KeySheetValue
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", Key, Value);
        }
    }
}
