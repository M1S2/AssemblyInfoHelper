# AssemblyInfoHelper

[![GitHub Release Version](https://img.shields.io/github/v/release/M1S2/AssemblyInfoHelper)](https://github.com/M1S2/AssemblyInfoHelper/releases/latest)
[![GitHub License](https://img.shields.io/github/license/M1S2/AssemblyInfoHelper)](https://github.com/M1S2/AssemblyInfoHelper/blob/master/LICENSE.md)
[![Nuget Version](https://img.shields.io/nuget/v/AssemblyInfoHelper.svg)](https://www.nuget.org/packages/AssemblyInfoHelper/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/AssemblyInfoHelper)](https://www.nuget.org/packages/AssemblyInfoHelper/)

## Purpose

The **AssemblyInfoHelper** gets and displays the assembly attributes of the assembly that calls this functions.
This contains the following informations:
- AssemblyTitle
- AssemblyDescription
- AssemblyCompany
- AssemblyProduct
- AssemblyCopyright
- AssemblyTrademark
- AssemblyVersion
- AssemblyFileVersion (this attribute is only show if it differs from the AssemblyVersion)
- AssemblyInformationalVersion
- AssemblyCreationTime

![General Infos](https://github.com/M1S2/AssemblyInfoHelper/raw/master/Screenshots/AssemblyInfoWindow_GeneralInfos.PNG)

The readme is get from the README.md file in the path given when creating the WindowAssemblyInfo or the application startup path.

![Readme](https://github.com/M1S2/AssemblyInfoHelper/raw/master/Screenshots/AssemblyInfoWindow_Readme.PNG)

The changelog is get from the CHANGELOG.md file in the path given when creating the WindowAssemblyInfo or the application startup path.

![Changelog](https://github.com/M1S2/AssemblyInfoHelper/raw/master/Screenshots/AssemblyInfoWindow_Changelog.PNG)

GitHub releases are taken from repository at the url given by the `GitHubRepo` attribute (see usage below). 

![GitHub Releases](https://github.com/M1S2/AssemblyInfoHelper/raw/master/Screenshots/AssemblyInfoWindow_GitHubReleases.PNG)

The **AssemblyInfoHelper.Demo** is used to test the AssemblyInfoHelper.

## Installation

Include the [latest release from nuget.org](https://www.nuget.org/packages/AssemblyInfoHelper/) in your project.

You can also use the Package Manager console with: `PM> Install-Package AssemblyInfoHelper`

## Usage

To show all releases from GitHub add the `GitHubRepo` attribute to the AssemblyInfo.cs file: 

```csharp
[assembly: AssemblyInfoHelper.GitHub.GitHubRepo("https://github.com/M1S2/AssemblyInfoHelper")]
```


The simplest way to show the WindowAssemblyInfo is to add a `AppInfoButton` control to the application. Everything is done inside this control.

```csharp
xmlns:assemblyInfoHelper="clr-namespace:AssemblyInfoHelper;assembly=AssemblyInfoHelper"
...
<assemblyInfoHelper:AppInfoButton EnableNewVersionNotification="True"/>
```

![AppInfoButton](https://github.com/M1S2/AssemblyInfoHelper/raw/master/Screenshots/AppInfoButton.PNG)

Or you can open the info window with: 

```csharp
AssemblyInfoHelper.WindowAssemblyInfo window = new AssemblyInfoHelper.WindowAssemblyInfo();
window.ShowDialog();
```

## Add assembly attributes

### New style projects
Add the following properties to a .csproj file to include assembly attributes:
```csharp
<PropertyGroup>
	<GenerateAssemblyInfo>true</GenerateAssemblyInfo>
	<Title>TitleText</Title>
	<Description>DescriptionText</Description>
	<Company>CompanyText</Company>
	<Product>ProductText</Product>
	<Copyright>Copyright © 2022</Copyright>
</PropertyGroup>
```

### Old style projects
Add the following lines to the AssemblyInfo.cs file to include assembly attributes:
```csharp
[assembly: AssemblyTitle("TitleText")]
[assembly: AssemblyDescription("DescriptionText")]
[assembly: AssemblyCompany("CompanyText")]
[assembly: AssemblyProduct("ProductText")]
[assembly: AssemblyCopyright("Copyright © 2022")]
[assembly: AssemblyTrademark("TrademarkText")]
```

## Update Feature

You can see and download all releases available on GitHub on the GitHub releases tab.
To Upgrade/Repair/Downgrade click on the button beside the corresponding release. The release is downloaded from GitHub and installed automatically depending if an installer or binaries are available.

The release binaries must be added to a GitHub release as asset. The following naming conventions are used to detect, what type of asset it is:

*For binaries:*
- %ProjectName%_Binaries.zip
- %ProjectName%.zip
- %ProjectName%_v1.0.0.zip
- bin.zip

*For installer:*
- %ProjectName%_Installer.zip
- Installer.zip
- Setup.zip
- Setup.exe

### Persistent files during update

If files should be preserved during the update process, the `UpdatePersistentFiles` attribute can be added to the `AssemblyInfo.cs` file:
```csharp
[assembly: AssemblyInfoHelper.GitHub.UpdatePersistentFiles("Filename.txt")]
```
Adapt the "Filename.txt" to the file you want to keep. The filename is relative to the executing assembly. E.g. if you want to keep a database file named "Database.db" that is located beside the executable "DemoApp.exe", use "Database.db" with the `UpdateKeepFile` attribute.

If multiple files should be kept, add one `UpdatePersistentFiles` attribute for each file.
```csharp
[assembly: AssemblyInfoHelper.GitHub.UpdatePersistentFiles("Filename1.txt")]
[assembly: AssemblyInfoHelper.GitHub.UpdatePersistentFiles("Folder\\Filename2.txt")]
```

### Temporary folder used by the update feature

The %AppData%\Local\%ProjectName% folder is used to temporary save the downloaded release. Also the files that should be persisted are saved here.
Afer the update was finished, the folder content is deleted. Only the Updater.exe remains because it can't delete itself.