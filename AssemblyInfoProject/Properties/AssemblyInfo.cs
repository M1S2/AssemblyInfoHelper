using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("AssemblyInfoProject")]
[assembly: AssemblyDescription("Description:\n" +
                                "Usage of the AssemblyInfoHelper DLL:\n\n" + 
                                "Simply include the AssemblyInfoHelper.dll in your project and open the FormAssemblyInfo as follows:\n\n" +
                                "   >> AssemblyInfoHelper.FormAssemblyInfo form = new AssemblyInfoHelper.FormAssemblyInfo();\n" +
                                "   >> form.ShowDialog();\n\n" +
                                "The form displays all infos that are in the AssemblyInfo.cs file of the project in wich the DLL is included.\n" +
                                "For each change log entry use the \"AssemblyInfoHelper.AssemblyChangeLog\" attribute.")]
[assembly: AssemblyInfoHelper.AssemblyKnownIssues("-Issue1\n\n-Issue2")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(5, 0, "-I\n-J\n-K")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(5, 1, "-L\n-M\n-N")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(4, 2, "-F\n-G\n-H")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(33, 891, "-O\n-P\n-Q")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(2, 0, "-D\n-E")]
[assembly: AssemblyInfoHelper.AssemblyChangeLog(1, 0, "-A\n-B\n-C")]
[assembly: AssemblyCompany("Markus Scheich")]
[assembly: AssemblyProduct("AssemblyInfoProject")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("Trademark")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten.  Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("5e8ae988-f04a-43c9-9d28-2d3290ce72df")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.3.4")]
[assembly: AssemblyFileVersion("1.0.0.0")]
