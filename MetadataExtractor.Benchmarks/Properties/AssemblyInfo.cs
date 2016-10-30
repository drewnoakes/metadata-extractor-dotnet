using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MetadataExtractor.Benchmarks")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("481876d6-f047-4cc7-a419-362afc9f0eee")]

// BELOW HERE: Common attributes, inlined from SharedAssemblyInfo.cs during migration to project.json
// TODO revert back to using a shared file once project.json is replaced with csproj in VS15

[assembly: AssemblyProduct("MetadataExtractor")]

[assembly: AssemblyCompany("Drew Noakes")]
[assembly: AssemblyCopyright("Copyright © Drew Noakes 2002-2016")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#elif RELEASE
[assembly: AssemblyConfiguration("Release")]
#endif
