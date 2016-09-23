using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma;
using Enigma.Configuration;

namespace Messaging
{
    public class ArmyMessagePart
    {
        public DateTime Timestamp { get; private set; }
        public string StartPosition { get; private set; }
        public string MessageKey { get; private set; }
        public string ActualMessageKey { get; private set; }
        public string Kenngruppen { get; private set; }
        public int Index { get; private set; }
        public int Count { get; private set; }
        public string PlainText { get; private set; }
        public string CipherText { get; private set; }

        private int CharacterCount { get; set; }

        internal ArmyMessagePart(Settings s, string input, int index, int count)
        {
            Timestamp = DateTime.Now;
            StartPosition = Utility.GetRandomString(3);
            MessageKey = Utility.GetRandomString(3);
            Index = index + 1;
            Count = count;
            
            Message m = new Message(s);
            ActualMessageKey = m.Encrypt(MessageKey, StartPosition);

            string k = s.Kenngruppen[Utility._rand.Next(s.Kenngruppen.Count)];
            bool before = Utility._rand.Next(2) == 1;

            if (before)
            {
                Kenngruppen = string.Concat(Utility.GetRandomString(2), k);
            }
            else
            {
                Kenngruppen = string.Concat(k, Utility.GetRandomString(2));
            }

            PlainText = input;

            string cleanInput = Utility.CleanString(input);
            string encrypted = m.Encrypt(cleanInput, ActualMessageKey);
            string rawEncrypted = Kenngruppen + Utility.CleanString(encrypted);

            CipherText = Utility.GetGroups(rawEncrypted, 5, 5);

            CharacterCount = rawEncrypted.Length;
        }

        internal string Header
        {
            get
            {
                return string.Format("{0:HHmm} = {1}tle = {2}tl = {3} = {4} {5} =",
                    Timestamp, Count, Index, CharacterCount, StartPosition, MessageKey);
            }
        }

        public override string ToString()
        {
            return string.Join("\r\n", Header, CipherText);
        }
    }
}
