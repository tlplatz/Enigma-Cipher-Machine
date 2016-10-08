using System.Collections.Generic;
using Enigma.Configuration;

namespace Messaging
{
    public abstract class BaseMessage
    {
        protected MonthlySettings _monthlySettings;

        public string MonthlySettingsFileName { get; protected set; }
        public string PlainText { get; protected set; }
        public int Day { get; protected set; }

        public IEnumerable<BaseMessagePart> Parts { get; protected set; }
    }
}
