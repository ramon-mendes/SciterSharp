using SciterSharp;
using SciterSharp.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace SciterSharp
{
	public class SciterMixinWindow : SciterWindow
	{
        public void CreateToplevelWindow(PInvokeUtils.RECT frame, IntPtr owner)
        {
			if (owner == IntPtr.Zero)
                throw new ArgumentException("Invalid owner window handle");

			SciterXDef.SCITER_CREATE_WINDOW_FLAGS creationFlags = DefaultCreateFlags;
            CreateWindow(frame, creationFlags, owner);
        }

#if WINDOWS
        public void CreateChildWindow(PInvokeUtils.RECT frame, IntPtr parent)
        {
            if (parent == IntPtr.Zero)
                throw new ArgumentException("Invalid parent window handle");

            if (PInvoke.IsWindow((HWND)parent) == false)
                throw new ArgumentException("Invalid parent window");

#if false
			string wndclass = Marshal.PtrToStringUni(_api.SciterClassName());
			_hwnd = PInvokeWindows.CreateWindowEx(0, wndclass, null, PInvokeWindows.WS_CHILD, 0, 0, frame.right, frame.bottom, parent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			//SetSciterOption(SciterXDef.SCITER_RT_OPTIONS.SCITER_SET_DEBUG_MODE, new IntPtr(1));// NO, user should opt for it
#else
            SciterXDef.SCITER_CREATE_WINDOW_FLAGS flags = SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_CHILD;
            CreateWindow(frame, flags, parent);
#endif
        }
#endif

        /*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="owner_hwnd"></param>
		public void CreatePopupAlphaWindow(int width, int height, IntPtr owner_hwnd)
		{
			PInvokeUtils.RECT frame = new PInvokeUtils.RECT();
			frame.right = width;
			frame.bottom = height;
			CreateWindow(frame, SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_ALPHA | SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_TOOL, owner_hwnd);
			// Sciter BUG: window comes with WM_EX_APPWINDOW style
		}*/

        private void RegisterClass()
		{
			string className = "scitersharp-frame";
			PInvoke.RegisterClassEx(new Windows.Win32.UI.WindowsAndMessaging.WNDCLASSEXW()
			{
				
			});
		}
	}
}
