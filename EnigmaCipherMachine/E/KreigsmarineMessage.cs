using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Enigma
{
    public class KreigsmarineMessage 
    {
        private KeySheet _sheet;
        private DigraphTable _digs = new DigraphTable();


        /*
            BDU 1540/8/107 24

            BDBJ EMEJ DERH RFRS OQRV DTYH QWBV HILS CXHR OPOD  
            GTQL DDHI KFTG EDZS WXQS EDFR HGYG EDZZ UYQV DTYY
            EDGH KIRM BDBJ EMEJ
        */

        protected Settings _settings;

        public string RecipientId { get; set; }
        public DateTime MessageDate { get; set; }
        public int SerialNumber { get; set; }
        public int GroupCount { get; set; }

        public string Prepend { get; set; }
        public string Append { get; set; }
        public string IndicatorGroup { get; set; }
        public string EncryptedKey { get; set; }
        public string Key { get; set; }

        public KreigsmarineMessage(Settings s, KeySheet sheet, DigraphTable digs)
        {
            _settings = s;
        }

        private string HeaderLine()
        {
            return string.Format("{0} {1:HHmm}/{1:dd}/{2} {3}", RecipientId, MessageDate, SerialNumber, GroupCount);
        }

        //public string Encrypt(string plainText)
        //{
        //    string indicatorGroup = 

        //}
    }
}
