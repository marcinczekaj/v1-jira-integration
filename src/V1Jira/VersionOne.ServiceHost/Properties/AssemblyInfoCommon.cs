/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("VersionOne, Inc.")]
[assembly: AssemblyCopyright("Copyright � 2011, VersionOne, Inc. All rights reserved.")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyInformationalVersion("1.4.0.0")]

#if DEBUG
[assembly: AssemblyDescription("Debug")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyDescription("Release")]
[assembly: AssemblyConfiguration("Release")]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("..\\..\\Common\\SigningKey\\VersionOne.snk")]
[assembly: AssemblyKeyName("")]


#endif

[assembly: ComVisible(false)]


