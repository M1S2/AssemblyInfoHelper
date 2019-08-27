using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AssemblyInfoHelper.GitHubReleases
{
    /// <summary>
    /// Opens <see cref="Hyperlink.NavigateUri"/> in a default system browser
    /// </summary>
    /// see: https://stackoverflow.com/questions/10238694/example-using-hyperlink-in-wpf
    public class ExternalBrowserHyperlinkControl : Hyperlink
    {
        public ExternalBrowserHyperlinkControl()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
