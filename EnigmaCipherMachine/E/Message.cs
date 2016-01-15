using System.Linq;
using System.Text;
using WizardNet.Enigma.Configuration;
using WizardNet.Enigma.Enums;
using WizardNet.Enigma.Util;

namespace WizardNet.Enigma
{
    public class Message
    {
        private Settings _settings;
        private Machine _machine;

        public Message(Settings s)
        {
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
            _machine.RotorSettings = rotorSettings;

            PlainText = Encipher_Decipher(Formatting.CleanInput(cipherText));

            return PlainText;
        }

    }
}
