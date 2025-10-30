using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HHSAdvWin
{
    public partial class MainWindowModel : ObservableObject
    {
        private readonly ZProperties properties;
        public MainWindowModel(ZProperties properties)
        {
            this.properties = properties;
            this.properties.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ZProperties.FontSize))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        OnPropertyChanged(nameof(FontSize));
                    });
                }
                else if (e.PropertyName == nameof(ZProperties.ThemeMode))
                {
                    OnPropertyChanged(nameof(ThemeMode));
                }
                else if (e.PropertyName == nameof(ZProperties.OpeningRoll))
                {
                    OnPropertyChanged(nameof(OpeningRoll));
                }
                else if (e.PropertyName == nameof(ZProperties.PlaySound))
                {
                    OnPropertyChanged(nameof(PlaySound));
                }
                else if (e.PropertyName == nameof(ZProperties.WindowHeight))
                {
                    OnPropertyChanged(nameof(WindowHeight));
                }
                else if (e.PropertyName == nameof(ZProperties.WindowWidth))
                {
                    OnPropertyChanged(nameof(WindowWidth));
                }
            };
        }

        public int FontSize
        {
            get => properties.FontSize;
            set
            {
                if (properties.FontSize != value)
                {
                    properties.FontSize = value;
                    OnPropertyChanged(nameof(FontSize));
                }
            }
        }

        public ThemeType ThemeMode
        {
            get => properties.ThemeMode;
        }

        public bool OpeningRoll
        {
            get => properties.OpeningRoll;
        }

        public bool PlaySound
        {
            get => properties.PlaySound;
        }
        public int WindowHeight
        {
            get => properties.WindowHeight;
            set
            {
                if (properties.WindowHeight != value)
                {
                    properties.WindowHeight = value;
                    OnPropertyChanged(nameof(WindowHeight));
                }
            }
        }
        public int WindowWidth
        {
            get => properties.WindowWidth;
            set
            {
                if (properties.WindowWidth != value)
                {
                    properties.WindowWidth = value;
                    OnPropertyChanged(nameof(WindowWidth));
                }
            }
        }

    }
}
