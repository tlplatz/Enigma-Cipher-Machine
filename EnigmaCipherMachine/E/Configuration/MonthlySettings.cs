using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Enigma.Enums;
using Enigma.Util;

namespace Enigma.Configuration
{
    [Serializable]
    public class MonthlySettings : IEquatable<MonthlySettings>, ICloneable
    {
        const string DEFAULT_SETTING_NAME = "Settings";

        [XmlAttribute]
        public int Year { get; set; }
        [XmlAttribute]
        public int Month { get; set; }
        [XmlAttribute]
        public string Title { get; set; }

        public List<Settings> DailySettings { get; set; }

        public MonthlySettings()
        {
            Title = DEFAULT_SETTING_NAME;
            DailySettings = new List<Settings>();
            noNamespaceSchemaLocation = IoUtil.XSD_FILE_NAME;
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
        public static MonthlySettings Parse(string settings)
        {
            return Formatting.ParseSettings(settings);
        }

        public override string ToString()
        {
            if (DailySettings.Any())
                return Formatting.MonthlySettings("Settings", Year, Month, DailySettings[0].MachineType, DailySettings);
            return
                base.ToString();

        }

        public bool Equals(MonthlySettings other)
        {
            if (other == null) return false;

            if (object.ReferenceEquals(this, other)) return true;

            if (Title != other.Title) return false;
            if (Year != other.Year) return false;
            if (Month != other.Month) return false;

            if (DailySettings.Count != other.DailySettings.Count) return false;

            for(int i=0; i<DailySettings.Count; i++)
            {
                if (DailySettings[i] != other.DailySettings[i])
                {
                    return false;
                }
            }

            return true;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (object.ReferenceEquals(this, obj)) return true;

            MonthlySettings s = obj as MonthlySettings;
            if (s == null) return false;

            return Equals(s);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public object Clone()
        {
            return MonthlySettings.Parse(this.ToString());
        }

        public static bool operator ==(MonthlySettings a, MonthlySettings b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }
        public static bool operator !=(MonthlySettings a, MonthlySettings b)
        {
            return !(a == b);
        }

        //this is a kludge to get the xml serializer to include the schema location when serializing to xml        
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string noNamespaceSchemaLocation
        {
            get; set;
        }
    }
}
