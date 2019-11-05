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
        /// <summary>
        /// Markdown that is displayed in this control
        /// </summary>
        public string MarkdownString
        {
            get { return (string)this.GetValue(MarkdownStringProperty); }
            set { this.SetValue(MarkdownStringProperty, value); }
        }
        public static readonly DependencyProperty MarkdownStringProperty = DependencyProperty.Register("MarkdownString", typeof(string), typeof(MarkdownViewer), new PropertyMetadata("", MarkdownStringChanged));


        public bool DocumentIsLoading
        {
            get { return (bool)this.GetValue(DocumentIsLoadingProperty); }
            private set { this.SetValue(DocumentIsLoadingProperty, value); }
        }
        public static readonly DependencyProperty DocumentIsLoadingProperty = DependencyProperty.Register("DocumentIsLoading", typeof(bool), typeof(MarkdownViewer), new PropertyMetadata(false));


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
        private async static void MarkdownStringChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer viewer = (MarkdownViewer)sender;

            string markdownString = viewer.MarkdownString;
            if (markdownString == null) { return; }

            viewer.DocumentIsLoading = true;

            FlowDocument doc = new FlowDocument();
            UIElement loadingTemplate = (UIElement)viewer.FindResource("LoadingTemplate");
            doc.Blocks.Add(new BlockUIContainer(loadingTemplate));
            viewer.Document = doc;

            //doc.Blocks.Add(new Paragraph(new Hyperlink(new Run("Link"))));
            //viewer.Document = doc;

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            SemaphoreSlim syncEvent = new SemaphoreSlim(0,1);
            Thread thread = new Thread(() => 
            {
                doc = MarkdownXaml.ToFlowDocument(markdownString, pipeline);
                //doc.Blocks.Add(new BlockUIContainer(new Button() { Content = "ABC" }));
                //string xaml = MarkdownXaml.ToXaml(markdownString, pipeline);
                syncEvent.Release();
            });
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.IsBackground = true;
            await syncEvent.WaitAsync();

            //foreach (Block block in doc.Blocks)
            //{
            //    List<Hyperlink> hyperLinks = block.FindChildren<Hyperlink>().ToList();
            //}
            //List<Image> images = FindImages(doc).ToList();

#warning Hyperlink is disabled when the document is created in new Thread (without the thread, the links are enabled)
#warning Freezable object, HostVisual ?
#warning Even if the document is created without an extra thread, the IsEnabled and IsEnabledCore properties of the Hyperlink are False (and the link works) ???!!!

            doc.AttachToCurrentThread();
            //doc.EnableAllHyperlinks();
            //doc.IsEnabled = true;
            viewer.Document = doc;
            //viewer.Document.Blocks.Clear();
            //viewer.Document.Blocks.AddRange(doc.Blocks);
            viewer.DocumentIsLoading = false;
        }





        //********************************************************************************************************************************************************************

        //https://blogs.msdn.microsoft.com/dwayneneed/2007/04/26/multithreaded-ui-hostvisual/
        //https://stackoverflow.com/questions/11358647/how-to-access-separate-thread-generated-wpf-ui-elements-from-the-dispatcher-thre

        private async static void MarkdownStringChanged2(object sender, DependencyPropertyChangedEventArgs e)
        {
            MarkdownViewer viewer = (MarkdownViewer)sender;

            string markdownString = viewer.MarkdownString;
            if (markdownString == null) { return; }

            viewer.DocumentIsLoading = true;

            FlowDocument doc = new FlowDocument();
            UIElement loadingTemplate = (UIElement)viewer.FindResource("LoadingTemplate");
            doc.Blocks.Add(new BlockUIContainer(loadingTemplate));
            viewer.Document = doc;

#warning Test
            ResourceKey key = MarkdownXaml.CodeBlockStyleKey;
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();


            viewer.CreateElementOnSeperateThread2(() =>
            {
                //return new FlowDocument(); 
                return new FlowDocument(new Paragraph(new Run("Hallo")));
                //return MarkdownXaml.ToFlowDocument(markdownString, pipeline);
            }, 
            result =>
            {
                viewer.Document = (FlowDocument)result;
                viewer.DocumentIsLoading = false;
            });

            /*viewer.CreateElementOnSeperateThread2(() =>
            {
                return new TextBlock(new Run("ABC"));
            },
            result =>
            {
                doc.Blocks.Add(new BlockUIContainer((UIElement)result));
                viewer.Document = doc;
            });*/


            //HostVisual hostVisual = new HostVisual();

            //            // Spin up a worker thread, and pass it the HostVisual that it should be part of.
            //            AutoResetEvent s_event = new AutoResetEvent(false);
            //            Thread thread = new Thread(new ParameterizedThreadStart((arg) => 
            //            {
            //#warning ...
            //            }));
            //            thread.SetApartmentState(ApartmentState.STA);
            //            thread.IsBackground = true;
            //            thread.Start(hostVisual);

            //            // Wait for the worker thread to spin up and create the VisualTarget.
            //            s_event.WaitOne();
            //            return hostVisual;


            //System.Threading.SemaphoreSlim syncEvent = new System.Threading.SemaphoreSlim(0, 1);
            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
            //    doc = MarkdownXaml.ToFlowDocument(markdownString, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            //    //string xaml = MarkdownXaml.ToXaml(markdownString, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
            //    syncEvent.Release();
            //});
            //thread.SetApartmentState(System.Threading.ApartmentState.STA); //Set the thread to STA
            //thread.Start();
            //thread.IsBackground = true;
            //await syncEvent.WaitAsync();


#warning Hyperlink is disabled when the document is created in new Thread (without the thread, the links are enabled)
#warning Freezable object, HostVisual ?

            //doc.AttachToCurrentThread();
            //doc.EnableAllHyperlinks();
            //doc.IsEnabled = true;
            //viewer.Document = doc;
            //viewer.Document.Blocks.Clear();
            //viewer.Document.Blocks.AddRange(doc.Blocks);
            //viewer.DocumentIsLoading = false;
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


        /// <summary>
        /// Creates UI element on a seperate thread and transfers it to
        /// main UI thread. 
        /// 
        /// Usage; if you have complex UI operation that takes a lot of time, such as XPS object creation.
        /// </summary>
        /// <param name="constructObject">Function that creates the necessary DependencyObject - will be executed on new thread</param>
        /// <param name="constructionCompleted">Callback to the function that receives the constructed object.</param>
        /// see: https://stackoverflow.com/questions/11358647/how-to-access-separate-thread-generated-wpf-ui-elements-from-the-dispatcher-thre
        public void CreateElementOnSeperateThread(Func<DependencyObject> constructObject, Action<DependencyObject> constructionCompleted)
        {
            VerifyAccess();

            // save dispatchers for future usage.
            // we create new element on a seperate STA thread
            // and then basically swap UIELEMENT's Dispatcher.
            Dispatcher threadDispatcher = null;
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;

            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(() =>
            {
                threadDispatcher = Dispatcher.CurrentDispatcher;
                ev.Set();

                Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            ev.WaitOne();

            threadDispatcher.BeginInvoke(new Action(() =>
            {
                DependencyObject constructedObject = constructObject();
                currentDispatcher.BeginInvoke(new Action(() =>
                {
                    if (constructedObject.GetType() == typeof(FlowDocument))
                    {
                        ((FlowDocument)constructedObject).AttachToCurrentThread();
                    }
                    else
                    {
                        FieldInfo fieldinfo = typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldinfo != null) { fieldinfo.SetValue(constructedObject, currentDispatcher); }
                    }

                    constructionCompleted(constructedObject);
                    threadDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
                }), DispatcherPriority.Normal);
            }), DispatcherPriority.Normal);
        }



        public void CreateElementOnSeperateThread2(Func<DependencyObject> constructObject, Action<DependencyObject> constructionCompleted)
        {
            VerifyAccess();

            // save dispatchers for future usage.
            // we create new element on a seperate STA thread
            // and then basically swap UIELEMENT's Dispatcher.
            Dispatcher threadDispatcher = null;
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;

            AutoResetEvent ev = new AutoResetEvent(false);
            Thread thread = new Thread(() =>
            {
                threadDispatcher = Dispatcher.CurrentDispatcher;
                DependencyObject constructedObject = constructObject();
                currentDispatcher.BeginInvoke(new Action(() =>
                {
                    if (constructedObject.GetType() == typeof(FlowDocument))
                    {
                        ((FlowDocument)constructedObject).AttachToCurrentThread();
                    }
                    else
                    {
                        FieldInfo fieldinfo = typeof(DispatcherObject).GetField("_dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fieldinfo != null) { fieldinfo.SetValue(constructedObject, currentDispatcher); }
                    }

                    constructionCompleted(constructedObject);
                }), DispatcherPriority.Normal);

                ev.Set();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            ev.WaitOne();
        }


        #region Not used
        /*
        //see: https://stackoverflow.com/questions/3899852/finding-all-images-in-a-flowdocument
        public static IEnumerable<Image> FindImages(FlowDocument document)
        {
            return document.Blocks.SelectMany(block => FindImages(block));
        }

        public static IEnumerable<Image> FindImages(Block block)
        {
            if (block is Table)
            {
                return ((Table)block).RowGroups
                    .SelectMany(x => x.Rows)
                    .SelectMany(x => x.Cells)
                    .SelectMany(x => x.Blocks)
                    .SelectMany(innerBlock => FindImages(innerBlock));
            }
            if (block is Paragraph)
            {
                return ((Paragraph)block).Inlines
                    .OfType<InlineUIContainer>()
                    .Where(x => x.Child is Image)
                    .Select(x => x.Child as Image);
            }
            if (block is BlockUIContainer)
            {
                Image i = ((BlockUIContainer)block).Child as Image;
                return i == null
                            ? new List<Image>()
                            : new List<Image>(new[] { i });
            }
            if (block is List)
            {
                return ((List)block).ListItems.SelectMany(listItem => listItem
                                                                      .Blocks
                                                                      .SelectMany(innerBlock => FindImages(innerBlock)));
            }
            throw new InvalidOperationException("Unknown block type: " + block.GetType());
        }*/
        #endregion
    }
}
