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
            DataContext = Settings;
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

        private void ThemeMode_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is ThemeType selected)
            {
                // 設定に反映
                var attrs = DataContext as Attributes;
                if (attrs != null)
                {
                    attrs.ThemeMode = selected;
                }

                // 実際にテーマを適用
                bool mode = ((selected == ThemeType.System && ZSystem.Instance.IsSystemInDarkMode) || selected == ThemeType.Dark);
                ZSystem.Instance.DarkMode = mode;
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            =>  WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        private void FontSize_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.FontSize > 0)
            { 
                // MainWindowのインスタンスを取得してLogFontSizeを設定
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    var attrs = DataContext as Attributes;
                    if (attrs != null)
                    {
                        attrs.FontSize = (int)rb.FontSize;
                    }
                    mainWindow.LogFontSize = (int)rb.FontSize;
                }
            }
        }
    }
}
