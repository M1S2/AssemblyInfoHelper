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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Markdig;
using System.IO;

namespace AssemblyInfoHelper.MarkdownControls
{
    /// <summary>
    /// Interaktionslogik für WebBrowserMarkdown.xaml
    /// </summary>
    public partial class WebBrowserMarkdown : UserControl
    {
        public string MarkdownString
        {
            get { return (string)this.GetValue(MarkdownStringProperty); }
            set { this.SetValue(MarkdownStringProperty, value); }
        }
        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register("MarkdownString", typeof(string), typeof(WebBrowserMarkdown), new PropertyMetadata("", MarkdownStringChanged));

        public WebBrowserMarkdown()
        {
            InitializeComponent();
        }

        //********************************************************************************************************************************************************************

        private static void MarkdownStringChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string markdownString = ((WebBrowserMarkdown)sender).MarkdownString;
            if (markdownString == null) { return; }
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string htmlString = "<font face = \"calibri\">" + Markdig.Markdown.ToHtml(markdownString, pipeline);
            Stream htmlStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(htmlString));            // Convert to stream to show german ä,ö,ü correctly
            ((WebBrowserMarkdown)sender).webBrowserMarkdown.NavigateToStream(htmlStream);       //NavigateToString(htmlString);
        }
            
        //********************************************************************************************************************************************************************

        /// <summary>
        /// Open links in the users default browser
        /// </summary>
        /// see: https://stackoverflow.com/questions/15847822/opening-web-browser-click-in-default-browser
        private void webBrowserMarkdown_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
        }
    }
}
