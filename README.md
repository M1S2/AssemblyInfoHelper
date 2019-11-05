# AssemblyInfoHelper

[![Nuget Version](https://img.shields.io/nuget/v/AssemblyInfoHelper.svg)](https://www.nuget.org/packages/AssemblyInfoHelper/)
![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/AssemblyInfoHelper)
[![GitHub License](https://img.shields.io/github/license/M1S2/AssemblyInfoHelper)](LICENSE.md)
![Nuget Downloads](https://img.shields.io/nuget/dt/AssemblyInfoHelper)

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

GitHub releases are taken from repository at the url given by the `GitHubRepo` attribute (see usage below). 

The **AssemblyInfoProject** is used to test the AssemblyInfoHelper.

## Installation

Include the [latest release from nuget.org](https://www.nuget.org/packages/AssemblyInfoHelper/) in your project.

You can also use the Package Manager console with: `PM> Install-Package AssemblyInfoHelper`

## Usage

To show all releases from GitHub add the `GitHubRepo` attribute to the AssemblyInfo.cs file: 

```
[assembly: AssemblyInfoHelper.GitHubReleases.GitHubRepo("https://github.com/M1S2/AssemblyInfoHelper")]
```


The simplest way to show the WindowAssemblyInfo is to add a `AppInfoButton` control to the application. Everything is done inside this control.

```
xmlns:assemblyInfoHelper="clr-namespace:AssemblyInfoHelper;assembly=AssemblyInfoHelper"
...
<assemblyInfoHelper:AppInfoButton EnableNewVersionNotification="True"/>
```


Or you can open the info window with: 

```
AssemblyInfoHelper.WindowAssemblyInfo window = new AssemblyInfoHelper.WindowAssemblyInfo();
window.ShowDialog();
```