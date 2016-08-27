using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Enigma
{
    public class Message
    {
        private Machine _machine;
        private Settings _settings;

        private string _plain;
        private string _cipher;

        public Message(Settings s)
        {
            _settings = s;
            _machine = new Machine(_settings);

            _settings.Validate();
        }

        public string PlainText { get { return _plain; } }
        public string CipherText { get { return _cipher; } }

        public string Encrypt(string plainText, string settings)
        {
            if(_settings.MachineType == Enums.MachineType.M4K)
            {
                if (settings.Length != 4)
                {
                    throw new ValidationException("M4 Enigma requires settings for 4 rotors");
                }               
            }
            else
            {
                if (settings.Length != 3)
                {
                    throw new ValidationException("M3 Enigma requires settings for 3 rotors");
                }
            }

            _plain = plainText;

            string clean = Util.Formatting.CleanInput(plainText);

            _machine.RotorSettings = settings;

            StringBuilder sb = new StringBuilder();
            foreach(char c in clean)
            {
                sb.Append(_machine.Encipher(c.ToString()));
            }

            if (_settings.MachineType == Enums.MachineType.M3)
            {
                _cipher = Util.Formatting.OutputGroups(sb.ToString());
            }
            else
            {
                _cipher = Util.Formatting.OutputGroups(sb.ToString(), 4);
            }

            return CipherText;
        }
        public string Decrypt(string cipherText, string settings)
        {
            if (_settings.MachineType == Enums.MachineType.M4K)
            {
                if (settings.Length != 4)
                {
                    throw new ValidationException("M4 Enigma requires settings for 4 rotors");
                }
            }
            else
            {
                if (settings.Length != 3)
                {
                    throw new ValidationException("M3 Enigma requires settings for 3 rotors");
                }
            }

            _cipher = cipherText;

            string clean = Util.Formatting.CleanInput(cipherText);

            _machine.RotorSettings = settings;


            StringBuilder sb = new StringBuilder();
            foreach (char c in clean)
            {
                sb.Append(_machine.Encipher(c.ToString()));
            }

            _plain = sb.ToString();

            return PlainText;
        }

        public string CurrentRotorPositions
        {
            get
            {
                return _machine.RotorSettings;
            }
        }
    }
}
