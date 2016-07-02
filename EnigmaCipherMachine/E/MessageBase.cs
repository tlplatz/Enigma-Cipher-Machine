using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Configuration;

namespace Enigma
{
    public class Message : MessageBase
    {
        public Message(Settings s) : base(s) { }
        public Message() : base() { }
    }
}
