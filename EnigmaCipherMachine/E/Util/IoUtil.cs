using System;
using System.IO;
using Enigma.Configuration;
using Enigma.Enums;

namespace Enigma.Util
{
    /// <summary>
    /// Provides a set of methods for creating configuration setting files
    /// </summary>
    public static class IoUtil
    {
        private const string MONTH_FOLDER_NAME_FORMAT = "{0:MMMM_yyyy}";
        private const string MONTH_FILE_NAME_FORMAT = "{0:MMMM_yyyy}_Settings{1}";
        public const string XSD_FILE_NAME = "Settings.xsd";
        private const string TEXT_FILE_EXTENSION = ".txt";
        private const string XML_FILE_EXTENSION = ".xml";

        private static void SaveFile(string fileName, string content)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(content);
                }
            }
        }
        internal static string MonthFolderName(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
            return string.Format(MONTH_FOLDER_NAME_FORMAT, dt);
        }
        internal static string MonthFileName(int year, int month, string ext)
        {
            DateTime dt = new DateTime(year, month, 1);
            return string.Format(MONTH_FILE_NAME_FORMAT, dt, ext);
        }
        internal static string XsdFileName(int year, int month)
        {
            return Path.Combine(MonthFolderName(year, month), XSD_FILE_NAME);
        }
        private static void ExtractXsd(int year, int month)
        {
            SaveFile(XsdFileName(year, month), Properties.Resources.Settings);
        }
        internal static void ExtractXsd(string fileName)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(fileName));
            if (dir.Exists)
            {
                string xsdFileName = Path.Combine(dir.FullName, XSD_FILE_NAME);
                if (!File.Exists(xsdFileName))
                {
                    SaveFile(xsdFileName, Properties.Resources.Settings);
                }
            }
        }

        public static void SaveMonth(string folder, string title, int year, int month, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            MonthlySettings s = MonthlySettings.Random(year, month, t, r, compatibilityMode);

            string fileName = MonthFileName(year, month, TEXT_FILE_EXTENSION);
            string filePath = Path.Combine(folder, fileName);
            SaveFile(filePath, Formatting.MonthlySettings(title, year, month, t, r, s.DailySettings));

            fileName = MonthFileName(year, month, XML_FILE_EXTENSION);
            filePath = Path.Combine(folder, fileName);
            s.Save(filePath);
        }
        public static void SaveMonth(string folder, string title, int year, int month, MachineType t)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            MonthlySettings s = MonthlySettings.Random(year, month, t);

            string fileName = MonthFileName(year, month, TEXT_FILE_EXTENSION);
            string filePath = Path.Combine(folder, fileName);
            SaveFile(filePath, Formatting.MonthlySettings(title, year, month, t, s.DailySettings));

            fileName = MonthFileName(year, month, XML_FILE_EXTENSION);
            filePath = Path.Combine(folder, fileName);
            s.Save(filePath);
        }

        public static void SaveYear(string folder, string title, int year, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            for (int m = 1; m <= 12; m++)
            {
                string monthFolderName = MonthFolderName(year, m);
                string monthFolderPath = Path.Combine(folder, monthFolderName);

                if (!Directory.Exists(monthFolderPath))
                {
                    Directory.CreateDirectory(monthFolderPath);
                }

                MonthlySettings s = MonthlySettings.Random(year, m, t, r, compatibilityMode);
                string serializedFileName = MonthFileName(year, m, ".xml");

                string serializedPath = Path.Combine(monthFolderPath, serializedFileName);
                s.Save(serializedPath);

                string textFileName = MonthFileName(year, m, ".txt");
                string textFilePath = Path.Combine(monthFolderPath, textFileName);
                SaveFile(textFilePath, Formatting.MonthlySettings(title, year, m, t, r, s.DailySettings));
            }
        }
        public static void SaveYear(string folder, string title, int year, MachineType t)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            for (int m = 1; m <= 12; m++)
            {
                string monthFolderName = MonthFolderName(year, m);
                string monthFolderPath = Path.Combine(folder, monthFolderName);

                if (!Directory.Exists(monthFolderPath))
                {
                    Directory.CreateDirectory(monthFolderPath);
                }

                MonthlySettings s = MonthlySettings.Random(year, m, t);
                string serializedFileName = MonthFileName(year, m, ".xml");

                string serializedPath = Path.Combine(monthFolderPath, serializedFileName);
                s.Save(serializedPath);

                string textFileName = MonthFileName(year, m, ".txt");
                string textFilePath = Path.Combine(monthFolderPath, textFileName);
                SaveFile(textFilePath, Formatting.MonthlySettings(title, year, m, t, s.DailySettings));
            }
        }

        public static void SaveDigraphTable(int year, int month, string folder)
        {
            DigraphTable tbl = DigraphTable.Random(year, month);
            DateTime dt = new DateTime(year, month, 1);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = string.Format("Digraph_{0:yyyyMMM}.txt", dt);
            string fullPath = Path.Combine(folder, fileName);
            SaveFile(fullPath, tbl.ToString());
        }
        public static void SaveKeySheet(int groupSize, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            KeySheet ks = new KeySheet(groupSize);

            string fullPath = Path.Combine(folder, "KeySheet.txt");
            SaveFile(fullPath, ks.ToString());
        }
    }
}
