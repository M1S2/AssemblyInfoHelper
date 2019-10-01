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

namespace AssemblyInfoHelper
{
    /// <summary>
    /// Interaktionslogik für MarkdownViewer.xaml
    /// </summary>
    public partial class MarkdownViewer : RichTextBox
    {
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

        private static void MarkdownStringChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string markdownString = ((MarkdownViewer)sender).MarkdownString;
            if(markdownString == null) { return; }
            FlowDocument doc = MarkdownXaml.ToFlowDocument(markdownString, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            //string docXaml = MarkdownXaml.ToXaml(markdownString, new MarkdownPipelineBuilder().UseXamlSupportedExtensions().Build());
            ((MarkdownViewer)sender).Document = doc;
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
