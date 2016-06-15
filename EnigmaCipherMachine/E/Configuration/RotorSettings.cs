using Enigma.Enums;
using System;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class RotorSetting
    {
        public RotorSetting()
        {
        }
        public RotorSetting(RotorName name, int ring)
        {
            Name = name;
            RingSetting = ring;
        }

        [XmlAttribute]
        public RotorName Name { get; set; }
        [XmlAttribute]
        public int RingSetting { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
