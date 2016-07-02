using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Enigma
{
    public class WehrmachtMessagePart : MessageBase
    {
        /*
         * 
         * 
             1230 = 3tle = 1tl = 250 = WZA UHL =   

             FDJKM LDAHH YEOEF PTWYB LENDP
             MKOXL DFAMU DWIJD XRJZY DFRIO
             MFTEV KTGUY DDZED TPOQX FDRIU
             CCBFM MQWYE FIPUL WSXHG YHJZE
             AOFDU FUTEC VVBDP OLZLG DEJTI
             HGYER DCXCV BHSEE TTKJK XAAQU
             GTTUO FCXZH IDREF TGHSZ DERFG
             EDZZS ERDET RFGTT RREOM MJMED
             EDDER FTGRE UUHKD DLEFG FGREZ
             ZZSEU YYRGD EDFED HJUIK FXNVB
        */

        public WehrmachtMessagePart() : base()
        {

        }
        public WehrmachtMessagePart(Settings s) : base(s)
        {

        }

        public WehrmachtMessage ParentMessage { get; set; }
        public int TotalParts { get; set; }
        public int Index { get; set; }
        public int LetterCount { get; set; }
        public string StartPosition { get; set; }
        public string Key { get; set; }
        public string EncryptedKey { get; set; }
        public string Indicator { get; set; }
        public string IndicatorPrepend { get; set; }
        public string IndicatorAppend { get; set; }
        public string RawMessage { get; set; }

        public override string Decrypt(string cipherText, string rotorSettings)
        {
            return base.Decrypt(cipherText, rotorSettings);
        }
        public override string Encrypt(string plainText, string rotorSettings)
        {
            PlainText = plainText;

            _machine.RotorSettings = StartPosition;
            EncryptedKey = base.Encipher_Decipher(rotorSettings);
            Key = rotorSettings;

            _machine.RotorSettings = rotorSettings;
            string rawMessage = base.Encipher_Decipher(plainText);

            string indicatorGroup = IndicatorPrepend + Indicator + IndicatorAppend;

            LetterCount = rawMessage.Length + 5;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(HeaderLine());
            sb.AppendLine(Enigma.Util.Formatting.OutputGroups(indicatorGroup + rawMessage, 5, 5, "  "));

            CipherText = sb.ToString();
            return CipherText;
        }

        private string HeaderLine()
        {
            return string.Format("{0:HHmm} = {1}tle = {2}tl = {3} = {4} {5} =",
                ParentMessage.Time,
                TotalParts,
                Index,
                LetterCount,
                StartPosition,
                EncryptedKey);
        }
    }
}
