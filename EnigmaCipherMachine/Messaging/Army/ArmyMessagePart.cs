using System;
using Enigma;
using Enigma.Configuration;
using Messaging.Util;

namespace Messaging.Army
{
    public class ArmyMessagePart : BaseMessagePart
    {
        public string Kenngruppen { get; private set; }

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

            PlainText = input;
            Kenngruppen = GetKenngruppen(s);

            string cleanInput = Utility.CleanString(input);
            string encrypted = m.Encrypt(cleanInput, ActualMessageKey);
            string rawEncrypted = Kenngruppen + Utility.CleanString(encrypted);

            CipherText = Utility.GetGroups(rawEncrypted, 5, 5);

            CharacterCount = rawEncrypted.Length;
        }

        private string GetKenngruppen(Settings s)
        {
            string k = s.Kenngruppen[Utility._rand.Next(s.Kenngruppen.Count)];
            bool before = Utility._rand.Next(2) == 1;

            if (before)
            {
                return string.Concat(Utility.GetRandomString(2), k);
            }
            else
            {
                return string.Concat(k, Utility.GetRandomString(2));
            }
        }

        protected override string Header
        {
            get
            {
                return string.Format("{0:HHmm} = {1}tle = {2}tl = {3} = {4} {5} =",
                    Timestamp, Count, Index, CharacterCount, StartPosition, MessageKey);
            }
        }
    }
}
