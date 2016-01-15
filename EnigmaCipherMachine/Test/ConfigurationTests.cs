using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WizardNet.Enigma.Configuration;
using WizardNet.Enigma.Enums;
using WizardNet.Enigma.UKW;
using WizardNet.Enigma.Rotors;
using System.IO;
using WizardNet.Enigma;

namespace Test
{
    [TestFixture]
    public class ConfigurationTests
    {
        public const string PLAIN_TEXT = "The quick brown fox jumps over the lazy dog x";
        public const string DECRYPTED_TEXT = "THEQUICKBROWNFOXJUMPSOVERTHELAZYDOGX";

        public const string EXPECTED_M4 = "ZBHG  VBNW  OZZZ  FXGZ  ITXN  \r\nBQWG  QGLQ  FPQD  CTCP";
        public const string EXPECTED_M3 = "EMMUM  GDOSC  VKCUU  QDQVE  KJTYX  \r\nODITX  RVCHM  G";
        public const string EXPECTED_M3K = "IVNY  XQID  IJER  JJBS  PXXG  \r\nKSAZ  XQAZ  BGYD  CNEZ";

        [Test]
        public void TestAAA1()
        {
            Settings s = new Settings(MachineType.M3, ReflectorType.B);

            s.Rotors.Add(new RotorSetting(RotorName.I, 0));
            s.Rotors.Add(new RotorSetting(RotorName.II, 0));
            s.Rotors.Add(new RotorSetting(RotorName.III, 0));

            Machine m = new Machine(s);

            m.RotorSettings = "AAA";

            Assert.AreEqual(m.Encipher("A"), "B");
            Assert.AreEqual(m.Encipher("A"), "D");
            Assert.AreEqual(m.Encipher("A"), "Z");
            Assert.AreEqual(m.Encipher("A"), "G");
            Assert.AreEqual(m.Encipher("A"), "O");

            //BDZGO
        }

        [Test]
        public void TestAAA2()
        {
            Settings s = new Settings(MachineType.M3, ReflectorType.B);

            s.Rotors.Add(new RotorSetting(RotorName.I, 1));
            s.Rotors.Add(new RotorSetting(RotorName.II, 1));
            s.Rotors.Add(new RotorSetting(RotorName.III, 1));

            Machine m = new Machine(s);

            m.RotorSettings = "AAA";

            Assert.AreEqual(m.Encipher("A"), "E");
            Assert.AreEqual(m.Encipher("A"), "W");
            Assert.AreEqual(m.Encipher("A"), "T");
            Assert.AreEqual(m.Encipher("A"), "Y");
            Assert.AreEqual(m.Encipher("A"), "X");

            //EWTYX
        }

        [Test]
        public void TestM3MessageOutput()
        {
            /*
                M3
                LOT
                 00 | C | I      III    IV            | 19 06 24     | CQ DV ES FO GT IU JR KM LY NW
                EMMUM  GDOSC  VKCUU  QDQVE  KJTYX
                ODITX  RVCHM  G
                THEQUICKBROWNFOXJUMPSOVERTHELAZYDOGX
            */

            Settings s = new Settings(MachineType.M3, ReflectorType.C);

            s.Rotors.Add(new RotorSetting(RotorName.I, 18));
            s.Rotors.Add(new RotorSetting(RotorName.III, 5));
            s.Rotors.Add(new RotorSetting(RotorName.IV, 23));

            s.Plugs.Add(new PlugSetting("CQ"));
            s.Plugs.Add(new PlugSetting("DV"));
            s.Plugs.Add(new PlugSetting("ES"));
            s.Plugs.Add(new PlugSetting("FO"));
            s.Plugs.Add(new PlugSetting("GT"));
            s.Plugs.Add(new PlugSetting("IU"));
            s.Plugs.Add(new PlugSetting("JR"));
            s.Plugs.Add(new PlugSetting("KM"));
            s.Plugs.Add(new PlugSetting("LY"));
            s.Plugs.Add(new PlugSetting("NW"));

            Message msg = new Message(s);

            string encipher = msg.Encrypt(PLAIN_TEXT, "LOT");
            Assert.AreEqual(encipher, EXPECTED_M3);
            string decipher = msg.Decrypt(encipher, "LOT");
            Assert.AreEqual(decipher, DECRYPTED_TEXT);
        }

        [Test]
        public void TestM3KMessageOutput()
        {
            /*
                M3K
                LOT
                 00 | B | VIII   VII    II            | 12 12 13     | AG BI CU ES FT HZ JP KO MX RW
                IVNY  XQID  IJER  JJBS  PXXG
                KSAZ  XQAZ  BGYD  CNEZ
                THEQUICKBROWNFOXJUMPSOVERTHELAZYDOGX
            */

            Settings s = new Settings(MachineType.M3K, ReflectorType.B);

            s.Rotors.Add(new RotorSetting(RotorName.VIII, 11));
            s.Rotors.Add(new RotorSetting(RotorName.VII, 11));
            s.Rotors.Add(new RotorSetting(RotorName.II, 12));

            s.Plugs.Add(new PlugSetting("AG"));
            s.Plugs.Add(new PlugSetting("BI"));
            s.Plugs.Add(new PlugSetting("CU"));
            s.Plugs.Add(new PlugSetting("ES"));
            s.Plugs.Add(new PlugSetting("FT"));
            s.Plugs.Add(new PlugSetting("HZ"));
            s.Plugs.Add(new PlugSetting("JP"));
            s.Plugs.Add(new PlugSetting("KO"));
            s.Plugs.Add(new PlugSetting("MX"));
            s.Plugs.Add(new PlugSetting("RW"));

            Message msg = new Message(s);

            string encipher = msg.Encrypt(PLAIN_TEXT, "LOT");
            Assert.AreEqual(encipher, EXPECTED_M3K);
            string decipher = msg.Decrypt(encipher, "LOT");
            Assert.AreEqual(decipher, DECRYPTED_TEXT);
        }

        [Test]
        public void TestM4MessageOutput()
        {
            /*
                M4
                BLOT
                 00 | B | Beta   II     V      I      | 08 20 19 09  | BI CQ DK EU FY JS LT NP RZ VX
                THe quick brown fox jumps over the lazy dog x
                ZBHG  VBNW  OZZZ  FXGZ  ITXN
                BQWG  QGLQ  FPQD  CTCP
                THEQUICKBROWNFOXJUMPSOVERTHELAZYDOGX
            */

            Settings s = new Settings(MachineType.M4K, ReflectorType.B_Dunn);

            s.Rotors.Add(new RotorSetting(RotorName.Beta, 7));
            s.Rotors.Add(new RotorSetting(RotorName.II, 19));
            s.Rotors.Add(new RotorSetting(RotorName.V, 18));
            s.Rotors.Add(new RotorSetting(RotorName.I, 8));

            s.Plugs.Add(new PlugSetting("BI"));
            s.Plugs.Add(new PlugSetting("CQ"));
            s.Plugs.Add(new PlugSetting("DK"));
            s.Plugs.Add(new PlugSetting("EU"));
            s.Plugs.Add(new PlugSetting("FY"));
            s.Plugs.Add(new PlugSetting("JS"));
            s.Plugs.Add(new PlugSetting("LT"));
            s.Plugs.Add(new PlugSetting("NP"));
            s.Plugs.Add(new PlugSetting("RZ"));
            s.Plugs.Add(new PlugSetting("VX"));

            Message msg = new Message(s);

            string encipher = msg.Encrypt(PLAIN_TEXT, "BLOT");
            Assert.AreEqual(encipher, EXPECTED_M4);
            string decipher = msg.Decrypt(encipher, "BLOT");
            Assert.AreEqual(decipher, DECRYPTED_TEXT);
        }

        [Test]
        public void TestEmptySettingsHaveExpectedValues()
        {
            Settings s = Settings.Empty();

            Assert.AreEqual(s.MachineType, MachineType.M3);
            Assert.AreEqual(s.ReflectorType, ReflectorType.B);

            Assert.AreEqual(s.Rotors.Count, 3);

            Assert.AreEqual(s.Rotors[0].Name, RotorName.I);
            Assert.AreEqual(s.Rotors[1].Name, RotorName.II);
            Assert.AreEqual(s.Rotors[2].Name, RotorName.III);

            Assert.AreEqual(s.Rotors[0].RingSetting, 0);
            Assert.AreEqual(s.Rotors[1].RingSetting, 0);
            Assert.AreEqual(s.Rotors[2].RingSetting, 0);

            Assert.AreEqual(s.Plugs.Count, 0);
        }

        [Test]
        public void TestRandomSettingsContainValidReflector()
        {
            Settings s = Settings.Random(MachineType.M3);

            ReflectorList reflectors = new ReflectorList(s.MachineType);
            Assert.IsTrue(reflectors.Select(r => r.ReflectorType).Contains(s.ReflectorType));
        }

        [Test]
        public void TestRandomSettingsContainValidRotors()
        {
            Settings s = Settings.Random(MachineType.M3);

            RotorList rotors = new RotorList(MachineType.M3);
            Assert.AreEqual(rotors.Select(r => r.RotorName).Intersect(s.Rotors.Select(r => r.Name)).Count(), s.Rotors.Count);

        }

        [Test]
        public void TestRandomSettingsAreValid()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> brokenRules;

            Assert.AreEqual(true, Validation.Validate(s, out brokenRules));
        }

        [Test]
        public void TestInvalidReflectorIsInvalid()
        {
            Settings s = Settings.Empty();
            s.ReflectorType = ReflectorType.B_Dunn;
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validation.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorTypeForMachine).Any());
        }

        [Test]
        public void TestInvalidRotorCountIsInvalid()
        {
            Settings s = Settings.Empty();
            s.Rotors.RemoveAt(0);
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validation.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorCount).Any());
        }

        [Test]
        public void TestInvalidRotorIsInvalid()
        {
            Settings s = Settings.Empty();
            s.Rotors[0].Name = RotorName.Gamma;
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validation.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorTypeForMachine).Any());
        }

        [Test]
        public void TestInvalidRotorRingSettingIsInvalid()
        {
            Settings s = Settings.Empty();

            s.Rotors[0].RingSetting = -1;
            List<BrokenRule> rules;

            Assert.AreEqual(false, Validation.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.RingSettingOutOfRange).Any());
        }

        [Test]
        public void TestMissingThinRotor()
        {
            Settings s = Settings.Empty();
            s.MachineType = MachineType.M4K;
            List<BrokenRule> rules = new List<BrokenRule>();

            Assert.AreEqual(false, Validation.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.ThinRotorMissing).Any());
        }

        [Test]
        public void TestTooManyPlugs()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });

            Assert.AreEqual(false, Validation.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.TooManyPlugs).Any());
        }

        [Test]
        public void TestNonUniquePlugLinks()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "A" });

            Assert.AreEqual(false, Validation.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.PlugsLinksNotUnique).Any());
        }

        [Test]
        public void TestDuplicatePlugs()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });

            Assert.AreEqual(false, Validation.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.DuplicatePlugs).Any());
        }

        [Test]
        public void TestSerializationAndSave()
        {
            string folderName = Path.GetTempPath();
            string fileName = Path.GetTempFileName();

            Settings s = Settings.Random(MachineType.M3);

            string filePath = Path.Combine(folderName, fileName);
            s.Save(filePath);
            string foo = s.ToString();

            s = Settings.Open(filePath);
            Assert.AreEqual(s.ToString(), foo);

            File.Delete(filePath);
        }

        [Test]
        public void TestThatMonthlySettingsHaveCorrectNumberOfDays()
        {
            int days = DateTime.DaysInMonth(2015, 12);
            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn);
            Assert.AreEqual(ms.DailySettings.Count, days);
        }

        [Test]
        public void TestMonthlyRotorSettingsInCompatibilityMode()
        {
            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn, true);
            Assert.AreEqual(true, ms.DailySettings.All(s => s.Rotors[0].Name == RotorName.Beta && s.Rotors[0].RingSetting == 0));
        }

        [Test]
        public void TestMonthlySettingsSerializationAndSave()
        {
            string folderName = Path.GetTempPath();
            string fileName = Path.GetTempFileName();

            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn, false);

            string filePath = Path.Combine(folderName, fileName);
            ms.Save(filePath);
            string foo = ms.ToString();

            ms = MonthlySettings.Open(filePath);
            Assert.AreEqual(ms.ToString(), foo);

            File.Delete(filePath);
        }
    }
}
