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

		public static SciterWindow AppWnd;
		public static Host AppHost;
		private static SciterMessages sm = new SciterMessages();

		[STAThread]
		static void Main(string[] args)
		{
			var list = new List<int> { 123 };
			var ss = SciterValue.FromObject(new { aa = list });

			Debug.WriteLine("Sciter: " + SciterX.Version);
			Debug.WriteLine("Bitness: " + IntPtr.Size);

			// Sciter needs this for drag'n'drop support; STAThread is required for OleInitialize succeess
			int oleres = PInvokeWindows.OleInitialize(IntPtr.Zero);
			Debug.Assert(oleres == 0);
			
			// Create the window
			AppWnd = new SciterWindow();

			var rc = new PInvokeUtils.RECT();
			rc.right = 800;
			rc.bottom = 600;

			var wnd = AppWnd;
			wnd.CreateMainWindow(1500, 800);
			wnd.CenterTopLevelWindow();
			wnd.Title = "TestCore";
			
			// Prepares SciterHost and then load the page
			AppHost = new Host();
			var host = AppHost;
			host.Setup(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");
			//host.DebugInspect();

			//byte[] css_bytes = File.ReadAllBytes(@"D:\ProjetosSciter\AssetsDrop\AssetsDrop\res\css\global.css");
			//SciterX.API.SciterAppendMasterCSS(css_bytes, (uint) css_bytes.Length);
			Debug.Assert(!host.EvalScript("Utils").IsUndefined);

			// Show window and Run message loop
			wnd.Show();
			PInvokeUtils.RunMsgLoop();
		}
	}
}