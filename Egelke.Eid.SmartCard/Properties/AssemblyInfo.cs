using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Egelke.Eid.SmartCard")]
[assembly: AssemblyDescription("Belgium eID client in .Net")]
[assembly: AssemblyConfiguration("Beta")]
[assembly: AssemblyCompany("Egelke BVBA")]
[assembly: AssemblyProduct(".Net eID Client")]
[assembly: AssemblyCopyright("Copyright © Egelke BVBA 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("9da64448-e6aa-49ea-b8f9-0ee77139211e")]

[assembly: AssemblyVersion("0.1.0")]
[assembly: AssemblyFileVersion("0.1.0")]
[assembly: AssemblyInformationalVersion("0.1.0-Beta1")]

#if DEBUG
[assembly: AssemblyKeyFile(@"../debug.snk")]
#else
[assembly: AssemblyKeyFile(@"../release.snk")]
#endif
