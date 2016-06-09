using System.Collections.Generic;
using System.Linq;

namespace Enigma
{
    internal abstract class FixedRotor
    {
        protected List<Contact> _contacts;

        protected FixedRotor(string name, string init)
        {
            Name = name;

            _contacts = new List<Contact>();

            for (int i = 0; i < init.Length; i++)
            {
                _contacts.Add(new Contact { WireRight = i, WireLeft = Constants.ALPHABET.IndexOf(init[i]) });
            }
        }
        protected FixedRotor(string name)
            : this(name, Constants.ALPHABET)
        {

        }

        public string Name { get; private set; }

        public virtual int WireLeft(int wireRight)
        {
            return _contacts[wireRight].WireLeft;
        }
        public virtual int WireRight(int wireLeft)
        {
            return _contacts.FirstOrDefault(c => c.WireLeft == wireLeft).WireRight;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Initializer
        {
            get
            {
                return string.Concat(_contacts.Select(c => Constants.ALPHABET[c.WireLeft]));
            }
        }
    }
}
