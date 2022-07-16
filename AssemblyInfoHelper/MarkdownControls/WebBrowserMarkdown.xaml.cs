﻿using System;
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
using System.Diagnostics;

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
            Stream htmlStream = new MemoryStream(System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(htmlString));      // Convert to stream with encoding ISO-8859-1 (Latin1) to show german ä,ö,ü correctly
            ((WebBrowserMarkdown)sender).webBrowserMarkdown.NavigateToStream(htmlStream);       //NavigateToString(htmlString);
        }
            
        //********************************************************************************************************************************************************************

        /// <summary>
        /// Open links in the users default browser
        /// </summary>
        private void webBrowserMarkdown_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;

                // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                string url = e.Uri.AbsoluteUri;
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
        }
    }
}
