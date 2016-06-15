using Enigma.Enums;
using Enigma.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Enigma.Configuration
{
    [Serializable]
    public class MonthlySettings
    {
        [XmlAttribute]
        public int Year { get; set; }
        [XmlAttribute]
        public int Month { get; set; }

        public List<Settings> DailySettings { get; set; }

        public MonthlySettings()
        {
            DailySettings = new List<Settings>();
            noNamespaceSchemaLocation = "Settings.xsd";
        }

        public void Save(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MonthlySettings));
            using (FileStream stm = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                ser.Serialize(stm, this);
            }
            //this extracts the xsd file into the same directory as the settings file
            IoUtil.ExtractXsd(fileName);
        }

        public static MonthlySettings Open(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MonthlySettings));
            using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return (MonthlySettings)ser.Deserialize(stm);
            }
        }

        public static MonthlySettings Random(int year, int month, MachineType t)
        {
            MonthlySettings result = new MonthlySettings();

            result.Month = month;
            result.Year = year;

            for (int i = DateTime.DaysInMonth(year, month); i > 0; i--)
            {
                Settings s = Settings.Random(t);
                s.Day = i;
                result.DailySettings.Add(s);
            }

            return result;
        }
        public static MonthlySettings Random(int year, int month, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            MonthlySettings result = new MonthlySettings();

            result.Month = month;
            result.Year = year;

            for (int i = DateTime.DaysInMonth(year, month); i > 0; i--)
            {
                Settings s = Settings.Random(t);
                s.Day = i;
                result.DailySettings.Add(s);
            }

            foreach (var item in result.DailySettings)
            {
                item.ReflectorType = r;
                if (t == MachineType.M4K)
                {
                    if (r == ReflectorType.B_Dunn)
                    {
                        item.Rotors[0].Name = RotorName.Beta;
                        if (compatibilityMode)
                        {
                            item.Rotors[0].RingSetting = 0;
                        }
                    }
                    else
                    {
                        item.Rotors[0].Name = RotorName.Gamma;
                        if (compatibilityMode)
                        {
                            item.Rotors[0].RingSetting = 0;
                        }
                    }
                }
            }

            return result;
        }

        public override string ToString()
        {
            if (DailySettings.Any())
                return Formatting.MonthlySettings("Settings", Year, Month, DailySettings[0].MachineType, DailySettings);
            return
                base.ToString();   

        }

        //this is a kludge to get the xml serializer to include the schema location when serializing to xml        
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation
        {
            get; set;
        }
    }
}
