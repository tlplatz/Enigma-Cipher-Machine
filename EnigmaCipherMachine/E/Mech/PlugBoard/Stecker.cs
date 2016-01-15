using System;
using System.Collections.Generic;
using System.Linq;
using WizardNet.Enigma.Base;


namespace WizardNet.Enigma.PlugBoard
{
    public class Stecker : FixedRotor
    {
        public Stecker()
            : base("Stecker", Constants.ALPHABET)
        {
            Plugs = new List<Plug>();
        }

        public List<Plug> Plugs { get; private set; }
        public string PlugSettings
        {
            get
            {
                return string.Join("  ", Plugs.Select(p => p.ToString()).OrderBy(p => p));
            }
            set
            {
                Plugs.Clear();

                string[] tokens = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in tokens)
                {
                    string a = s[0].ToString();
                    string b = s[1].ToString();

                    if (a != b)
                    {
                        if (GetPlug(a, b) == null && GetPlug(b, a) == null)
                        {
                            Plugs.Add(new Plug(a, b));
                        }
                    }
                }

                foreach (Contact c in _contacts)
                {
                    c.WireLeft = c.WireRight;
                }

                foreach (Plug p in Plugs)
                {
                    _contacts[Constants.ALPHABET.IndexOf(p.LetterA)].WireLeft = Constants.ALPHABET.IndexOf(p.LetterB);
                    _contacts[Constants.ALPHABET.IndexOf(p.LetterB)].WireLeft = Constants.ALPHABET.IndexOf(p.LetterA);
                }
            }
        }

        private Plug GetPlug(string a, string b)
        {
            return Plugs.FirstOrDefault(p => p.LetterA == a && p.LetterB == b);
        }
    }

}
