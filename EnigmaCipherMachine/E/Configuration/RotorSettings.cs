﻿using Enigma.Enums;
using System;

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

        public RotorName Name { get; set; }
        public int RingSetting { get; set; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
