using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Enigma;
using Enigma.Configuration;
using Enigma.Enums;
using Enigma.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConvertedTests
{
    [TestClass]
    public class ConfigurationTests
    {
        public const string PLAIN_TEXT = "The quick brown fox jumps over the lazy dog x";
        public const string DECRYPTED_TEXT = "THEQUICKBROWNFOXJUMPSOVERTHELAZYDOGX";

        public const string EXPECTED_M4 = "ZBHG  VBNW  OZZZ  FXGZ  ITXN\r\nBQWG  QGLQ  FPQD  CTCP";
        public const string EXPECTED_M3 = "EMMUM  GDOSC  VKCUU  QDQVE  KJTYX\r\nODITX  RVCHM  G";
        public const string EXPECTED_M3K = "IVNY  XQID  IJER  JJBS  PXXG\r\nKSAZ  XQAZ  BGYD  CNEZ";

        public const string M3_SETTING_LINE = " 00 | C | I      III    IV            | 19 06 24     | CQ DV ES FO GT IU JR KM LY NW";
        public const string M3K_SETTING_LINE = " 00 | B | VIII   VII    II            | 12 12 13     | AG BI CU ES FT HZ JP KO MX RW";
        public const string M4K_SETTING_LINE = " 00 | B | Beta   II     V      I      | 08 20 19 09  | BI CQ DK EU FY JS LT NP RZ VX";

        public const string LONG_TEST_PLAIN_TEXT = @"or shame deny that thou bear'st love to any,
Who for thy self art so unprovident.
Grant, if thou wilt, thou art beloved of many,
But that thou none lov'st is most evident:
For thou art so possessed with murderous hate,
That 'gainst thy self thou stick'st not to conspire,
Seeking that beauteous roof to ruinate
Which to repair should be thy chief desire.
O! change thy thought, that I may change my mind:
Shall hate be fairer lodged than gentle love?
Be, as thy presence is, gracious and kind,
Or to thyself at least kind-hearted prove:
   Make thee another self for love of me,
   That beauty still may live in thine or thee.";


        [TestMethod]
        public void SettingsConstructorWithFullSettingsMatchesExpectedValue()
        {
            Enigma.Configuration.Settings s = new Settings(
                MachineType.M4K,
                ReflectorType.B,
                new RotorName[] { RotorName.Beta, RotorName.II, RotorName.V, RotorName.I },
                new int[] { 7, 19, 18, 8 },
                new string[] { "BI", "CQ", "DK", "EU", "FY", "JS", "LT", "NP", "RZ", "VX" });

            Enigma.Configuration.Settings s2 = Enigma.Configuration.Settings.ParseSettingLine(M4K_SETTING_LINE);

            Assert.AreEqual(s.ToString(), s2.ToString());
        }

        [TestMethod]
        public void SettingsConstructorWithParamArrayArgsMatchesExpectedValue()
        {
            Enigma.Configuration.Settings s = new Settings(
                MachineType.M4K,
                ReflectorType.B,
                RotorName.Beta, 7, RotorName.II, 19, RotorName.V, 18, RotorName.I, 8,
                "BI", "CQ", "DK", "EU", "FY", "JS", "LT", "NP", "RZ", "VX");

            Enigma.Configuration.Settings s2 = Enigma.Configuration.Settings.ParseSettingLine(M4K_SETTING_LINE);

            Assert.AreEqual(s.ToString(), s2.ToString());
        }

        [TestMethod]
        public void AAA1()
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

            m.SteckerSettings = "BQ  DU  ZI  GC  OK";
            m.RotorSettings = "AAA";

            Assert.AreEqual(m.Encipher("A"), "Q");
            Assert.AreEqual(m.Encipher("A"), "U");
            Assert.AreEqual(m.Encipher("A"), "I");
            Assert.AreEqual(m.Encipher("A"), "C");
            Assert.AreEqual(m.Encipher("A"), "K");

        }

        [TestMethod]
        public void AAA2()
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

            m.SteckerSettings = "EQ  WU  TI  YC  XK";

            m.RotorSettings = "AAA";

            Assert.AreEqual(m.Encipher("A"), "Q");
            Assert.AreEqual(m.Encipher("A"), "U");
            Assert.AreEqual(m.Encipher("A"), "I");
            Assert.AreEqual(m.Encipher("A"), "C");
            Assert.AreEqual(m.Encipher("A"), "K");
        }

        [TestMethod]
        public void M3MessageOutput()
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

        [TestMethod]
        public void M3KMessageOutput()
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

        [TestMethod]
        public void M4MessageOutput()
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

        [TestMethod]
        public void EmptySettingsHaveExpectedValues()
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

        [TestMethod]
        public void RandomSettingsContainValidReflector()
        {
            Settings s = Settings.Random(MachineType.M3);

            ReflectorList reflectors = new ReflectorList(s.MachineType);
            Assert.IsTrue(reflectors.Select(r => r.ReflectorType).Contains(s.ReflectorType));
        }

        [TestMethod]
        public void RandomSettingsContainValidRotors()
        {
            Settings s = Settings.Random(MachineType.M3);

            RotorList rotors = new RotorList(MachineType.M3);
            Assert.AreEqual(rotors.Select(r => r.RotorName).Intersect(s.Rotors.Select(r => r.Name)).Count(), s.Rotors.Count);

        }

        [TestMethod]
        public void RandomSettingsAreValid()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> brokenRules;

            Assert.AreEqual(true, Validator.Validate(s, out brokenRules));
        }

        [TestMethod]
        public void InvalidReflectorIsInvalid()
        {
            Settings s = Settings.Empty();
            s.ReflectorType = ReflectorType.B_Dunn;
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validator.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorTypeForMachine).Any());
        }

        [TestMethod]
        public void InvalidRotorCountIsInvalid()
        {
            Settings s = Settings.Empty();
            s.Rotors.RemoveAt(0);
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validator.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorCount).Any());
        }

        [TestMethod]
        public void InvalidRotorIsInvalid()
        {
            Settings s = Settings.Empty();
            s.Rotors[0].Name = RotorName.Gamma;
            List<BrokenRule> brokenRules;

            Assert.AreEqual(false, Validator.Validate(s, out brokenRules));
            Assert.AreEqual(true, brokenRules.Select(r => r.FailureType == ValidationFailureType.InvalidRotorTypeForMachine).Any());
        }

        [TestMethod]
        public void InvalidRotorRingSettingIsInvalid()
        {
            Settings s = Settings.Empty();

            s.Rotors[0].RingSetting = -1;
            List<BrokenRule> rules;

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.RingSettingOutOfRange).Any());
        }

        [TestMethod]
        public void MissingThinRotor()
        {
            Settings s = Settings.Empty();
            s.MachineType = MachineType.M4K;
            List<BrokenRule> rules = new List<BrokenRule>();

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.ThinRotorMissing).Any());
        }

        [TestMethod]
        public void TooManyPlugs()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.TooManyPlugs).Any());
        }

        [TestMethod]
        public void NonUniquePlugLinks()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "A" });

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.PlugsLinksNotUnique).Any());
        }

        [TestMethod]
        public void DuplicatePlugs()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });
            s.Plugs.Add(new PlugSetting { LetterA = "A", LetterB = "B" });

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.DuplicatePlugs).Any());
        }

        [TestMethod]
        public void DuplicatedLetters()
        {
            Settings s = Settings.Random(MachineType.M3);
            List<BrokenRule> rules = new List<BrokenRule>();

            s.Plugs.Clear();
            s.Plugs.Add(new PlugSetting("AB"));
            s.Plugs.Add(new PlugSetting("CB"));
            s.Plugs.Add(new PlugSetting("SA"));

            Assert.AreEqual(false, Validator.Validate(s, out rules));
            Assert.AreEqual(true, rules.Select(r => r.FailureType == ValidationFailureType.LettersDuplicatedInPlugs).Any());
        }

        [TestMethod]
        public void SerializationAndSave()
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

        [TestMethod]
        public void MonthlySettingsHaveCorrectNumberOfDays()
        {
            int days = DateTime.DaysInMonth(2015, 12);
            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn);
            Assert.AreEqual(ms.DailySettings.Count, days);
        }

        [TestMethod]
        public void MonthlyRotorSettingsInCompatibilityMode()
        {
            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn, true);
            Assert.AreEqual(true, ms.DailySettings.All(s => s.Rotors[0].Name == RotorName.Beta && s.Rotors[0].RingSetting == 0));
        }

        [TestMethod]
        public void MonthlySettingsSerializationAndSave()
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

        [TestMethod]
        public void ParseSettingLineM3()
        {
            Settings s = Settings.ParseSettingLine(M3_SETTING_LINE);

            Assert.AreEqual(s.MachineType, MachineType.M3);
            Assert.AreEqual(s.ReflectorType, ReflectorType.C);

            Assert.AreEqual(s.Rotors[0].Name, RotorName.I);
            Assert.AreEqual(s.Rotors[1].Name, RotorName.III);
            Assert.AreEqual(s.Rotors[2].Name, RotorName.IV);

            Assert.AreEqual(s.Rotors[0].RingSetting, 18);
            Assert.AreEqual(s.Rotors[1].RingSetting, 5);
            Assert.AreEqual(s.Rotors[2].RingSetting, 23);

            Assert.AreEqual(s.Plugs[0].ToString(), "CQ");
            Assert.AreEqual(s.Plugs[1].ToString(), "DV");
            Assert.AreEqual(s.Plugs[2].ToString(), "ES");
            Assert.AreEqual(s.Plugs[3].ToString(), "FO");
            Assert.AreEqual(s.Plugs[4].ToString(), "GT");
            Assert.AreEqual(s.Plugs[5].ToString(), "IU");
            Assert.AreEqual(s.Plugs[6].ToString(), "JR");
            Assert.AreEqual(s.Plugs[7].ToString(), "KM");
            Assert.AreEqual(s.Plugs[8].ToString(), "LY");
            Assert.AreEqual(s.Plugs[9].ToString(), "NW");

        }

        [TestMethod]
        public void ParseSettingLineM3K()
        {
            Settings s = Settings.ParseSettingLine(M3K_SETTING_LINE);

            Assert.AreEqual(s.MachineType, MachineType.M3K);
            Assert.AreEqual(s.ReflectorType, ReflectorType.B);

            Assert.AreEqual(s.Rotors[0].Name, RotorName.VIII);
            Assert.AreEqual(s.Rotors[1].Name, RotorName.VII);
            Assert.AreEqual(s.Rotors[2].Name, RotorName.II);

            Assert.AreEqual(s.Rotors[0].RingSetting, 11);
            Assert.AreEqual(s.Rotors[1].RingSetting, 11);
            Assert.AreEqual(s.Rotors[2].RingSetting, 12);

            Assert.AreEqual(s.Plugs[0].ToString(), "AG");
            Assert.AreEqual(s.Plugs[1].ToString(), "BI");
            Assert.AreEqual(s.Plugs[2].ToString(), "CU");
            Assert.AreEqual(s.Plugs[3].ToString(), "ES");
            Assert.AreEqual(s.Plugs[4].ToString(), "FT");
            Assert.AreEqual(s.Plugs[5].ToString(), "HZ");
            Assert.AreEqual(s.Plugs[6].ToString(), "JP");
            Assert.AreEqual(s.Plugs[7].ToString(), "KO");
            Assert.AreEqual(s.Plugs[8].ToString(), "MX");
            Assert.AreEqual(s.Plugs[9].ToString(), "RW");
        }

        [TestMethod]
        public void ParseSettingLineM4K()
        {
            Settings s = Settings.ParseSettingLine(M4K_SETTING_LINE);

            Assert.AreEqual(s.MachineType, MachineType.M4K);
            Assert.AreEqual(s.ReflectorType, ReflectorType.B_Dunn);

            Assert.AreEqual(s.Rotors[0].Name, RotorName.Beta);
            Assert.AreEqual(s.Rotors[1].Name, RotorName.II);
            Assert.AreEqual(s.Rotors[2].Name, RotorName.V);
            Assert.AreEqual(s.Rotors[3].Name, RotorName.I);

            Assert.AreEqual(s.Rotors[0].RingSetting, 7);
            Assert.AreEqual(s.Rotors[1].RingSetting, 19);
            Assert.AreEqual(s.Rotors[2].RingSetting, 18);
            Assert.AreEqual(s.Rotors[3].RingSetting, 8);

            Assert.AreEqual(s.Plugs[0].ToString(), "BI");
            Assert.AreEqual(s.Plugs[1].ToString(), "CQ");
            Assert.AreEqual(s.Plugs[2].ToString(), "DK");
            Assert.AreEqual(s.Plugs[3].ToString(), "EU");
            Assert.AreEqual(s.Plugs[4].ToString(), "FY");
            Assert.AreEqual(s.Plugs[5].ToString(), "JS");
            Assert.AreEqual(s.Plugs[6].ToString(), "LT");
            Assert.AreEqual(s.Plugs[7].ToString(), "NP");
            Assert.AreEqual(s.Plugs[8].ToString(), "RZ");
            Assert.AreEqual(s.Plugs[9].ToString(), "VX");
        }

        [TestMethod]
        public void M4KCompatibility()
        {
            Settings s = new Settings(MachineType.M4K, ReflectorType.B_Dunn);

            s.Rotors.Add(new RotorSetting(RotorName.Beta, 0));
            s.Rotors.Add(new RotorSetting(RotorName.I, 12));
            s.Rotors.Add(new RotorSetting(RotorName.II, 13));
            s.Rotors.Add(new RotorSetting(RotorName.VII, 22));

            s.Plugs.Add(new PlugSetting("CN"));
            s.Plugs.Add(new PlugSetting("DI"));
            s.Plugs.Add(new PlugSetting("EQ"));
            s.Plugs.Add(new PlugSetting("FL"));
            s.Plugs.Add(new PlugSetting("GV"));
            s.Plugs.Add(new PlugSetting("KR"));
            s.Plugs.Add(new PlugSetting("MS"));
            s.Plugs.Add(new PlugSetting("OY"));
            s.Plugs.Add(new PlugSetting("PX"));
            s.Plugs.Add(new PlugSetting("WZ"));

            Message msg = new Message(s);
            string encrypted = msg.Encrypt(PLAIN_TEXT, "ALOT");

            s.MachineType = MachineType.M3K;
            s.ReflectorType = ReflectorType.B;

            s.Rotors.Clear();

            s.Rotors.Add(new RotorSetting(RotorName.I, 12));
            s.Rotors.Add(new RotorSetting(RotorName.II, 13));
            s.Rotors.Add(new RotorSetting(RotorName.VII, 22));

            s.Plugs.Clear();

            s.Plugs.Add(new PlugSetting("CN"));
            s.Plugs.Add(new PlugSetting("DI"));
            s.Plugs.Add(new PlugSetting("EQ"));
            s.Plugs.Add(new PlugSetting("FL"));
            s.Plugs.Add(new PlugSetting("GV"));
            s.Plugs.Add(new PlugSetting("KR"));
            s.Plugs.Add(new PlugSetting("MS"));
            s.Plugs.Add(new PlugSetting("OY"));
            s.Plugs.Add(new PlugSetting("PX"));
            s.Plugs.Add(new PlugSetting("WZ"));

            msg = new Message(s);

            string encrypted2 = msg.Encrypt(PLAIN_TEXT, "LOT");

            Assert.AreEqual(encrypted, encrypted2);

        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void InvalidSettingsOnMessageThrowsExpectedException()
        {
            //settings are purposely invalid
            Settings s = new Settings(MachineType.M4K, ReflectorType.B);
            Message msg = new Message(s);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void InvalidRotorSettingsOnMessageEncryptDecryptThrowsExpectedException()
        {
            Settings s = Settings.Random(MachineType.M4K);
            Message msg = new Message(s);

            //too many rotor settings specified
            string cipherText = msg.Encrypt(PLAIN_TEXT, "AAAAA");
        }

        [TestMethod]
        public void ExceptionIncludesBrokenRules()
        {
            try
            {
                Settings s = new Settings(MachineType.M4K, ReflectorType.B);
                s.Validate();
            }
            catch (ValidationException valEx)
            {
                Assert.IsTrue(valEx.BrokenRules.Any());
                return;
            }
            Assert.Fail("No exception thrown");
        }

        private int xmlValidationCount = 0;
        [TestMethod]
        public void MonthlySettingsSaveValidXmlWhenSerializing()
        {
            string folderName = Path.GetTempPath();
            string fileName = Path.GetTempFileName();

            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn, false);

            string filePath = Path.Combine(folderName, fileName);
            ms.Save(filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            doc.Schemas.Add("", Path.Combine(folderName, "Settings.xsd"));
            doc.Validate(XmlValidationHandler);

            Assert.AreEqual(xmlValidationCount, 0);

            File.Delete(filePath);
            File.Delete(Path.Combine(folderName, "Settings.xsd"));
        }

        private void XmlValidationHandler(object sender, ValidationEventArgs e)
        {
            xmlValidationCount += 1;
        }

        [TestMethod]
        public void WhenSavingSettingsTheSchemaFileIsAlsoSaved()
        {
            string folderName = Path.GetTempPath();
            string fileName = Path.GetTempFileName();

            MonthlySettings ms = MonthlySettings.Random(2015, 12, MachineType.M4K, ReflectorType.B_Dunn, false);

            string filePath = Path.Combine(folderName, fileName);
            ms.Save(filePath);

            Assert.AreEqual(true, File.Exists(Path.Combine(folderName, "Settings.xsd")));

            File.Delete(filePath);
            File.Delete(Path.Combine(folderName, "Settings.xsd"));
        }

        [TestMethod]
        public void TestYearlyFileExport()
        {
            string folderName = Path.GetTempPath();
            string destinationName = Guid.NewGuid().ToString("N");
            string path = Path.Combine(folderName, destinationName);

            Directory.CreateDirectory(path);

            int y = DateTime.Now.Year;

            Enigma.Util.IoUtil.SaveYear(path, "Settings", y, MachineType.M4K);

            for (int mo = 1; mo <= 12; mo++)
            {
                string monthFolderName = Path.Combine(path, Enigma.Util.IoUtil.MonthFolderName(y, mo));
                Assert.AreEqual(true, Directory.Exists(monthFolderName));

                string txtPath = Path.Combine(monthFolderName, Enigma.Util.IoUtil.MonthFileName(y, mo, ".txt"));
                Assert.AreEqual(true, File.Exists(txtPath));

                string xmlPath = Path.Combine(monthFolderName, Enigma.Util.IoUtil.MonthFileName(y, mo, ".xml"));
                Assert.AreEqual(true, File.Exists(xmlPath));

                string xsdPath = Path.Combine(path, Enigma.Util.IoUtil.XsdFileName(y, mo));
                Assert.AreEqual(true, File.Exists(xsdPath));

                File.Delete(xsdPath);
                File.Delete(xmlPath);
                File.Delete(txtPath);

                Directory.Delete(monthFolderName);
            }

            Directory.Delete(path);
        }

        [TestMethod]
        public void TestCharacterByCharacterTyping()
        {
            Message msg = new Message(Settings.ParseSettingLine(M4K_SETTING_LINE));
            string ct = msg.Encrypt(PLAIN_TEXT, "TGBD");
            string finalPos = msg.CurrentRotorPositions;

            StringBuilder sb = new StringBuilder();

            string rotorPosition = "TGBD";

            foreach (char c in PLAIN_TEXT)
            {
                sb.Append(msg.Encrypt(c.ToString(), rotorPosition));
                rotorPosition = msg.CurrentRotorPositions;
            }

            Assert.AreEqual(ct, Enigma.Util.Formatting.OutputGroups(sb.ToString(), 4));
        }

        [TestMethod]
        public void CheckEqualityMethodsAndOperators()
        {
            Settings s3 = Settings.ParseSettingLine(M3K_SETTING_LINE);
            Settings s4 = Settings.ParseSettingLine(M4K_SETTING_LINE);
            object o3 = Settings.ParseSettingLine(M3K_SETTING_LINE);

            Assert.AreEqual(s3.Equals(s4), false);
            Assert.AreEqual(s4.Equals(s3), false);
            Assert.AreEqual(s3.Equals(o3), true);
            Assert.AreEqual(s3.Equals(null), false);

            Settings s42 = Settings.ParseSettingLine(M4K_SETTING_LINE);

            Assert.AreEqual(s4.GetHashCode(), s42.GetHashCode());
            Assert.AreEqual(s4 == s42, true);
            Assert.AreEqual(s4 != s3, true);
        }

        [TestMethod]
        public void CheckMonthlySettingParsingAndEquality()
        {
            MonthlySettings monSet = MonthlySettings.Random(2000, 1, MachineType.M3);
            MonthlySettings check = MonthlySettings.Parse(monSet.ToString());
            Assert.IsTrue(monSet == check);

            monSet = MonthlySettings.Random(2000, 1, MachineType.M3K);
            check = MonthlySettings.Parse(monSet.ToString());
            Assert.IsTrue(monSet == check);

            monSet = MonthlySettings.Random(2000, 1, MachineType.M4K);
            check = MonthlySettings.Parse(monSet.ToString());
            Assert.IsTrue(monSet == check);
        }

        [TestMethod]
        public void CheckSettingsCloneMethod()
        {
            Settings s = Settings.Random(MachineType.M4K);
            var check = s.Clone() as Settings;

            Assert.IsTrue(check != null && check == s);
        }

        [TestMethod]
        public void CheckMonthlySettingsCloneMethod()
        {
            MonthlySettings monset = MonthlySettings.Random(2000, 1, MachineType.M3);
            var check = monset.Clone() as MonthlySettings;

            Assert.IsTrue(check != null && check == monset);
        }

        [TestMethod]
        public void CreateKeySheet()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            IoUtil.SaveKeySheet(DateTime.Now.Year, path);
        }

        [TestMethod]
        public void TestArmyMessaging()
        {
            string fileName = IoUtil.MonthFileName(DateTime.Now.Year, DateTime.Now.Month, ".xml");
            string folder = Path.GetTempPath();

            string xmlFullPath = Path.Combine(folder, fileName);

            MonthlySettings monSet = MonthlySettings.Random(DateTime.Now.Year, DateTime.Now.Month, MachineType.M3);

            monSet.Save(xmlFullPath);

            Messaging.ArmyMessage msg = new Messaging.ArmyMessage(xmlFullPath, LONG_TEST_PLAIN_TEXT, DateTime.Now.Day);

            string messageText = msg.ToString();

            string decrypted = Messaging.ArmyMessage.Decrypt(xmlFullPath, messageText);
            string cleanPlain = Messaging.Utility.GetPaddedString(Messaging.Utility.CleanString(LONG_TEST_PLAIN_TEXT), 5);

            Assert.AreEqual(decrypted, cleanPlain);

            File.Delete(xmlFullPath);
        }

        [TestMethod]
        public void TestNavyMessagingM4()
        {
            string fileName = IoUtil.MonthFileName(DateTime.Now.Year, DateTime.Now.Month, ".xml");
            string digraphFileName = IoUtil.DigraphFileName(DateTime.Now.Year, DateTime.Now.Month);

            string folder = Path.GetTempPath();

            string xmlFullPath = Path.Combine(folder, fileName);
            string digraphFullPath = Path.Combine(folder, digraphFileName);

            MonthlySettings monSet = MonthlySettings.Random(DateTime.Now.Year, DateTime.Now.Month, MachineType.M4K);
            monSet.Save(xmlFullPath);
            IoUtil.SaveDigraphTable(DateTime.Now.Year, DateTime.Now.Month, folder);

            Messaging.NavyMessage msg = new Messaging.NavyMessage(xmlFullPath, digraphFullPath, LONG_TEST_PLAIN_TEXT, DateTime.Now.Day, "XYZ");

            string messageText = msg.ToString();

            string decrypted = Messaging.NavyMessage.Decrypt(xmlFullPath, digraphFullPath, messageText);
            string cleanPlain = Messaging.Utility.GetPaddedString(Messaging.Utility.CleanString(LONG_TEST_PLAIN_TEXT), 4);

            Assert.AreEqual(decrypted, cleanPlain);

            File.Delete(xmlFullPath);
            File.Delete(digraphFullPath);
        }

        [TestMethod]
        public void TestNavyMessagingM3()
        {
            string fileName = IoUtil.MonthFileName(DateTime.Now.Year, DateTime.Now.Month, ".xml");
            string digraphFileName = IoUtil.DigraphFileName(DateTime.Now.Year, DateTime.Now.Month);

            string folder = Path.GetTempPath();

            string xmlFullPath = Path.Combine(folder, fileName);
            string digraphFullPath = Path.Combine(folder, digraphFileName);

            MonthlySettings monSet = MonthlySettings.Random(DateTime.Now.Year, DateTime.Now.Month, MachineType.M3K);
            monSet.Save(xmlFullPath);
            IoUtil.SaveDigraphTable(DateTime.Now.Year, DateTime.Now.Month, folder);

            Messaging.NavyMessage msg = new Messaging.NavyMessage(xmlFullPath, digraphFullPath, LONG_TEST_PLAIN_TEXT, DateTime.Now.Day, "XYZ");

            string messageText = msg.ToString();

            string decrypted = Messaging.NavyMessage.Decrypt(xmlFullPath, digraphFullPath, messageText);
            string cleanPlain = Messaging.Utility.GetPaddedString(Messaging.Utility.CleanString(LONG_TEST_PLAIN_TEXT), 4);

            Assert.AreEqual(decrypted, cleanPlain);

            File.Delete(xmlFullPath);
            File.Delete(digraphFullPath);
        }

    }
}
