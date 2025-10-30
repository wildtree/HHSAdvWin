using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HHSAdvWin
{
    public partial class ZPreferencesModel : ObservableObject
    {
        private readonly ZProperties properties;
        public ZPreferencesModel(ZProperties properties)
        {
            this.properties = properties;
            this.properties.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ZProperties.FontSize))
                {
                    OnPropertyChanged(nameof(FontSize));
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
            set
            {
                if (properties.ThemeMode != value)
                {
                    properties.ThemeMode = value;
                    OnPropertyChanged(nameof(ThemeMode));
                }
            }
        }

        public bool OpeningRoll
        {
            get => properties.OpeningRoll;
            set
            {
                if (properties.OpeningRoll != value)
                {
                    properties.OpeningRoll = value;
                    OnPropertyChanged(nameof(OpeningRoll));
                }
            }
        }

        public bool PlaySound
        {
            get => properties.PlaySound;
            set
            {
                if (properties.PlaySound != value)
                {
                    properties.PlaySound = value;
                    OnPropertyChanged(nameof(PlaySound));
                }
            }
        }
    }
}
