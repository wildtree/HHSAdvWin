using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Schema;

namespace HHSAdvWin
{
    public static class Placeholder
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text", typeof(string), typeof(Placeholder),
                new PropertyMetadata(null, OnAnyPropertyChanged));

        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached(
                "Foreground", typeof(Brush), typeof(Placeholder),
                new PropertyMetadata(Brushes.Gray, OnAnyPropertyChanged));

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.RegisterAttached(
                "FontFamily", typeof(FontFamily), typeof(Placeholder),
                new PropertyMetadata(new FontFamily("Yu Gothic"), OnAnyPropertyChanged));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.RegisterAttached(
                "FontSize", typeof(double), typeof(Placeholder),
                new PropertyMetadata(14.0, OnAnyPropertyChanged));

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached(
                "Margin", typeof(Thickness), typeof(Placeholder),
                new PropertyMetadata(new Thickness(4, 2, 0, 0), OnAnyPropertyChanged));

        public static string GetText(DependencyObject obj) => (string)obj.GetValue(TextProperty);
        public static void SetText(DependencyObject obj, string value) => obj.SetValue(TextProperty, value);

        public static Brush GetForeground(DependencyObject obj) => (Brush)obj.GetValue(ForegroundProperty);
        public static void SetForeground(DependencyObject obj, Brush value) => obj.SetValue(ForegroundProperty, value);

        public static FontFamily GetFontFamily(DependencyObject obj) => (FontFamily)obj.GetValue(FontFamilyProperty);
        public static void SetFontFamily(DependencyObject obj, FontFamily value) => obj.SetValue(FontFamilyProperty, value);

        public static double GetFontSize(DependencyObject obj) => (double)obj.GetValue(FontSizeProperty);
        public static void SetFontSize(DependencyObject obj, double value) => obj.SetValue(FontSizeProperty, value);

        public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);
        public static void SetMargin(DependencyObject obj, Thickness value) => obj.SetValue(MarginProperty, value);

        // 内部保持（TextBoxごとに Adorner と Layer を紐付け）
        private static readonly DependencyProperty AdornerProperty =
            DependencyProperty.RegisterAttached("Adorner", typeof(PlaceholderAdorner), typeof(Placeholder));
        private static void SetAdorner(DependencyObject obj, PlaceholderAdorner value) => obj.SetValue(AdornerProperty, value);
        private static PlaceholderAdorner GetAdorner(DependencyObject obj) => (PlaceholderAdorner)obj.GetValue(AdornerProperty);

        private static readonly DependencyProperty LayerProperty =
            DependencyProperty.RegisterAttached("Layer", typeof(AdornerLayer), typeof(Placeholder));
        private static void SetLayer(DependencyObject obj, AdornerLayer value) => obj.SetValue(LayerProperty, value);
        private static AdornerLayer GetLayer(DependencyObject obj) => (AdornerLayer)obj.GetValue(LayerProperty);

        private static void OnAnyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Loaded -= TextBox_Loaded;
                textBox.Loaded += TextBox_Loaded;
            }
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            void Attach()
            {
                var layer = AdornerLayer.GetAdornerLayer(textBox);
                if (layer == null) return;

                // 既存のプレースホルダー削除
                var adorners = layer.GetAdorners(textBox);
                if (adorners != null)
                {
                    foreach (var ad in adorners)
                        if (ad is PlaceholderAdorner) layer.Remove(ad);
                }

                // 新しいプレースホルダー追加
                var adorner = new PlaceholderAdorner(
                    textBox,
                    GetText(textBox),
                    GetForeground(textBox),
                    GetFontFamily(textBox),
                    GetFontSize(textBox),
                    GetMargin(textBox)
                );
                layer.Add(adorner);
                SetAdorner(textBox, adorner);
                SetLayer(textBox, layer);

                // 再描画トリガー
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.GotFocus -= TextBox_FocusChanged;
                textBox.LostFocus -= TextBox_FocusChanged;
                textBox.SizeChanged -= TextBox_SizeChanged;
                textBox.IsVisibleChanged -= TextBox_IsVisibilityChanged;

                textBox.TextChanged += TextBox_TextChanged;
                textBox.GotFocus += TextBox_FocusChanged;
                textBox.LostFocus += TextBox_FocusChanged;
                textBox.SizeChanged += TextBox_SizeChanged;
                textBox.IsVisibleChanged += TextBox_IsVisibilityChanged;

                adorner.InvalidateVisual();
            }

            if (AdornerLayer.GetAdornerLayer(textBox) == null)
                textBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(Attach));
            else
                Attach();
        }

        private static void TextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.GotFocus -= TextBox_FocusChanged;
            textBox.LostFocus -= TextBox_FocusChanged;
            textBox.SizeChanged -= TextBox_SizeChanged;
            textBox.IsVisibleChanged -= TextBox_IsVisibilityChanged;
            var adoner = GetAdorner(textBox);
            var layer = GetLayer(textBox);
            if (adoner != null && layer != null)
            {
                layer.Remove(adoner);
            }
            SetAdorner(textBox, null);
            SetLayer(textBox, null);
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            GetAdorner(textBox)?.InvalidateVisual();
        }
        private static void TextBox_FocusChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            GetAdorner(textBox)?.InvalidateVisual();
        }
        private static void TextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            GetAdorner(textBox)?.InvalidateVisual();
        }
        private static void TextBox_IsVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            GetAdorner(textBox)?.InvalidateVisual();
        }
    }

    public class PlaceholderAdorner : Adorner
    {
        private readonly string placeholder;
        private readonly Brush brush;
        private readonly Typeface typeface;
        private readonly double fontSize;
        private readonly Thickness margin;

        public PlaceholderAdorner(UIElement adornedElement, string placeholder, Brush brush, FontFamily fontFamily, double fontSize, Thickness margin)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
            this.placeholder = placeholder;
            this.brush = brush ?? Brushes.Gray;
            this.typeface = new Typeface(fontFamily ?? new FontFamily("Yu Gothic"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            this.fontSize = fontSize;
            this.margin = margin;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return AdornedElement.RenderSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (AdornedElement is TextBox textBox && string.IsNullOrEmpty(textBox.Text) && textBox.Visibility == Visibility.Visible)
            {
                var formattedText = new FormattedText(
                    placeholder ?? "",
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    fontSize,
                    brush,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip
                );

                drawingContext.DrawText(formattedText, new Point(margin.Left, margin.Top));
            }
        }
    }
}