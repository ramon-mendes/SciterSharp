using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;

namespace TestCore
{
	class WindowApp : SciterMixinWindow
	{
		public static void Create()
		{
			var wnd = new WindowApp();
			wnd.CreateToplevelWindow(800, 600);

			var host = new Host();
			host.Setup(wnd);
			host.AttachEvh(new HostEvh());
			host.SetupPage("index.html");
			host.CallFunction("Wow");
			//host.DebugInspect();

			wnd.Show();
		}

		protected override bool ProcessWindowMessage(nint hwnd, uint msg, nint wParam, nint lParam, ref nint lResult)
		{
			return base.ProcessWindowMessage(hwnd, msg, wParam, lParam, ref lResult);
		}
	}
}
