﻿using System;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class PlugSetting
    {
        public PlugSetting() { }
        public PlugSetting(string a, string b)
        {
            LetterA = a;
            LetterB = b;
        }
        public PlugSetting(string s)
            : this(s[0].ToString(), s[1].ToString())
        {
        }

        [XmlAttribute]
        public string LetterA { get; set; }
        [XmlAttribute]
        public string LetterB { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", LetterA, LetterB);
        }
    }
}
