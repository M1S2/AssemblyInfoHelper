using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AssemblyInfoHelper.GitHub
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
            // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            string url = e.Uri.AbsoluteUri;
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });

            e.Handled = true;
        }
    }
}
