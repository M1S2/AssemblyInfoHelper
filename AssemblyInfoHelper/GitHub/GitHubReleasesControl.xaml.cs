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

namespace AssemblyInfoHelper.GitHub
{
    /// <summary>
    /// Interaktionslogik für GitHubReleasesControl.xaml
    /// </summary>
    public partial class GitHubReleasesControl : UserControl
    {
        // You must use any type of Markdig.Wpf to get the Markdig.Wpf.dll built in the output directory of the project that is using this library.
        // Only using the MarkdownViewer in XAML isn't enough to get the Markdig.Wpf.dll copied correctly. 
        // This results in partial binding information errors.
        // see: https://stackoverflow.com/questions/15816769/dependent-dll-is-not-getting-copied-to-the-build-output-folder-in-visual-studio
        //private ComponentResourceKey dummyStyle = Markdig.Wpf.Styles.Heading1StyleKey;

        public GitHubReleasesControl()
        {
            InitializeComponent();
        }

        /*
        //see: https://social.msdn.microsoft.com/Forums/vstudio/en-US/eb455469-1c44-4f8a-ac6e-3892be16c99e/parent-wpf-listbox-doesnt-scroll-when-the-mouse-cursor-is-over-a-item-of-its-listbox-inside-?forum=wpf
        private void MarkdownViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = FindParent<ScrollViewer>(sender as DependencyObject);
            if (scrollViewer != null)
            {
                if (e.Delta < 0)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 30);
                }
                else if (e.Delta > 0)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 30);
                }
            }
        }

        /// <summary>
        /// Find the parent of child with the specified type T
        /// </summary>
        /// <typeparam name="T">Type of parent that is searched</typeparam>
        /// <param name="child">Start point of search</param>
        /// <returns>The first parent of child with the specified type</returns>
        /// see: https://social.msdn.microsoft.com/Forums/vstudio/en-US/c47754bd-38c7-40b3-b64a-38a48884fde8/how-to-find-a-parent-of-a-specific-type?forum=wpf
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if ((parent as T) != null) { return parent as T; }
            else { return FindParent<T>(parent); }
        }
        */
    }
}
