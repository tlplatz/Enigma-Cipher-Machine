using Enigma.Base;
using Enigma.Enums;
using System.Linq;

namespace Enigma.Rotors
{
    public class Rotor : FixedRotor
    {
        public Rotor(RotorName name, params int[] notches)
            : base(name.ToString(), Constants.Rotors[(int)name])
        {
            RotorName = name;

            foreach (int i in notches)
            {
                _contacts[i].Notch = true;
            }
        }

        public RotorName RotorName { get; private set; }
        public int Position { get; private set; }

        public string Setting
        {
            get { return Constants.ALPHABET[Position].ToString(); }
            set { Position = Constants.ALPHABET.IndexOf(value); }
        }
        public int RingSetting { get; set; }

        public void Advance()
        {
            Position += 1;
            if (Position >= Constants.ALPHABET.Length) Position -= Constants.ALPHABET.Length;
        }

        public override int WireLeft(int wireRight)
        {
            int offset = Position - RingSetting;
            if (offset < 0) offset += Constants.ALPHABET.Length;

            int index = wireRight + offset;
            if (index >= Constants.ALPHABET.Length) index -= Constants.ALPHABET.Length;

            index = _contacts[index].WireLeft;

            index -= offset;
            if (index < 0) index += Constants.ALPHABET.Length;
            return index;
        }
        public override int WireRight(int wireLeft)
        {
            int offset = Position - RingSetting;
            if (offset < 0) offset += Constants.ALPHABET.Length;

            int index = wireLeft + offset;
            if (index >= Constants.ALPHABET.Length) index -= Constants.ALPHABET.Length;

            index = _contacts.FirstOrDefault(c => c.WireLeft == index).WireRight;

            index -= offset;
            if (index < 0) index += Constants.ALPHABET.Length;
            return index;
        }

        public bool OnNotch
        {
            get { return _contacts[Position].Notch; }
        }
    }
}
