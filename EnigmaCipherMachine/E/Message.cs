using Enigma.Configuration;
using Enigma.Enums;
using Enigma.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma
{
    /// <summary>
    /// Represents an encrypted or decrypted message
    /// </summary>
    public class Message
    {
        private Settings _settings;
        private Machine _machine;

        /// <summary>
        /// Constructor to create a new message object by specifying the machine settings
        /// </summary>
        /// <param name="s">The machine settings for the message</param>
        public Message(Settings s)
        {
            s.Validate();

            _settings = s;
            _machine = new Machine(_settings);
        }
        /// <summary>
        /// Constructor to create a new message object using default empty settings
        /// </summary>
        public Message()
            : this(Settings.Empty())
        {

        }

        /// <summary>
        /// The message plain text.
        /// </summary>
        public string PlainText { get; private set; }
        /// <summary>
        /// The message cipher text.
        /// </summary>
        public string CipherText { get; private set; }
        /// <summary>
        /// The current rotor positions (the topmost letter on each rotor appearing in the windows)
        /// </summary>
        public string CurrentRotorPositions
        {
            get
            {
                return _machine.RotorSettings;
            }
        }

        /// <summary>
        /// Encrypts the plaintext using the specified initial rotor settings
        /// Sets the PlainText property to the plainText input value
        /// Sets the CipherText property to the resulting encrypted value
        /// </summary>
        /// <param name="plainText">The plain text message to encrypt</param>
        /// <param name="rotorSettings">The initial rotor settings</param>
        /// <returns>The encrypted text</returns>
        public string Encrypt(string plainText, string rotorSettings, bool extended = false)
        {
            PlainText = plainText;

            ValidateRotorSettings(rotorSettings);

            _machine.RotorSettings = rotorSettings;

            string formatted = extended ? Formatting.ApplyExtendedFormatting(plainText) : Formatting.CleanInput(plainText);

            if (_settings.MachineType == MachineType.M3)
            {
                CipherText = Formatting.OutputGroups(Encipher_Decipher(formatted));
            }
            else
            {
                CipherText = Formatting.OutputGroups(Encipher_Decipher(formatted), 4);
            }

            return CipherText;
        }
        /// <summary>
        /// Decrypts the cipher text using the specified initial rotor settings
        /// Sets the CipherText property to the cipherText input value
        /// Sets the PlainText property to the resulting decrypted value
        /// </summary>
        /// <param name="cipherText">The encrypted text to decrypt</param>
        /// <param name="rotorSettings">The initial rotor settings</param>
        /// <returns>The decrypted text</returns>
        public string Decrypt(string cipherText, string rotorSettings, bool extended = false)
        {
            CipherText = cipherText;

            ValidateRotorSettings(rotorSettings);

            _machine.RotorSettings = rotorSettings;

            string formatted = Encipher_Decipher(Formatting.CleanInput(cipherText));

            PlainText = extended ? Formatting.RemoveExtendedFormatting(formatted) : formatted;

            return PlainText;
        }

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
        private void ValidateRotorSettings(string rotorSettings)
        {
            if (_settings.MachineType == MachineType.M4K)
            {
                if (rotorSettings.Length != 4)
                {
                    throw new ValidationException("Invalid rotor setting count, exactly 4 values are required")
                    {
                        BrokenRules = new List<BrokenRule>
                        {
                            new BrokenRule
                            {
                                Message = "Invalid rotor setting count, 4 values are required",
                                FailureType = ValidationFailureType.InvalidRotorSettings
                            }
                        }
                    };
                }
            }
            else
            {
                if (rotorSettings.Length != 3)
                {
                    throw new ValidationException("Invalid rotor setting count, 3 values are required")
                    {
                        BrokenRules = new List<BrokenRule>
                        {
                            new BrokenRule
                            {
                                Message = "Invalid rotor setting count, exactly 3 values are required",
                                FailureType = ValidationFailureType.InvalidRotorSettings
                            }
                        }
                    };
                }
            }
        }


    }
}

