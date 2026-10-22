using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SciterSharp;

namespace PublishNuget
{
	class MainClass
	{
		const string PackageId = "SciterSharpJS";// must match <PackageId> in SciterSharp.csproj

		public static void Main(string[] args)
		{
			Console.WriteLine("SciterSharp: " + LibVersion.AssemblyVersion);

			// SciterSharp.csproj targets net8.0 (Windows + Linux) and net8.0-windows (WinForms extras);
			// a single package holds both libs plus the runtimes/<rid>/native Sciter binaries.
			// TODO macOS: OSX is not part of the unified project yet
			Environment.CurrentDirectory += "/../../../SciterSharp";

			SpawnProcess("dotnet", "pack SciterSharp.csproj -c Release -o bin/nupkg -p:Version=" + LibVersion.AssemblyVersion);
			SpawnProcess("dotnet", "nuget push bin/nupkg/" + PackageId + "." + LibVersion.AssemblyVersion + ".nupkg --api-key " + NugetKeys.MIDI + " --source https://api.nuget.org/v3/index.json");

			File.WriteAllText("version.json", "{ \"version\": \"" + LibVersion.AssemblyVersion + "\" }"); // latest version for shields.io
		}

		static void SpawnProcess(string exe, string args, bool ignore_error = false, bool wait = true)
		{
			var startInfo = new ProcessStartInfo(exe, args)
			{
				FileName = exe,
				Arguments = args,
				UseShellExecute = false
			};

			var p = Process.Start(startInfo);
			if(wait)
			{
				p.WaitForExit();

				if(p.ExitCode != 0 && ignore_error == false)
				{
					Console.ForegroundColor = ConsoleColor.Red;

					string msg = exe + ' ' + args;
					Console.WriteLine("");
					Console.WriteLine("-------------------------");
					Console.WriteLine("FAILED: " + msg);
					Console.WriteLine("EXIT CODE: " + p.ExitCode);
					Console.WriteLine("Press ENTER to exit");
					Console.WriteLine("-------------------------");

					Console.ReadLine();
					Environment.Exit(0);
				}
			}
		}
	}
}