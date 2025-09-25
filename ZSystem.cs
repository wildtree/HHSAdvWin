using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static HHSAdvWin.ZProperties;

namespace HHSAdvWin
{
    internal class ZSystem
    {
        public enum GameStatus { Title = 0, Play = 1, GameOver = 2, }
        public GameStatus Status { get; set; } = GameStatus.Title;

        private static ZSystem? instance = null;
        public string dataFolder { get; private set; } = string.Empty;
        private ZMap? map = null;
        private ZRules? rules = null;
        private ZWords? dict = null;
        private ZObjects? objects = null;
        private ZMessage? messages = null;
        private ZAudio? audio = null;
        private ZProperties? properties = null;

        private class VersionAttributes
        {
            public int version { get; set; } = 0;
        }
        private VersionAttributes version = new VersionAttributes();
        public ZMap Map
        {
            get
            {
                return map!;
            }
        }
        public ZRules Rules
        {
            get
            {
                return rules!;
            }
        }
        public ZWords Dict
        {
            get
            {
                return dict!;
            }
        }
        public ZObjects Objects
        {
            get
            {
                return objects!;
            }
        }
        public ZMessage Messages
        {
            get
            {
                return messages!;
            }
        }
        public ZAudio Audio
        {
            get { return audio!; }
        }
        public ZProperties Properties
        {
            get
            {
                return properties!;
            }
        }
        public static ZSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ZSystem();
                }
                return instance;
            }
        }

        private bool darkMode = false;
        public bool DarkMode
        {
            get { return darkMode; }
            set
            {
                darkMode = value;
                string themePath = value ? "data/themes/DarkTheme.xaml" : "data/themes/LightTheme.xaml";
                var uri = new Uri(themePath, UriKind.Relative);
                ResourceDictionary themeDict = new ResourceDictionary() { Source = uri };

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(themeDict);
            }
        }
        public bool IsSystemInDarkMode
        {
            get
            {
                const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKeyPath))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value is int intValue)
                        {
                            return intValue == 0; // 0 = ダークモード, 1 = ライトモード
                        }
                    }
                }
                return false; // デフォルトはライトモード
            }
        }
        private ZSystem()
        {
            dataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HHSAdvWin");
            string srcDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            CopyData(srcDir);
            map = new ZMap(System.IO.Path.Combine(dataFolder, "map.dat"));
            rules = new ZRules(System.IO.Path.Combine(dataFolder, "rule.dat"));
            dict = new ZWords(System.IO.Path.Combine(dataFolder, "highds.com"));
            objects = new ZObjects(System.IO.Path.Combine(dataFolder, "thin.dat"));
            messages = new ZMessage(System.IO.Path.Combine(dataFolder, "msg.dat"));
            audio = new ZAudio(dataFolder);
            properties = new ZProperties();
        }

        public void Init()
        {
            Properties.Load(System.IO.Path.Combine(dataFolder, "HHSAdvWin.json"));
            bool dm = ((Properties.Attrs.ThemeMode == ThemeType.System && IsSystemInDarkMode) || Properties.Attrs.ThemeMode == ThemeType.Dark);
            DarkMode = dm;
            Status = GameStatus.Title;
            ZUserData.Instance.load(System.IO.Path.Combine(dataFolder, "data.dat"));
            map.Cursor = 76;
        }
        public void Quit()
        {
            SavePreferences();
        }
        public void SavePreferences()
        {
            Properties.Save(System.IO.Path.Combine(dataFolder, "HHSAdvWin.json"));
        }

        private int GetVersion(string dirName)
        {
            string fileName = System.IO.Path.Combine(dirName, "version.json");
            if (!File.Exists(fileName)) return 0;
            string jsonString = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(jsonString))
            {
                return 0;
            }
            var deserializedAttributes = JsonSerializer.Deserialize<VersionAttributes>(jsonString);
            if (deserializedAttributes == null)
            {
                return 0;
            }
            version = deserializedAttributes;
            return version.version;
        }

        private bool CopyData(string srcDir)
        {
            if (!Directory.Exists(srcDir)) return false;
            if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);
            int srcVer = GetVersion(srcDir);
            int dstVer = GetVersion(dataFolder);
            if (srcVer > dstVer)
            {
                return CopyRecursive(srcDir, dataFolder);
            }
            return true;
        }

        private bool CopyRecursive(string srcDir, string dstDir)
        {
            if (!Directory.Exists(srcDir)) return false;
            if (!Directory.Exists(dstDir)) Directory.CreateDirectory(dstDir);
            foreach (var srcPath in Directory.GetFiles(srcDir))
            {
                string fileName = Path.GetFileName(srcPath);
                string dstPath = System.IO.Path.Combine(dstDir, fileName);
                File.Copy(srcPath, dstPath, true);
            }
            foreach (var subDir in Directory.GetDirectories(srcDir))
            {
                string dirName = Path.GetFileName(subDir);
                string dstSubDir = System.IO.Path.Combine(dstDir, dirName);
                CopyRecursive(subDir, dstSubDir);
            }
            return true;
        }
    }
}

