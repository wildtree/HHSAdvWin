// Properties for HHSAdvSDL

using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace HHSAdvWin
{
    public enum ThemeType { Light, Dark, System }
    public class ZProperties : INotifyPropertyChanged
    {
        public class Attributes
        {
            public string FontPath { get; set; } = @"C:\Windows\Fonts\YuGothR.ttc";
            public bool OpeningRoll { get; set; } = true;
            public bool PlaySound { get; set; } = true;
            private ThemeType themeMode = ThemeType.Light;
            public ThemeType ThemeMode 
            {
                get => themeMode;
                set
                {
                    if (themeMode != value)
                    {
                        themeMode = value;
                        bool mode = ((value == ThemeType.System && ZSystem.Instance.IsSystemInDarkMode) || value == ThemeType.Dark);
                        ZSystem.Instance.DarkMode = mode;
                    }
                }
            }
            public int FontSize { get; set; } = 12;

            public int WindowWidth { get; set; } = 320;
            public int WindowHeight { get; set; } = 450;
        }

        public int FontSize
        {
            get => attributes.FontSize;
            set
            {
                if (attributes.FontSize != value)
                {
                    attributes.FontSize = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FontSize)));
                }
            }
        }

        public ThemeType ThemeMode
        {
            get => attributes.ThemeMode;
            set
            {
                if (attributes.ThemeMode != value)
                {
                    attributes.ThemeMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeMode)));
                }
            }
        }
        public bool OpeningRoll
        {
            get => attributes.OpeningRoll;
            set
            {
                if (attributes.OpeningRoll != value)
                {
                    attributes.OpeningRoll = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OpeningRoll)));
                }
            }
        }
        public bool PlaySound
        {
            get => attributes.PlaySound;
            set
            {
                if (attributes.PlaySound != value)
                {
                    attributes.PlaySound = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlaySound)));
                }
            }
        }
        public int WindowWidth
        {
            get => attributes.WindowWidth;
            set
            {
                if (attributes.WindowWidth != value)
                {
                    attributes.WindowWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowWidth)));
                }
            }
        }
        public int WindowHeight
        {
            get => attributes.WindowHeight;
            set
            {
                if (attributes.WindowHeight != value)
                {
                    attributes.WindowHeight = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowHeight)));
                }
            }
        }

        private Attributes attributes = new Attributes();

        public event PropertyChangedEventHandler? PropertyChanged;


        public Attributes Attrs
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public ZProperties()
        {
        }

        public bool Load(string fileName)
        {
            if (!File.Exists(fileName)) return false;

            string jsonString = File.ReadAllText(fileName);
            if (string.IsNullOrEmpty(jsonString))
            {
                return false;
            }
            var deserializedAttributes = JsonSerializer.Deserialize<Attributes>(jsonString);
            if (deserializedAttributes == null)
            {
                return false;
            }
            attributes = deserializedAttributes;

            return true;
        }
        public bool Save(string fileName)
        {
            File.WriteAllText(fileName, JsonSerializer.Serialize(attributes));
            return true;
        }

    }
}
