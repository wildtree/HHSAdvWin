using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static HHSAdvWin.ZSystem;
using static System.Net.Mime.MediaTypeNames;

namespace HHSAdvWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Canvas canvas;
        private ZSystem zsystem = ZSystem.Instance;
        private const string CRLF = "\r\n";

        private static string[] title = new string[]
        {
            @"ハイハイスクールアドベンチャー",
            @"Copyright(c)1995-2025",
            @"ZOBplus",
            @"hiro",
        };
        private string credits =
@"High High School Adventure

PalmOS version: hiro © 2002-2004
Android version: hiro © 2011-2025
M5 version: hiro © 2023-2025
Qt version: hiro © 2024
PicoCalc version: hiro © 2025
SDL version: hiro © 2025
Windows version: hiro © 2025

- Project ZOBPlus -
Hayami <hayami@zob.jp>
Exit <exit@zob.jp>
ezumi <ezumi@zob.jp>
Ogu <ogu@zob.jp>
neopara <neopara@zob.jp>
hiro <hiro@zob.jp>

--- Original Staff ---

- Director -
HIRONOBU NAKAGUCHI

- Graphic Designers -

NOBUKO YANAGITA
YUMIKO HOSONO
HIRONOBU NAKAGUCHI
TOSHIHIKO YANAGITA
TOHRU OHYAMA

MASANORI ISHII
YASUSHI SHIGEHARA
HIDETOSHI SUZUKI
TATSUYA UCHIBORI
MASAKI NOZAWA

TOMOKO OHKAWA
FUMIKAZU SHIRATSUCHI
YASUNORI YAMADA
MUNENORI TAKIMOTO

- Message Converters -
TATSUYA UCHIBORI
HIDETOSHI SUZUKI
YASUSHI SHIGEHARA
YASUNORI YAMADA

- Floppy Disk Converters -
HIRONOBU NAKAGUCHI

- Music -
MASAO MIZOBE

- Special Thanks To -
HIROSHI YAMAMOTO
TAKAYOSHI KASHIWAGI

- Cooperate with -
Furniture KASHIWAGI

ZAMA HIGH SCHOOL MICRO COMPUTER CIRCLE";

        private string opening =
@"ストーリー

2019年神奈山県立ハイ高等学校は地盤が弱く校舎の老朽化も進んだため、とうとう廃校にする以外方法がなくなってしまった。
ところで大変な情報を手に入れた。

それは、

「ハイ高校にＡＴＯＭＩＣ ＢＯＭＢが仕掛けられている。」

と、いうものだ。

どうやらハイ高が廃校になった時、気が狂った理科の先生がＡＴＯＭＩＣ ＢＯＭＢを、学校のどこかに仕掛けてしまったらしい。
お願いだ。我が母校のコナゴナになった姿を見たくはない。
早くＡＴＯＭＩＣ ＢＯＭＢを取り除いてくれ……！！

行動は英語で、
<動詞>
或いは、
<動詞>+<目的語>
のように入れていただきたい。
例えば、
look room
と入れれば部屋の様子を見ることが出来るという訳だ。

それでは Ｇｏｏｄ Ｌｕｃｋ！！！............";


        public MainWindow()
        {
            InitializeComponent();
            Loaded += Init;
        }

        private void Init(object sender, EventArgs e)
        {
            canvas = new Canvas(BitmapArea);

            TitleScreen();
            InputTextBox.Visibility = Visibility.Collapsed;
            HitAnyKey.Visibility = Visibility.Visible;
            AppMainWindow.InvalidateVisual();

        }

        private void DrawScreen(bool drawMessage = true)
        {
            if (canvas != null)
            {
                ZMap map = zsystem.Map;
                if (canvas.ColorFilterType == Canvas.FilterType.Blue)
                {
                    canvas.ColorFilterType = Canvas.FilterType.None;
                }
                if (IsDark())
                {
                    canvas.ColorFilterType = Canvas.FilterType.Blue;
                }
                map.Draw(canvas);
                if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                {
                    string s = map.Message;
                    if (map.IsBlank)
                    {
                        LogArea.AppendText(zsystem.Messages.GetMessage(0xcc));
                        LogArea.AppendText(CRLF);
                    }
                    if (!string.IsNullOrEmpty(s))
                    {
                        LogArea.AppendText(s);
                        LogArea.AppendText(CRLF);
                    }
                }
                ZUserData user = ZUserData.Instance;
                ZObjects obj = zsystem.Objects;
                for (int i = 0; i < ZUserData.Items; i++)
                {
                    if (user.getPlace(i) == map.Cursor)
                    {
                        bool shift = (i == 1 && user.getFact(0) != 1);

                        obj.Id = i;
                        obj.Draw(canvas, shift);
                        if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                        {
                            LogArea.AppendText(zsystem.Messages.GetMessage(0x96 + i));
                            LogArea.AppendText(CRLF);
                        }
                    }
                }
                if (user.getFact(1) == map.Cursor)
                {
                    obj.Id = 14;
                    obj.Draw(canvas);
                    if (drawMessage && zsystem.Status != ZSystem.GameStatus.GameOver)
                    {
                        LogArea.AppendText(zsystem.Messages.GetMessage(0xb4));
                        LogArea.AppendText(CRLF);
                    }

                }
                ScrollArea.ScrollToEnd();
                canvas.colorFilter();
                canvas.Invalidate();
            }
        }

        private void TitleScreen()
        {
            System.Windows.Application.Current.MainWindow.PreviewKeyDown += Application_PreviewKeyDown;
            zsystem.Init();
            LogArea.Clear();
            canvas.ColorFilterType = Canvas.FilterType.None;
            StringBuilder sb = new StringBuilder();
            foreach (var s in title)
            {
                sb.AppendLine(s);
            }
            LogArea.AppendText(sb.ToString());
            DrawScreen(false);
        }

        private void Application_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch(zsystem.Status)
            {
                case ZSystem.GameStatus.Title:
                    EventHandler h = null;
                    h = (s, e) =>
                    {
                        zsystem.Map.Cursor = 1;
                        LogArea.Clear();
                        zsystem.Status = ZSystem.GameStatus.Play;
                        // invoke opening roll?
                        DrawScreen(true);
                        HitAnyKey.Visibility = Visibility.Collapsed;
                        InputTextBox.Visibility = Visibility.Visible;
                        InputTextBox.Focus();
                    };
                    System.Windows.Application.Current.MainWindow.PreviewKeyDown -= Application_PreviewKeyDown;
                    if (zsystem.Properties.Attrs.OpeningRoll)
                    {
                        ZRoll openingRoll = new ZRoll { Owner = this, Credits = opening };
                        openingRoll.Completed += h;
                        openingRoll.ShowCredits();
                    }
                    else
                    {
                        h.Invoke(null, null);
                    }
                    e.Handled = true;
                    break;
                case ZSystem.GameStatus.GameOver:
                    System.Windows.Application.Current.MainWindow.PreviewKeyDown -= Application_PreviewKeyDown;
                    TitleScreen();
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                e.Handled = false;
                return;
            }
            e.Handled = true;
            string inputText = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(inputText)) return;
            
            string[] args = inputText.Split(new char[] { ' ' } );
            var cmd = args[0].Trim();
            var obj = (args.Length > 1) ? args[1].Trim() : string.Empty;
            ZCore.Instance.CmdId = (byte)zsystem.Dict.findVerb(cmd);
            ZCore.Instance.ObjId = (byte)zsystem.Dict.findObj(obj);
            // call game interpreter
            InputTextBox.Text = string.Empty;
            StringBuilder sb = new StringBuilder(">> ");
            LogArea.AppendText(sb.Append(inputText).Append(CRLF).ToString());
            //
            TimeElapsed();
            if (zsystem.Status == ZSystem.GameStatus.GameOver) return;
            ExecuteRules();
            if (zsystem.Status == ZSystem.GameStatus.GameOver) return;
            CheckTeacher();
            DrawScreen(true);
        }

        private void GameOver()
        {
            zsystem.Status = ZSystem.GameStatus.GameOver;
            InputTextBox.Visibility = Visibility.Collapsed;
            HitAnyKey.Visibility = Visibility.Visible;
            System.Windows.Application.Current.MainWindow.PreviewKeyDown += Application_PreviewKeyDown;
        }

        private bool IsDark()
        {
            bool dim = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            switch (core.MapId)
            {
                case 47:
                case 48:
                case 49:
                case 61:
                case 64:
                case 65:
                case 67:
                case 68:
                case 69:
                case 71:
                case 74:
                case 75:
                case 77:
                    if (user.getFact(7) != 0)
                    {
                        if (user.getFact(6) != 0)
                        {
                            // dark mode (blue)
                            dim = true;
                        }
                    }
                    else
                    {
                        // blackout
                        core.MapViewId = core.MapId;
                        zsystem.Map.Cursor = 84;
                    }
                    break;
                default:
                    if (user.getFact(6) != 0)
                    {
                        dim = false;
                    }
                    break;
            }
            return dim;
        }
        public void TimeElapsed()
        {
            ZUserData user = ZUserData.Instance;
            ZMessage messages = zsystem.Messages;

            if (user.getFact(3) > 0 && user.getFact(7) == 1)
            {
                // Light is ON
                user.setFact(3, (byte)(user.getFact(3) - 1));
                if (user.getFact(3) < 8 && user.getFact(3) > 0)
                {
                    // battery LOW
                    user.setFact(6, 1); // dim mode
                    LogArea.AppendText(messages.GetMessage(0xd9));
                    LogArea.AppendText(CRLF);
                }
                else if (user.getFact(3) == 0)
                {
                    // battery ware out
                    user.setFact(7, 0); // light off
                    LogArea.AppendText(messages.GetMessage(0xc0));
                    LogArea.AppendText(CRLF);
                }
            }
            if (user.getFact(11) > 0)
            {
                user.setFact(11, (byte)(user.getFact(11) - 1));
                if (user.getFact(11) == 0)
                {
                    LogArea.AppendText(messages.GetMessage(0xd8));
                    if (user.getPlace(7) == 48)
                    {
                        user.getLink(75 - 1).N = 77;
                        user.getLink(68 - 1).W = 77;
                        LogArea.AppendText(messages.GetMessage(0xda));
                        LogArea.AppendText(CRLF);
                    }
                    else if (user.getPlace(7) == 255 || user.getPlace(7) == zsystem.Map.Cursor)
                    {
                        // suicide explosion
                        // set screen color to red
                        canvas!.ColorFilterType = Canvas.FilterType.Red;
                        LogArea.AppendText(messages.GetMessage(0xcf));
                        LogArea.AppendText(CRLF);
                        LogArea.AppendText(messages.GetMessage(0xcb));
                        LogArea.AppendText(CRLF);
                        GameOver();
                    }
                    else
                    {
                        user.setPlace(7, 0);
                    }
                }
            }
        }
        private void CheckTeacher()
        {
            ZUserData user = ZUserData.Instance;
            ZCore core = ZCore.Instance;
            if (zsystem.Status == GameStatus.GameOver || user.getFact(1) == core.MapId)
                return;
            int rd = 100 + core.MapId + ((user.getFact(1) > 0) ? 1000 : 0);
            int rz = new Random().Next(3000);
            user.setFact(1, (byte)((rd < rz) ? 0 : core.MapId));
            switch (core.MapId)
            {
                case 1:
                case 48:
                case 50:
                case 51:
                case 52:
                case 53:
                case 61:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 83:
                case 86:
                    user.setFact(1, 0);
                    break;
            }
        }
        private void SaveGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = System.IO.Path.Combine(zsystem.dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var br = new BinaryWriter(fs))
                {
                    br.Write(core.pack());
                    br.Write(user.pack());
                }
            }
        }
        private void LoadGame(int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(index).Append(".dat");
            string fileName = System.IO.Path.Combine(zsystem.dataFolder, sb.ToString());
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    core.unpack(br.ReadBytes(core.packedSize));
                    user.unpack(br.ReadBytes(user.packedSize));
                }
            }
            zsystem.Map.Cursor = core.MapId;
        }

        private void ExecuteRules()
        {
            bool okay = false;
            ZCore core = ZCore.Instance;
            ZUserData user = ZUserData.Instance;
            ZMessage messages = zsystem.Messages;
            ZMap map = zsystem.Map;
            ZProperties properties = zsystem.Properties;
            foreach (var rule in zsystem.Rules.Rules)
            {
                if (rule.Evaluate())
                {
                    map.Cursor = core.MapId;
                    ZCore.ZCommand c = new ZCore.ZCommand();
                    ZAudio audio = zsystem.Audio;
                    while ((c = core.pop()).Cmd != ZCore.ZCommand.Command.Nop)
                    {
                        byte o = c.Operand;
                        switch (c.Cmd)
                        {
                            case ZCore.ZCommand.Command.Nop:
                                break;
                            case ZCore.ZCommand.Command.Message:
                                string s = messages.GetMessage(o);
                                if ((o & 0x80) == 0)
                                {
                                    s = map.Find(core.CmdId, core.ObjId);
                                }
                                LogArea.AppendText(s);
                                LogArea.AppendText(CRLF);
                                break;
                            case ZCore.ZCommand.Command.Sound:
                                if (properties.Attrs.PlaySound)
                                    audio!.Play(o);
                                break;
                            case ZCore.ZCommand.Command.Dialog:
                                ZDialog dialog;
                                switch (o)
                                {
                                    case 0: // boy or girl
                                        user.setFact(0, 1); // boy
                                        dialog = new ZDialog("選択", zsystem.Messages.GetMessage(0xe7), new string[] { "男子", "女子", string.Empty }) { Owner = this };
                                        if (dialog.ShowDialog() == true)
                                        {
                                            user.setFact(0, (byte)dialog.Selected);
                                            map!.Cursor = 3;
                                        }
                                        break;
                                    case 1:
                                        string[] labels = new string[] { "1", "2", "3" };
                                        string title = "セーブ";
                                        if (core.CmdId != 0x0f)
                                        {
                                            title = "ロード";
                                            int n = labels.Length;
                                            if (!File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "1.dat")))
                                            {
                                                --n;
                                                labels[0] = string.Empty;
                                            }
                                            if (!File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "2.dat")))
                                            {
                                                --n;
                                                labels[1] = string.Empty;
                                            }
                                            if (!File.Exists(System.IO.Path.Combine(zsystem.dataFolder, "3.dat")))
                                            {
                                                --n;
                                                labels[2] = string.Empty;
                                            }
                                            if (n == 0)
                                            {
                                                MessageBoxHelper.ShowCentered(this, "セーブデータが存在していません。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
                                                break;
                                            }
                                        }
                                        dialog = new ZDialog(title, zsystem.Messages.GetMessage(0xe8), labels) { Owner = this };
                                        if (dialog.ShowDialog() == true)
                                        {
                                            if (core.CmdId != 0x0f)
                                            {
                                                LoadGame(dialog.Selected);
                                            }
                                            else
                                            {
                                                SaveGame(dialog.Selected);
                                            }
                                        }
                                        break;
                                    case 2:
                                        MessageBoxHelper.ShowCentered(this, user.getItemList(), "持物", MessageBoxButton.OK, MessageBoxImage.None);
                                        break;
                                    case 3:
                                        dialog = new ZDialog("選択", zsystem.Messages.GetMessage(0xe9), new string[] { "黄", "赤", string.Empty }) { Owner = this };
                                        if (dialog.ShowDialog() == true)
                                        {
                                            if (user.getPlace(11) != 0xff)
                                            {
                                                LogArea.AppendText(zsystem.Messages.GetMessage(0xe0));
                                                LogArea.AppendText(CRLF);
                                            }
                                            if (dialog.Selected == 1 || user.getPlace(11) != 0xff)
                                            {
                                                canvas.ColorFilterType = Canvas.FilterType.Red;
                                                LogArea.AppendText(zsystem.Messages.GetMessage(0xc7));
                                                LogArea.AppendText(CRLF);
                                                LogArea.AppendText(zsystem.Messages.GetMessage(0xee));
                                                LogArea.AppendText(CRLF);
                                                GameOver();
                                                break;
                                            }
                                            user.setPlace(11, 0);
                                            map.Cursor = 74;
                                            break;
                                        }
                                        break;
                                }
                                break;
                            case ZCore.ZCommand.Command.GameOver:
                                switch (o)
                                {
                                    case 1: // teacher
                                        canvas!.ColorFilterType = Canvas.FilterType.Sepia;
                                        DrawScreen(false);
                                        break;
                                    case 2: // explosion
                                        canvas!.ColorFilterType = Canvas.FilterType.Red;
                                        DrawScreen(false);
                                        break;
                                    case 3: // clear
                                        if (properties.Attrs.PlaySound)
                                            audio!.Play(0);
                                        ZRoll endroll = new ZRoll { Owner = this, Credits = credits, TotalDuration = TimeSpan.FromSeconds(35) };
                                        endroll.Completed += (s, e) =>
                                        {
                                            DrawScreen(true);
                                        };
                                        endroll.ShowCredits();
                                        break;
                                }
                                GameOver();
                                break;
                        }
                    }
                    if (zsystem.Status == GameStatus.GameOver) return;
                    LogArea.AppendText(messages.GetMessage(0xed)); // Ｏ．Ｋ．
                    LogArea.AppendText(CRLF);
                    okay = true;
                    break;
                }
            }
            map!.Cursor = core.MapId;
            if (!okay)
            {
                string s = map.Find(core.CmdId, core.ObjId);
                if (string.IsNullOrEmpty(s))
                {
                    s = messages.GetMessage(0xec); // ダメ
                }
                LogArea.AppendText(s);
                LogArea.AppendText(CRLF);
            }
            if (map.Cursor == 74)
            {
                int msg_id = 0;
                user.setFact(13, (byte)(user.getFact(13) + 1));
                switch (user.getFact(13))
                {
                    case 4: msg_id = 0xe2; break;
                    case 6: msg_id = 0xe3; break;
                    case 10: msg_id = 0xe4; break;
                }
                if (msg_id != 0)
                {
                    LogArea.AppendText(messages.GetMessage(msg_id));
                    LogArea.AppendText(CRLF);
                }
            }
        }

        private void Menu_Quit_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            zsystem.Quit();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ZPreferences() { Owner = this };
            if (dialog.ShowDialog() == true)
            {

            }
        }

        private void Help_About_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ZAboutDialog() { Owner = this };
            dialog.ShowDialog();
        }
    }

    public static class MessageBoxHelper
    {
        // Win32 API 定義
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_CBT = 5;
        private const int HCBT_ACTIVATE = 5;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private static IntPtr _hook;
        private static HookProc _hookProc;

        public static MessageBoxResult ShowCentered(Window owner, string message, string caption,
            MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            // オーナーウィンドウのハンドル取得
            var ownerHandle = new WindowInteropHelper(owner).Handle;

            _hookProc = (nCode, wParam, lParam) =>
            {
                if (nCode == HCBT_ACTIVATE)
                {
                    // MessageBox の位置を計算して移動
                    GetWindowRect(ownerHandle, out RECT ownerRect);
                    GetWindowRect(wParam, out RECT msgRect);

                    int ownerWidth = ownerRect.Right - ownerRect.Left;
                    int ownerHeight = ownerRect.Bottom - ownerRect.Top;
                    int msgWidth = msgRect.Right - msgRect.Left;
                    int msgHeight = msgRect.Bottom - msgRect.Top;

                    int x = ownerRect.Left + (ownerWidth - msgWidth) / 2;
                    int y = ownerRect.Top + (ownerHeight - msgHeight) / 2;

                    SetWindowPos(wParam, IntPtr.Zero, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);

                    UnhookWindowsHookEx(_hook);
                }
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            };

            // フックをセット
            _hook = SetWindowsHookEx(WH_CBT, _hookProc, IntPtr.Zero, GetCurrentThreadId());

            // MessageBox を表示
            return MessageBox.Show(owner, message, caption, buttons, icon);
        }
    }

}
