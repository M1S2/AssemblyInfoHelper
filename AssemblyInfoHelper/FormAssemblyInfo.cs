using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyInfoHelper
{
    public partial class FormAssemblyInfo : Form
    {
        public FormAssemblyInfo()
        {
            InitializeComponent();
        }

        private void FormAssemblyInfo_Load(object sender, EventArgs e)
        {
            this.Icon = Application.OpenForms[0].Icon;      // Get the icon of the first open form (main form)

            txt_assemblyTitle.Text = AssemblyInfoHelperClass.AssemblyTitle;
            txt_assemblyCompany.Text = AssemblyInfoHelperClass.AssemblyCompany;
            txt_assemblyProduct.Text = AssemblyInfoHelperClass.AssemblyProduct;
            txt_assemblyCopyright.Text = AssemblyInfoHelperClass.AssemblyCopyright;
            txt_assemblyTrademark.Text = AssemblyInfoHelperClass.AssemblyTrademark;
            txt_assemblyCulture.Text = AssemblyInfoHelperClass.AssemblyCulture;
            txt_assemblyVersion.Text = AssemblyInfoHelperClass.AssemblyVersion;
            txt_assemblyFileVersion.Text = AssemblyInfoHelperClass.AssemblyFileVersion;
            txt_assemblyLastBuild.Text = AssemblyInfoHelperClass.AssemblyLinkerTime.ToString();

            txt_assemblyDescription.Text = AssemblyInfoHelperClass.AssemblyDescription.Replace("\n", Environment.NewLine);
            txt_assemblyChangeLog.Text = AssemblyInfoHelperClass.AssemblyChangeLogString.Replace("\n", Environment.NewLine);
            txt_assemblyKnownIssues.Text = AssemblyInfoHelperClass.AssemblyKnownIssues.Replace("\n", Environment.NewLine);
        }
    }
}
