using System;

namespace Enigma.PlugBoard
{
    public class Plug : IComparable<Plug>
    {
        public Plug(string a, string b)
        {
            LetterA = a;
            LetterB = b;
        }

        public string LetterA { get; set; }
        public string LetterB { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", LetterA, LetterB);
        }

        public int CompareTo(Plug other)
        {
            if (Object.ReferenceEquals(this, other)) return 0;
            return this.ToString().CompareTo(other.ToString());
        }
    }
}
