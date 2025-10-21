using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;
using Windows.Win32;

namespace SciterSharp
{
	public class SciterMixinWindow : SciterWindow
	{
		public void CreateToplevelWindow(int width, int height, IntPtr owner = default)
		{

		}

		public void CreateChildWindow(int width, int height, IntPtr parent)
		{

		}

		private void RegisterClass()
		{
			string className = "scitersharp-frame";
			PInvoke.RegisterClassEx(new Windows.Win32.UI.WindowsAndMessaging.WNDCLASSEXW()
			{
				
			});
		}
	}
}
