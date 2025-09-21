using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;


namespace HHSAdvWin
{
    public static class IconHelper
    {
        public static BitmapSource GetAppIcon()
        {
            // 実行中のアセンブリのパスを取得
            string exePath = Assembly.GetEntryAssembly()!.Location;

            using (Icon icon = Icon.ExtractAssociatedIcon(exePath)!)
            {
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
