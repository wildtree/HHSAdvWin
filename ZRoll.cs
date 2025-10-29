using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace HHSAdvWin
{
    internal class ZRoll
    {
        public MainWindow Owner { get; set; } = System.Windows.Application.Current.MainWindow as MainWindow;
        public string Credits { get; set; } = string.Empty;
        public TimeSpan TotalDuration { get; set; } = TimeSpan.FromSeconds(20);

        public EventHandler Completed { get; set; }
        public ZRoll()
        {
        }

        public void ShowCredits()
        {
            Owner.Overlay.Visibility = Visibility.Visible;

            // ビットマップ生成
            var bmp = CreateCreditsBitmap(Credits, 16);
            Owner.CreditsImage.Source = bmp;
            Owner.CreditsTransform.Y = Owner.ActualHeight;

            // レイアウト確定後にスクロール開始（チラ見え防止）
            Owner.Dispatcher.BeginInvoke(new Action(() =>
            {
                var scroll = new DoubleAnimation(Owner.ActualHeight, -bmp.PixelHeight, TotalDuration)
                {
                    AccelerationRatio = 0,
                    DecelerationRatio = 0
                };
                scroll.Completed += (s2, e2) =>
                {
                    Completed?.Invoke(s2, e2);
                    Owner.Overlay.Visibility = Visibility.Collapsed;
                    Owner.CreditsImage.Source = null;
                };
                Owner.CreditsTransform.BeginAnimation(TranslateTransform.YProperty, scroll);
            }), DispatcherPriority.Render);
        }

        private BitmapSource CreateCreditsBitmap(string creditsText, double fontSize = 24)
        {
            var typeface = new Typeface(new FontFamily("Yu Gothic UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var dpi = VisualTreeHelper.GetDpi(Owner);
            double pixelsPerDip = dpi.PixelsPerDip;

            var formatted = new FormattedText(
                creditsText.Replace("\r\n", "\n"),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                Brushes.White,
                96 //dpi.PixelsPerDip
            )
            {
                TextAlignment = TextAlignment.Center,
                MaxTextWidth = Owner.ActualWidth - fontSize,
                MaxTextHeight = double.PositiveInfinity,
                MaxLineCount = int.MaxValue
            };
            int bmpWidth = (int)Owner.RollRect.Rect.Width;
            int bmpHeight = (int)Math.Ceiling(formatted.Height + formatted.OverhangAfter) + 4;

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawText(formatted, new Point(0, 0));
            }

            var rtb = new RenderTargetBitmap(bmpWidth, bmpHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;

        }

    }
}
