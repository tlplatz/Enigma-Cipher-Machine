using System;
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
