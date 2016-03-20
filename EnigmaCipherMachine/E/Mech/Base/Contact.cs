
namespace Enigma.Base
{
    internal class Contact
    {
        public int WireRight { get; set; }
        public int WireLeft { get; set; }
        public bool Notch { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}", WireLeft, WireRight);
        }
    }
}
