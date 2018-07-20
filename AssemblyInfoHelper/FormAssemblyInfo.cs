using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Markdig;
using System.IO;

namespace AssemblyInfoHelper
{
    public partial class FormAssemblyInfo : Form
    {
        private string _readmePath;
        private string _changeLogPath;

        /// <summary>
        /// Show the FormAssemblyInfo and get the README.md and CHANGELOG.md files from the given paths.
        /// </summary>
        /// <param name="readmePath">Path for the README.md file.</param>
        /// <param name="changeLogPath">Path for the CHANGELOG.md file.</param>
        public FormAssemblyInfo(string readmePath, string changeLogPath)
        {
            InitializeComponent();
            _readmePath = readmePath;
            _changeLogPath = changeLogPath;
        }

        /// <summary>
        /// Show the FormAssemblyInfo and get the readme and changelog content from the README.md and CHANGELOG.md files in the same folder as the executable. (Application.StartupPath)
        /// </summary>
        public FormAssemblyInfo()
        {
            InitializeComponent();
            _readmePath = Application.StartupPath + @"\README.md";
            _changeLogPath = Application.StartupPath + @"\CHANGELOG.md";
        }

        /// <summary>
        /// Load all controls with the corresponding values
        /// </summary>
        private void FormAssemblyInfo_Load(object sender, EventArgs e)
        {
            this.Icon = Application.OpenForms[0].Icon;      // Get the icon of the first open form (main form)

            txt_assemblyTitle.Text = AssemblyInfoHelperClass.AssemblyTitle;
            txt_assemblyDescription.Text = AssemblyInfoHelperClass.AssemblyDescription.Replace("\n", Environment.NewLine);
            txt_assemblyCompany.Text = AssemblyInfoHelperClass.AssemblyCompany;
            txt_assemblyProduct.Text = AssemblyInfoHelperClass.AssemblyProduct;
            txt_assemblyCopyright.Text = AssemblyInfoHelperClass.AssemblyCopyright;
            txt_assemblyTrademark.Text = AssemblyInfoHelperClass.AssemblyTrademark;
            txt_assemblyCulture.Text = AssemblyInfoHelperClass.AssemblyCulture;
            txt_assemblyVersion.Text = AssemblyInfoHelperClass.AssemblyVersion;
            txt_assemblyFileVersion.Text = AssemblyInfoHelperClass.AssemblyFileVersion;
            txt_assemblyLastBuild.Text = AssemblyInfoHelperClass.AssemblyLinkerTime.ToString();

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            string readmeText = "<font face = \"calibri\">";
            string changelogText = "<font face = \"calibri\">";

            if (File.Exists(_readmePath))
            {
                string readme = Markdig.Markdown.ToHtml(File.ReadAllText(_readmePath), pipeline);
                readmeText += Markdig.Markdown.ToHtml(File.ReadAllText(_readmePath), pipeline);
            }
            else
            {
                readmeText += "No readme file found in: <br><br>" + _readmePath;
            }

            if (File.Exists(_changeLogPath))
            {
                changelogText += Markdig.Markdown.ToHtml(File.ReadAllText(_changeLogPath), pipeline);
            }
            else
            {
                changelogText += "No changelog file found in: <br><br>" + Environment.NewLine + _changeLogPath;
            }

            webBrowser_Readme.DocumentText = readmeText;
            webBrowser_ChangeLog.DocumentText = changelogText;
        }
    }
}
