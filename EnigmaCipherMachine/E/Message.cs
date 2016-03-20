using Enigma.Configuration;
using Enigma.Enums;
using Enigma.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma
{
    public class Message
    {
        private Settings _settings;
        private Machine _machine;

        public Message(Settings s)
        {
            List<BrokenRule> brokenRules = new List<BrokenRule>();

            if (!Validation.Validate(s, out brokenRules))
            {
                throw new ValidationException(string.Join("\r\n", brokenRules.Select(b => b.Message)));
            }

            _settings = s;
            _machine = new Machine(_settings);
        }
        public Message()
            : this(Settings.Empty())
        {

        }

        public string PlainText { get; private set; }
        public string CipherText { get; private set; }

        private string Encipher_Decipher(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input.ToUpper())
            {
                if (Constants.ALPHABET.Contains(c))
                {
                    sb.Append(_machine.Encipher(c.ToString()));
                }
            }
            return sb.ToString();
        }

        public string Encrypt(string plainText, string rotorSettings)
        {
            if(_settings.MachineType == MachineType.M4K)
            {
                if (rotorSettings.Length != 4)
                {
                    throw new ValidationException("Invalid rotor setting count, 4 values are required");
                }
            }
            else
            {
                if (rotorSettings.Length != 3)
                {
                    throw new ValidationException("Invalid rotor setting count, 3 values are required");
                }
            }

            _machine.RotorSettings = rotorSettings;

            if (_settings.MachineType == MachineType.M3)
            {
                CipherText = Formatting.OutputGroups(Encipher_Decipher(Formatting.CleanInput(plainText)));
            }
            else
            {
                CipherText = Formatting.OutputGroups(Encipher_Decipher(Formatting.CleanInput(plainText)), 4);
            }

            return CipherText;
        }
        public string Decrypt(string cipherText, string rotorSettings)
        {
            if (_settings.MachineType == MachineType.M4K)
            {
                if (rotorSettings.Length != 4)
                {
                    throw new ValidationException("Invalid rotor setting count, 4 values are required");
                }
            }
            else
            {
                if (rotorSettings.Length != 3)
                {
                    throw new ValidationException("Invalid rotor setting count, 3 values are required");
                }
            }

            _machine.RotorSettings = rotorSettings;

            PlainText = Encipher_Decipher(Formatting.CleanInput(cipherText));

            return PlainText;
        }

        public string RotorPositions
        {
            get
            {
                return _machine.RotorSettings;
            }
        }
    }
}

