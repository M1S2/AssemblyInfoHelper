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
using Neo.Markdig.Xaml;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Threading;

namespace AssemblyInfoHelper.Markdown
{
    /// <summary>
    /// Interaktionslogik für MarkdownViewer.xaml
    /// </summary>
    public partial class MarkdownViewer : RichTextBox
    {
#warning Images not shown in FlowDocument (Batches) (Image size = 0 ?)
#warning Loading markdown with images is very slow

        /// <summary>
        /// Markdown that is displayed in this control
        /// </summary>
        public string MarkdownString
        {
            get { return (string)this.GetValue(MarkdownStringProperty); }
            set { this.SetValue(MarkdownStringProperty, value); }
        }
        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register("MarkdownString", typeof(string), typeof(MarkdownViewer), new PropertyMetadata("", MarkdownStringChanged));

        public MarkdownViewer()
        {
            InitializeComponent();
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Create a new FlowDocument whenever the MarkdownString property changed. The document is created in an separate thread to keep the UI reactive.
        /// </summary>
        /// see: https://stackoverflow.com/questions/5579415/wpf-richtextbox-document-creation-threading-issue
        /// see: https://turecki.net/flowdocument-from-a-different-thread
        private static void MarkdownStringChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer viewer = (MarkdownViewer)sender;

            string markdownString = viewer.MarkdownString;
            if (markdownString == null) { return; }

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            FlowDocument doc = MarkdownXaml.ToFlowDocument(markdownString, pipeline);
            viewer.Document = doc;
        }

        //********************************************************************************************************************************************************************

        /// <summary>
        /// Open links in the users default browser
        /// </summary>
        /// see: https://stackoverflow.com/questions/15847822/opening-web-browser-click-in-default-browser
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                System.Diagnostics.Process.Start(e.Parameter.ToString());
            }
        }

    }
}
