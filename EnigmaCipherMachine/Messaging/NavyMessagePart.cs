using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma;
using Enigma.Configuration;
using Enigma.Enums;

namespace Messaging
{
    public class NavyMessagePart
    {
        public DateTime Timestamp { get; private set; }
        public string StartPosition { get; private set; }
        public string MessageKey { get; private set; }
        public string ActualMessageKey { get; private set; }
        public string EncryptedStartPosition { get; private set; }
        public string EncryptedMessageKey { get; private set; }
        public string CallSign { get; private set; }
        public int Index { get; private set; }
        public int Count { get; private set; }
        public string PlainText { get; private set; }
        public string CipherText { get; private set; }

        private int GroupCount { get; set; }

        internal NavyMessagePart(Settings s, string callSign, string input, int index, int count, string[,] digrams)
        {
            Timestamp = DateTime.Now;
            Index = index + 1;
            Count = count;
            PlainText = input;
            CallSign = callSign;

            int inputLen = input.Length;

            Message m = new Message(s);
            char[,] indic = new char[4, 2];

            if(s.MachineType == Enigma.Enums.MachineType.M3K)
            {
                StartPosition = Utility.GetRandomString(3);
                MessageKey = Utility.GetRandomString(3);
                ActualMessageKey = m.Encrypt(MessageKey, StartPosition);                

                for(int i=0; i<StartPosition.Length;i++)
                {
                    indic[i, 0] = StartPosition[i];
                    indic[i + 1, 1] = MessageKey[i];
                }

                //XYZ_              --> last char is random
                //_ABC              --> first char is random


                indic[3, 0] = Utility.GetRandomCharacter();
                indic[0, 1] = Utility.GetRandomCharacter();
            }
            else
            {
                StartPosition = Utility.GetRandomString(4);
                MessageKey = Utility.GetRandomString(4);
                ActualMessageKey = m.Encrypt(MessageKey, StartPosition);

                for (int i = 0; i < StartPosition.Length; i++)
                {
                    indic[i, 0] = StartPosition[i];
                    indic[i, 1] = MessageKey[i];
                }
            }

            string[] combinedDigrams = new string[4];
            for(int i=0; i<4; i++)
            {
                combinedDigrams[i] = string.Concat(indic[i, 0], indic[i, 1]);
            }

            string[] encryptedDigrams = new string[4];
            for(int i=0; i<4; i++)
            {
                int x = Utility.ALPHA.IndexOf(combinedDigrams[i][0]);
                int y = Utility.ALPHA.IndexOf(combinedDigrams[i][1]);
                encryptedDigrams[i] = digrams[x, y];
            }

            string encryptedIndicators = string.Concat(encryptedDigrams);

            EncryptedStartPosition = encryptedIndicators.Substring(0, 4);
            EncryptedMessageKey = encryptedIndicators.Substring(4);

            string cleanPlaintext = Utility.CleanString(input);
            string rawEncryption = m.Encrypt(cleanPlaintext, ActualMessageKey);
            string cleanRawEncryption = Utility.CleanString(rawEncryption);

            GroupCount = (cleanRawEncryption.Length / 4) + 4;

            string cleanWithIndicators = encryptedIndicators + cleanRawEncryption + encryptedIndicators;

            CipherText = Utility.GetGroups(cleanWithIndicators, 4, 10);

        }

        internal string Header
        {
            get
            {
                return string.Format("{0} {1:HHmm}/{2}/{3}/{4} {5}", CallSign, Timestamp, Timestamp.Day, Index, Count, GroupCount);
            }
        }

        public override string ToString()
        {
            return string.Join("\r\n", Header, CipherText);
        }
    }
}
