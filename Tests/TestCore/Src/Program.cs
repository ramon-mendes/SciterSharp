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

namespace TestCore
{
	class Program
	{
		class SciterMessages : SciterDebugOutputHandler
		{
			protected override void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text)
			{
				Console.WriteLine(text);
				//Debug.Write(text);// so I can see Debug output even if 'native debugging' is off
			}
		}

		public static SciterWindow AppWnd;
		public static Host AppHost;
		private static SciterMessages sm = new SciterMessages();

		[STAThread]
		static void Main(string[] args)
		{
			Console.WriteLine("Sciter: " + SciterX.Version);
			Console.WriteLine("Bitness: " + IntPtr.Size);

			// Sciter needs this for drag'n'drop support; STAThread is required for OleInitialize succeess
			int oleres = PInvokeWindows.OleInitialize(IntPtr.Zero);
			Debug.Assert(oleres == 0);
			
			// Create the window
			AppWnd = new SciterWindow();

			var rc = new PInvokeUtils.RECT();
			rc.right = 800;
			rc.bottom = 600;

			var wnd = AppWnd;
			//wnd.CreateWindow(rc, SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_POPUP | SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_MAIN | SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_RESIZEABLE);
			wnd.CreateMainWindow(1500, 800);
			wnd.CenterTopLevelWindow();
			wnd.Title = "TestCore";
			wnd.Icon = Properties.Resources.IconMain;
			
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