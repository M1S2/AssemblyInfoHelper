using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyInfoProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void hilfeToolStripButton_Click(object sender, EventArgs e)
        {
            AssemblyInfoHelper.FormAssemblyInfo form = new AssemblyInfoHelper.FormAssemblyInfo(Application.StartupPath + @"\..\..\..\README.md", Application.StartupPath + @"\..\..\..\CHANGELOG.md");
            //AssemblyInfoHelper.FormAssemblyInfo form = new AssemblyInfoHelper.FormAssemblyInfo();

            form.ShowDialog();
        }
    }
}
