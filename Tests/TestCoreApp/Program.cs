using SciterSharp;
using SciterSharp.Interop;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace TestCore
{
	class Program
	{
		class SciterMessages : SciterDebugOutputHandler
		{
			protected override void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text)
			{
				Debug.WriteLine(text);
				//Debug.Write(text);// so I can see Debug output even if 'native debugging' is off
			}
		}

		public static WindowApp AppWnd;
		public static Host AppHost;
		private static SciterMessages sm = new();

		[STAThread]
		static void Main(string[] args)
		{
			// Sciter needs this for drag'n'drop support; STAThread is required for OleInitialize succeess
			int oleres = PInvokeWindows.OleInitialize(IntPtr.Zero);
			Debug.Assert(oleres == 0);

			var list = new List<int> { 123 };
			var ss = SciterValue.FromObject(new { aa = list });

			Debug.WriteLine("Sciter: " + SciterX.Version);
			Debug.WriteLine("Bitness: " + IntPtr.Size);
			
			// Create the window
			AppWnd = new();

			var wnd = AppWnd;
			wnd.CreateTopLevelWindow(new PInvokeUtils.RECT(1500, 800));
			wnd.CenterTopLevelWindow();
			
			// Prepares SciterHost and then load the page
			AppHost = new Host();
			var host = AppHost;
			host.Setup(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");
			host.CallFunction("Wow");
			//host.DebugInspect();

			Debug.Assert(!host.EvalScript("Utils").IsUndefined);

			// Show window and Run message loop
			wnd.Show();
			var r = wnd.Size;
			wnd.Title = "TestCore";// only works after I show the window

			PInvokeUtils.RunMsgLoop();
		}
	}
}