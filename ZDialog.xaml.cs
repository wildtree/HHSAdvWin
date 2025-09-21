using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// ZDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ZDialog : Window
    {
        private string Title { get; set; } = "選択";
        private string Message { get; set; } = "選択してください";
        private string[] Labels =
        {
            "#1", "#2", "#3",
        };
        public int Selected { get; private set; } = 0;
        public ZDialog(string title, string message, string[] labels)
        {
            InitializeComponent();
            Title = title;
            TitleTextBlock.Text = Title;
            Message = message;
            Labels = labels;
            Loaded += ZDialog_Loaded;
        }

        private void ZDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ZDialogWindow.Title = Title;
            textBox.Clear();
            textBox.Text = Message;
            bool isChecked = true;
            if (string.IsNullOrEmpty(Labels[0]))
            {
                Option1.Visibility = Visibility.Collapsed;
            }
            else
            {
                Option1.Visibility = Visibility.Visible;
                Option1.Content = Labels[0];
                Option1.IsChecked = isChecked;
                isChecked = false;
            }
            if (string.IsNullOrEmpty(Labels[1]))
            {
                Option2.Visibility = Visibility.Collapsed;
            }
            else
            {
                Option2.Visibility = Visibility.Visible;
                Option2.Content = Labels[1];
                Option2.IsChecked = isChecked;
                isChecked = false;
            }
            if (string.IsNullOrEmpty(Labels[2]))
            {
                Option3.Visibility = Visibility.Collapsed;
            }
            else
            {
                Option3.Visibility = Visibility.Visible;
                Option3.Content = Labels[2];
                Option3.IsChecked = isChecked;
                isChecked = false;
            }
            
        }

        private void Okay_Clicked(object sender, RoutedEventArgs e)
        {
            Selected = 0;
            if (Option1.IsChecked == true)
            {
                Selected = 1;
            }
            if (Option2.IsChecked == true)
            {
                Selected = 2;
            }
            if (Option3.IsChecked == true)
            {
                Selected = 3;
            }
            DialogResult = true;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        // タイトルバーのドラッグ移動用イベントハンドラを追加
        private void TitleBar_DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
