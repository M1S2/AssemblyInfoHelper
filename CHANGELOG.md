# AssemblyInfoHelper

## [v5.3.1]

- Bump Microsoft.Windows.Compatibility in AssemblyInfoHelper and AssemblyInfoHelper.Demo to fix a security vulnerability

## [v5.3.0]

- Added feature to persist files during update

## [v5.2.2]

- Updated MdXaml to 1.16.1 to improve Readme viewer, added MdXaml.Plugins, MdXaml.Html and MdXaml.Svg

## [v5.2.1]

- Fixed hanging application when closed during update (avoid Environment.Exit() call) (#12)
- Fixed non-working links to repository URL and tags (#11)

## [v5.2.0]

- Corrected encoding for WebBrowserMarkdown control to show german special characters
- Fixed non-working links in WebBrowserMarkdown control

## [v5.1.0]

- Removed support for AssemblyCulture attribute
- Hide empty attribute values
- Extended Readme

## [v5.0.1]

- Added some missing localizations

## [v5.0.0]

- Added localization for UI
- The library can now target net452, net472, netcoreapp3.1 and net6.0
- Improved markdown visualization of Changelog and GitHub release notes

## [v4.3.1] - 19.06.2020 19:24

- Updated MahApps.Metro to v2.0.1
- Updated MahApps.Metro.IconPacks to v4.2.0
- Small bug fixes

## [v4.3.0] - 04.05.2020 22:18

- Update feature added

## [v4.2.1] - 13.12.2019 20:01
 
- Unnecessary dependencies removed
- Color of Expander arrow in GitHub Releases Tab corrected (was always black)

## [v4.2.0] - 05.11.2019 22:04

- Show release notes
- AppInfoButton control added for simpler usage of the AssemblyInfoHelper
- Switch added to disable notification of new versions
- Show release type (major, minor, patch) in tag icon

## [v4.1.1] - 27.08.2019 21:39

- GitHub release tab now doesn't crash without internet
- AssemblyInfoHelperClass can also be used to get attributes from any assembly now

## [v4.1.0] - 22.08.2019 22:05

- New tab added for GitHub releases
- Image display in Readme improved

## [v4.0.0] - 08.03.2019 16:39

- Using MahApps.Metro to get a Metro look for the WPF window

## [v3.0.0] - 09.09.2018 23:03

- This version contains a WPF window and should be used with WPF applications.

## [v2.0.0] - 29.07.2018 14:32
### Removed
- AssemblyKnownIssues attribute removed. Report known issues in README.md
- AssemblyChangeLog attribute removed. Use the CHANGELOG.md file instead.

### Added
- Use README.md file for description of the project.
- Use CHANGELOG.md file for changelog of the project.
- Markdig for displaying markdown syntax.

## [1.0.0]
This version of the AssemblyInfoHelper uses
- the AssemblyDescription attribute for the description of the project.
- a new attribute "AssemblyKnownIssues" for known issues.
- a new attribute "AssemblyChangeLog" for change log entries.
