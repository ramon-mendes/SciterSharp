using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using SciterSharp;

namespace PublishNuget
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("SciterSharp: " + LibVersion.AssemblyVersion);

			if(Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Environment.CurrentDirectory += "/../../../SciterSharp";

				SpawnProcess("msbuild", "SciterSharpOSX.csproj /t:Clean,Build /p:Configuration=Release");

				string nuspec = File.ReadAllText("SciterSharpOSX.nuspec");
				nuspec = Regex.Replace(nuspec,
							  "<version>.*?</version>",
							  "<version>" + LibVersion.AssemblyVersion + "</version>",
							  RegexOptions.None);
				File.WriteAllText("SciterSharpOSX.nuspec", nuspec);

				SpawnProcess("nuget", "pack SciterSharpOSX.nuspec");
				SpawnProcess("nuget", "push SciterSharpOSX." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");
			}
			else
			{
				var path = Environment.GetEnvironmentVariable("PATH");
				Environment.SetEnvironmentVariable("PATH", path + @";C:\Windows\Microsoft.NET\Framework64\v4.0.30319\");
				Environment.CurrentDirectory = @"D:\ProjetosSciter\SciterSharp\SciterSharp";

				SpawnProcess("msbuild", "SciterSharpWindows.csproj /t:Clean,Build /p:Configuration=Release");
				SpawnProcess("nuget", "pack SciterSharpWindows.csproj -Prop Configuration=Release");
				SpawnProcess("nuget", "push SciterSharpWindows." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");

				SpawnProcess("msbuild", "SciterSharpGTK.csproj /t:Clean,Build /p:Configuration=Release");
				SpawnProcess("nuget", "pack SciterSharpGTK.csproj -Prop Configuration=Release");
				SpawnProcess("nuget", "push SciterSharpGTK." + LibVersion.AssemblyVersion + ".nupkg " + NugetKeys.MIDI + " -Source nuget.org");
			}

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