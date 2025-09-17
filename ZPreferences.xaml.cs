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
            DialogResult = true;
        }
    }
}
