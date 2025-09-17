using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HHSAdvWin
{
    /// <summary>
    /// ZAboutDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ZAboutDialog : Window
    {
        public class AboutContents
        {
            public string IconImageFile
            {
                get
                {
                    return System.IO.Path.Combine(ZSystem.Instance.dataFolder, "icon.png");
                }
            }
            public string AboutText
            {
                get
                {
                    return @"<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
<Paragraph>
<Span FontSize='16' FontWeight='Bold'>High High School Adventure</Span>
</Paragraph>
<Paragraph>
PalmOS version: hiro © 2002-2004<LineBreak/>
Android version: hiro © 2011-2025<LineBreak/>
M5 version: hiro © 2023-2025<LineBreak/>
Qt version: hiro © 2024<LineBreak/>
PicoCalc version: hiro © 2025<LineBreak/>
SDL version: hiro © 2025<LineBreak/>
Windows version: hiro © 2025<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>- Project ZOBPlus -</Bold><LineBreak/>
Hayami &lt;hayami@zob.jp&gt;<LineBreak/>
Exit &lt;exit@zob.jp&gt;<LineBreak/>
ezumi &lt;ezumi@zob.jp&gt;<LineBreak/>
Ogu &lt;ogu@zob.jp&gt;<LineBreak/>
neopara &lt;neopara@zob.jp&gt;<LineBreak/>
hiro &lt;hiro@zob.jp&gt;<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>--- Original Staff ---</Bold><LineBreak/>
Directed By HIRONOBU NAKAGUCHI<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>- Graphic Designers -</Bold><LineBreak/>
</Paragraph>
<Table CellSpacing='5' BorderBrush='Black' BorderThickness='0'>
<Table.Columns>
<TableColumn/>
<TableColumn/>
<TableColumn/>
<TableColumn/>
<TableColumn/>
</Table.Columns>
<TableRowGroup>
<TableRow>
<TableCell><Paragraph>NOBUKO YANAGITA</Paragraph></TableCell>
<TableCell><Paragraph>YUMIKO HOSONO</Paragraph></TableCell>
<TableCell><Paragraph>HIRONOBU NAKAGUCHI</Paragraph></TableCell>
<TableCell><Paragraph>TOSHIHIKO YANAGITA</Paragraph></TableCell>
<TableCell><Paragraph>TOHRU OHYAMA</Paragraph></TableCell>
</TableRow>
<TableRow>
<TableCell><Paragraph>MASANORI ISHII</Paragraph></TableCell>
<TableCell><Paragraph>YASUSHI SHIGEHARA</Paragraph></TableCell>
<TableCell><Paragraph>HIDETOSHI SUZUKI</Paragraph></TableCell>
<TableCell><Paragraph>TATSUYA UCHIBORI</Paragraph></TableCell>
<TableCell><Paragraph>MASAKI NOZAWA</Paragraph></TableCell>
</TableRow>
<TableRow>
<TableCell><Paragraph>TOMOKO OHKAWA</Paragraph></TableCell>
<TableCell><Paragraph>FUMIKAZU SHIRATSUCHI</Paragraph></TableCell>
<TableCell><Paragraph>YASUNORI YAMADA</Paragraph></TableCell>
<TableCell><Paragraph>UNENORI TAKIMOTO</Paragraph></TableCell>
<TableCell><Paragraph> </Paragraph></TableCell>
</TableRow>
</TableRowGroup>
</Table>
<Paragraph>
<Bold>- Message Converters -</Bold><LineBreak/>
</Paragraph>
<Table CellSpacing='5' BorderBrush='Black' BorderThickness='0'>
<Table.Columns>
<TableColumn/>
<TableColumn/>
<TableColumn/>
<TableColumn/>
</Table.Columns>
<TableRowGroup>
<TableRow>
<TableCell><Paragraph>TATSUYA UCHIBORI</Paragraph></TableCell>
<TableCell><Paragraph>HIDETOSHI SUZUKI</Paragraph></TableCell>
<TableCell><Paragraph>YASUSHI SHIGEHARA</Paragraph></TableCell>
<TableCell><Paragraph>YASUNORI YAMADA</Paragraph></TableCell>
</TableRow>
</TableRowGroup>
</Table>
<Paragraph>
<Bold>- Floppy Disk Converters -</Bold><LineBreak/>
HIRONOBU NAKAGUCHI<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>- Music -</Bold><LineBreak/>
MASAO MIZOBE<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>- Special Thanks To -</Bold><LineBreak/>
HIROSHI YAMAMOTO / TAKAYOSHI KASHIWAGI<LineBreak/>
</Paragraph>
<Paragraph>
<Bold>- Cooperate with -</Bold><LineBreak/>
Furniture KASHIWAGI<LineBreak/>
</Paragraph>
<Paragraph>
<Span FontSize='16' FontWeight='Bold'>ZAMA HIGH SCHOOL MICRO COMPUTER CIRCLE</Span>
</Paragraph>
</FlowDocument>";
                }
            }
        }
        public ZAboutDialog()
        {
            InitializeComponent();
            var data = new AboutContents();
            icon.Source = new BitmapImage(new Uri(data.IconImageFile, UriKind.Absolute));
            //using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data.AboutText)))
            //{
            //    var textRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            //    textRange.Load(stream, DataFormats.Xaml);
            //}
            textBox.Document = (FlowDocument)XamlReader.Parse(data.AboutText);
        }

        private void Button_Okay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
