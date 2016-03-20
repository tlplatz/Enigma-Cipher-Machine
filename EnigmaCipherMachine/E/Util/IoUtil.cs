using Enigma.Configuration;
using Enigma.Enums;
using System;
using System.IO;

namespace Enigma.Util
{
    public class IoUtil
    {
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
        private static string MonthFolderName(int year, int month)
        {
            DateTime dt = new DateTime(year, month, 1);
            return string.Format("{0:MMMM_yyyy}", dt);
        }
        private static string MonthFileName(int year, int month, string ext)
        {
            DateTime dt = new DateTime(year, month, 1);
            return string.Format("{0:MMMM_yyyy}_Settings{1}", dt, ext);
        }

        public static void SaveMonth(string folder, string title, int year, int month, MachineType t, ReflectorType r, bool compatibilityMode = false)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            MonthlySettings s = MonthlySettings.Random(year, month, t, r, compatibilityMode);

            string fileName = MonthFileName(year, month, ".txt");
            string filePath = Path.Combine(folder, fileName);
            SaveFile(filePath, Formatting.MonthlySettings(title, year, month, t, r, s.DailySettings));
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

        public static void SaveMonth(string folder, string title, int year, int month, MachineType t)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            MonthlySettings s = MonthlySettings.Random(year, month, t);

            string fileName = MonthFileName(year, month, ".txt");
            string filePath = Path.Combine(folder, fileName);
            SaveFile(filePath, Formatting.MonthlySettings(title, year, month, t, s.DailySettings));
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

    }
}
