using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging
{
    public abstract class BaseMessagePart
    {
        public DateTime Timestamp { get; protected set; }
        public string StartPosition { get; protected set; }
        public string MessageKey { get; protected set; }
        public string ActualMessageKey { get; protected set; }

        public int Index { get; protected set; }
        public int Count { get; protected set; }
        public string PlainText { get; protected set; }
        public string CipherText { get; protected set; }

        protected abstract string Header { get; }

        public override string ToString()
        {
            return string.Join("\r\n", Header, CipherText);
        }
    }
}
