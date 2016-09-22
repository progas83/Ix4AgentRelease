using SimplestLogger.VisualLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ix4ServiceConfigurator.Controls
{
    public class RichTextBoxExtended : RichTextBox
    {

        public LogInfoArgs RichText
        {
            get { return (LogInfoArgs)GetValue(RichTextProperty); }
            set { SetValue(RichTextProperty, value); }
        }

        public static readonly DependencyProperty RichTextProperty =
            DependencyProperty.Register("RichText", typeof(LogInfoArgs), typeof(RichTextBoxExtended), new PropertyMetadata(new LogInfoArgs(0, string.Empty), PropertyRichTextChandedCallback, PropertyRichTextCoerceValueCallback));


        private static void PropertyRichTextChandedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object PropertyRichTextCoerceValueCallback(DependencyObject d, object baseValue)
        {
            RichTextBox rtb = d as RichTextBox;
            LogInfoArgs container = baseValue as LogInfoArgs;
            AppendText(rtb, container);
            return baseValue;
        }

        private const string RREdColor = "Red";
        public static void AppendText(RichTextBox box, LogInfoArgs container)
        {
            if (container == null || string.IsNullOrEmpty(container.Message))
                return;
            Paragraph p = box.Document.Blocks.FirstBlock as Paragraph;
            p.LineHeight = 0.1;

            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = string.Format("{0}: {1}{2}", DateTime.Now.ToString(), container.Message, Environment.NewLine);
            if (container.Status == LogStatus.Error)
            {
                BrushConverter bc = new BrushConverter();
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString(RREdColor));
            }
            box.ScrollToEnd();
        }

    }
}
