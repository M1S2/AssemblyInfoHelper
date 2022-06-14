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

namespace AssemblyInfoHelper.GitHub
{
    /// <summary>
    /// Interaktionslogik für GitHubReleasesControl.xaml
    /// </summary>
    public partial class GitHubReleasesControl : UserControl
    {
        MdXaml.Markdown md;             // Dummy object from MdXaml library to force output of DLL to build directory.

        public GitHubReleasesControl()
        {
            InitializeComponent();
        }
    }
}
