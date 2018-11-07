using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestGraphics
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if WINDOWS
            // Sciter needs this for drag'n'drop support; STAThread is required for OleInitialize succeess
            int oleres = PInvokeWindows.OleInitialize(IntPtr.Zero);
			Debug.Assert(oleres == 0);
#endif

			Console.WriteLine("Sciter: " + SciterX.Version);

			// Create the window
			var wnd = new SciterWindow();
			wnd.CreateMainWindow(1500, 800);
			wnd.CenterTopLevelWindow();
			wnd.Title = "Sciter Bootstrap";
#if WINDOWS
			wnd.Icon = Properties.Resources.IconMain;
#endif

			// Prepares SciterHost and then load the page
			var host = new Host();
			host.SetupWindow(wnd);
			host.RegisterBehaviorHandler(typeof(DrawBitmapBehavior), "DrawBitmap");
			host.RegisterBehaviorHandler(typeof(DrawTextBehavior), "DrawText");
			host.RegisterBehaviorHandler(typeof(DrawGeometryBehavior), "DrawGeometry");
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");

			// Show window and Run message loop
			wnd.Show();
			PInvokeUtils.RunMsgLoop();
		}
	}
}