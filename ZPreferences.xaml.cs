using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static HHSAdvWin.ZProperties;

namespace HHSAdvWin
{
    /// <summary>
    /// ZPreferences.xaml の相互作用ロジック
    /// </summary>
    public partial class ZPreferences : Window
    {
        public ZProperties.Attributes Settings
        {
            get
            {
                return ZSystem.Instance.Properties.Attrs;
            }
            private set
            {
                ZSystem.Instance.Properties.Attrs = value;
            }
        }

        public ZPreferences()
        {
            InitializeComponent();
            DataContext = new ZPreferencesModel(ZSystem.Instance.Properties);
            switch (Settings.FontSize)
            {
                case 12:
                    FontSizeBtn12.IsChecked = true;
                    break;
                case 16:
                    FontSizeBtn16.IsChecked = true;
                    break;
                case 20:
                    FontSizeBtn20.IsChecked = true;
                    break;
                case 24:
                    FontSizeBtn24.IsChecked = true;
                    break;
                default:
                    FontSizeBtn12.IsChecked = true;
                    break;
            }
        }

        private void Okay_Click(object sender, RoutedEventArgs e)
        {
            var props = typeof(ZProperties.Attributes).GetProperties();
            // validation if necessary
            /*
            foreach (var prop in props)
            {
                
            }
            */
            ZSystem.Instance.SavePreferences();
            DialogResult = true;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            =>  WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
