using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enigma.Util;
using Messaging;

namespace MessageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //IoUtil.SaveDigraphTable(2016, 9, @"C:\Users\tlplatz.wizardnet.002\Desktop");
            //IoUtil.SaveMonth(@"C:\Users\tlplatz.WIZARDNET.002\Desktop", "Navy_Test", 2016, 9, Enigma.Enums.MachineType.M4K);

            string digrams = @"C:\Users\tlplatz.WIZARDNET.002\Desktop\Digraph_2016Sep.txt";
            string textSettings = @"C:\Users\tlplatz.WIZARDNET.002\Desktop\September_2016_Settings.txt";
            string xmlSettings = @"C:\Users\tlplatz.WIZARDNET.002\Desktop\September_2016_Settings.xml";

            string plainText = @"Before 1940, the German military used the daily key and startposition, according to the key sheet. The operator selected a random message key. This message key was encoded twice, to exclude errors. As example, the trigram GHK is encoded twice, resulting in XMC FZQ. Next, the operator moved the rotors to the message key GHK and encoded the message. The two trigrams, being the encoded message key, were transmitted, together with the message. The receiver sets his machine on the start position, as described in the codebook, and decodes the trigrams XMC FZQ back into the GHK message key. Next, he sets the message key GHK as start position on his machine, to continue decoding the rest of the message. However, this procedure was actually a security flaw. The message key is encoded twice, resulting in a relation between first and fourth, second and fifth, and third and sixth character. Moreover, many message keys on a particular day would have the same setup and startpositions. This security problem enabled the Polish Cipher Bureau to break the pre-war Enigma messages.";

            NavyMessage msg = new NavyMessage(textSettings, digrams, plainText, 17, "XYZ");
            string foo = msg.ToString();

        }
    }
}
