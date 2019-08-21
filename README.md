# AssemblyInfoHelper

Version: %version%

[![nuget](https://img.shields.io/nuget/v/AssemblyInfoHelper.svg)](https://www.nuget.org/packages/AssemblyInfoHelper/)

## Purpose

The **AssemblyInfoHelper** gets and displays the assembly info of the assembly that calls this functions.
This contains the following informations:
- AssemblyTitle
- AssemblyCompany
- AssemblyProduct
- AssemblyCopyright
- AssemblyTrademark
- AssemblyCulture
- AssemblyVersion
- AssemblyFileVersion
- AssemblyLinkerTime

The description is get from the README.md file in the path given when creating the WindowAssemblyInfo.

The changelog is get from the CHANGELOG.md file in the path given when creating the WindowAssemblyInfo.

The **AssemblyInfoProject** is used to test the AssemblyInfoHelper.

## Installation

Include the [latest release](https://www.nuget.org/packages/AssemblyInfoHelper/) from nuget.org in your project.

You can also use the Package Manager console with: `PM> Install-Package AssemblyInfoHelper`

## Usage

Open the info window with: 

```
AssemblyInfoHelper.WindowAssemblyInfo window = new AssemblyInfoHelper.WindowAssemblyInfo();
window.ShowDialog();
```

To show all releases from GitHub add the `GitHubRepo` attribute to the AssemblyInfo.cs file: 

```
[assembly: AssemblyInfoHelper.GitHubReleases.GitHubRepo("https://github.com/M1S2/AssemblyInfoHelper")]
```