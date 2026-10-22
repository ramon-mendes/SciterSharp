// Copyright 2025 Ramon F. Mendes
// 
// This file is part of SciterSharp.
// 
// SciterSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SciterSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SciterSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SciterSharp.Interop;

#if OSX
using AppKit;
using Foundation;
#else
using System.Drawing;
#endif

namespace SciterSharp
{
#if OSX
	public class OSXView : NSView
	{
		public OSXView(IntPtr handle)
			: base(handle)
		{
		}
	}
#endif

	public class SciterWindow
#if WINDOWS // net8.0-windows: WinForms interop
		: System.Windows.Forms.IWin32Window
#endif
	{
		protected static SciterX.ISciterAPI _api = SciterX.API;
		public IntPtr _hwnd { get; protected set; }
		private SciterXDef.FPTR_SciterWindowDelegate _proc;
		/// <summary>Linux only: the GtkWindow that contains the Sciter widget (_hwnd)</summary>
		public IntPtr _gtkwindow { get; private set; }
#if OSX
		public NSView _nsview { get; private set; }
#endif

		public static implicit operator IntPtr(SciterWindow wnd)
		{
			return wnd._hwnd;
		}

		public bool SetSciterOption(SciterXDef.SCITER_RT_OPTIONS option, IntPtr value)
		{
			Debug.Assert(_hwnd != IntPtr.Zero);
			return _api.SciterSetOption(_hwnd, option, value);
		}

		public SciterWindow()
		{
			var allow = SciterXDef.SCRIPT_RUNTIME_FEATURES.ALLOW_EVAL |
								SciterXDef.SCRIPT_RUNTIME_FEATURES.ALLOW_FILE_IO |
								SciterXDef.SCRIPT_RUNTIME_FEATURES.ALLOW_SOCKET_IO |
								SciterXDef.SCRIPT_RUNTIME_FEATURES.ALLOW_SYSINFO;
			_api.SciterSetOption(IntPtr.Zero, SciterXDef.SCITER_RT_OPTIONS.SCITER_SET_SCRIPT_RUNTIME_FEATURES, new IntPtr((int)allow));

			_proc = OperatingSystem.IsWindows() ? InternalProcessSciterWindowMessage : null;
		}

		public SciterWindow(IntPtr hwnd)
		{
			_proc = OperatingSystem.IsWindows() ? InternalProcessSciterWindowMessage : null;
			_hwnd = hwnd;
			AfterHandleSet();
		}

		private void AfterHandleSet()
		{
			if(OperatingSystem.IsLinux())
			{
				_gtkwindow = PInvokeGTK.gtk_widget_get_toplevel(_hwnd);
				Debug.Assert(_gtkwindow != IntPtr.Zero);
			}
#if OSX
			_nsview = new OSXView(_hwnd);
#endif
		}

		public const SciterXDef.SCITER_CREATE_WINDOW_FLAGS DefaultCreateFlags =
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_MAIN |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_TITLEBAR |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_RESIZEABLE |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_CONTROLS |
			SciterXDef.SCITER_CREATE_WINDOW_FLAGS.SW_ENABLE_DEBUG;

		/// <summary>
		/// Creates the Sciter window and returns the native handle
		/// </summary>
		/// <param name="frame">Rectangle of the window</param>
		/// <param name="creationFlags">Flags for the window creation, defaults to SW_MAIN | SW_TITLEBAR | SW_RESIZEABLE | SW_CONTROLS | SW_ENABLE_DEBUG</param>
		public void CreateWindow(PInvokeUtils.RECT frame = default, SciterXDef.SCITER_CREATE_WINDOW_FLAGS creationFlags = DefaultCreateFlags, IntPtr parent = default)
		{
			Debug.Assert(_hwnd == IntPtr.Zero);

			_hwnd = _api.SciterCreateWindow(
				creationFlags,
				ref frame,
				_proc,
				IntPtr.Zero,
				parent
			);

            if (_hwnd == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                string message = new Win32Exception(error).Message;
                Debug.Assert(false);

                throw new Exception("CreateWindow() failed: " + message);
            }

			AfterHandleSet();
		}


		public void Destroy()
		{
			if(OperatingSystem.IsWindows())
				PInvokeWindows.DestroyWindow(_hwnd);
			else if(OperatingSystem.IsLinux())
				PInvokeGTK.gtk_widget_destroy(_gtkwindow);
		}

		[SupportedOSPlatform("windows")]
		public bool ModifyStyle(PInvokeWindows.SetWindowLongFlags dwRemove, PInvokeWindows.SetWindowLongFlags dwAdd)
		{
			int GWL_EXSTYLE = -20;

			PInvokeWindows.SetWindowLongFlags dwStyle = (PInvokeWindows.SetWindowLongFlags)PInvokeWindows.GetWindowLongPtr(_hwnd, GWL_EXSTYLE);
			PInvokeWindows.SetWindowLongFlags dwNewStyle = (dwStyle & ~dwRemove) | dwAdd;
			if(dwStyle == dwNewStyle)
				return false;

			PInvokeWindows.SetWindowLongPtr(_hwnd, GWL_EXSTYLE, (IntPtr)dwNewStyle);
			return true;
		}

		[SupportedOSPlatform("windows")]
		public bool ModifyStyleEx(PInvokeWindows.SetWindowLongFlags dwRemove, PInvokeWindows.SetWindowLongFlags dwAdd)
		{
			int GWL_STYLE = -16;

			PInvokeWindows.SetWindowLongFlags dwStyle = (PInvokeWindows.SetWindowLongFlags)PInvokeWindows.GetWindowLongPtr(_hwnd, GWL_STYLE);
			PInvokeWindows.SetWindowLongFlags dwNewStyle = (dwStyle & ~dwRemove) | dwAdd;
			if(dwStyle == dwNewStyle)
				return false;

			PInvokeWindows.SetWindowLongPtr(_hwnd, GWL_STYLE, (IntPtr)dwNewStyle);
			return true;
		}

        /// <summary>
        /// Centers the window in the screen.
        /// Call it after the window is created, but before it is shown to avoid flickering
        /// </summary>
        public void CenterTopLevelWindow()
		{
#if OSX
			_nsview.Window.Center();
#else
			if(OperatingSystem.IsWindows())
			{
				PInvokeUtils.RECT rectWindow;
				PInvokeWindows.GetWindowRect(_hwnd, out rectWindow);

				PInvokeUtils.RECT rectWorkArea = new PInvokeUtils.RECT();
				PInvokeWindows.SystemParametersInfo(PInvokeWindows.SPI_GETWORKAREA, 0, ref rectWorkArea, 0);

				int nX = (rectWorkArea.Width - rectWindow.Width) / 2 + rectWorkArea.left;
				int nY = (rectWorkArea.Height - rectWindow.Height) / 2 + rectWorkArea.top;

				PInvokeWindows.MoveWindow(_hwnd, nX, nY, rectWindow.Width, rectWindow.Height, false);
			}
			else if(OperatingSystem.IsLinux())
			{
				int screen_width = PInvokeGTK.gdk_screen_width();
				int screen_height = PInvokeGTK.gdk_screen_height();

				int window_width, window_height;
				PInvokeGTK.gtk_window_get_size(_gtkwindow, out window_width, out window_height);

				int nX = (screen_width - window_width) / 2;
				int nY = (screen_height - window_height) / 2;

				PInvokeGTK.gtk_window_move(_gtkwindow, nX, nY);
			}
#endif
		}

		/// <summary>
		/// Cross-platform handy method to get the size of the screen
		/// </summary>
		/// <returns>SIZE measures of the screen of primary monitor</returns>
		public static PInvokeUtils.SIZE GetPrimaryMonitorScreenSize()
		{
#if OSX
			var sz = NSScreen.MainScreen.Frame.Size;
			return new PInvokeUtils.SIZE((int)sz.Width, (int)sz.Height);
#else
			if(OperatingSystem.IsWindows())
			{
				int nScreenWidth = PInvokeWindows.GetSystemMetrics(PInvokeWindows.SystemMetric.SM_CXSCREEN);
				int nScreenHeight = PInvokeWindows.GetSystemMetrics(PInvokeWindows.SystemMetric.SM_CYSCREEN);
				return new PInvokeUtils.SIZE() { cx = nScreenWidth, cy = nScreenHeight };
			}
			if(OperatingSystem.IsLinux())
			{
				int screen_width = PInvokeGTK.gdk_screen_width();
				int screen_height = PInvokeGTK.gdk_screen_height();
				return new PInvokeUtils.SIZE() { cx = screen_width, cy = screen_height };
			}
			throw new PlatformNotSupportedException();
#endif
		}

		public PInvokeUtils.SIZE ScreenSize
		{
			get
			{
#if OSX
				var sz = _nsview.Window.Screen.Frame.Size;
				return new PInvokeUtils.SIZE((int)sz.Width, (int)sz.Height);
#else
				if(OperatingSystem.IsWindows())
				{
					IntPtr hmonitor = PInvokeWindows.MonitorFromWindow(_hwnd, PInvokeWindows.MONITOR_DEFAULTTONEAREST);
					PInvokeWindows.MONITORINFO mi = new PInvokeWindows.MONITORINFO() { cbSize = Marshal.SizeOf(typeof(PInvokeWindows.MONITORINFO)) };
					PInvokeWindows.GetMonitorInfo(hmonitor, ref mi);
					return new PInvokeUtils.SIZE(mi.rcMonitor.Width, mi.rcMonitor.Height);
				}
				return new PInvokeUtils.SIZE();// TODO Linux: not implemented
#endif
			}
		}

		public PInvokeUtils.SIZE Size
		{
			get
			{
#if OSX
				var sz = _nsview.Window.Frame.Size;
				return new PInvokeUtils.SIZE { cx = (int)sz.Width, cy = (int)sz.Height };
#else
				if(OperatingSystem.IsWindows())
				{
					PInvokeUtils.RECT rectWindow;
					PInvokeWindows.GetWindowRect(_hwnd, out rectWindow);
					return new PInvokeUtils.SIZE { cx = rectWindow.Width, cy = rectWindow.Height };
				}
				if(OperatingSystem.IsLinux())
				{
					int window_width, window_height;
					PInvokeGTK.gtk_window_get_size(_gtkwindow, out window_width, out window_height);
					return new PInvokeUtils.SIZE(window_width, window_height);
				}
				throw new PlatformNotSupportedException();
#endif
			}
		}

		public PInvokeUtils.POINT Position
		{
			get
			{
#if OSX
				var pos = _nsview.Window.Frame.Location;
				return new PInvokeUtils.POINT((int)pos.X, (int)pos.Y);
#else
				if(OperatingSystem.IsWindows())
				{
					PInvokeUtils.RECT rectWindow;
					PInvokeWindows.GetWindowRect(_hwnd, out rectWindow);
					return new PInvokeUtils.POINT(rectWindow.left, rectWindow.top);
				}
				return new PInvokeUtils.POINT();// TODO Linux: not implemented
#endif
			}

			set
			{
#if OSX
				var pt = new CoreGraphics.CGPoint(value.X, value.Y);
				_nsview.Window.SetFrameTopLeftPoint(pt);
#else
				if(OperatingSystem.IsWindows())
					PInvokeWindows.MoveWindow(_hwnd, value.X, value.Y, Size.cx, Size.cy, false);
				else if(OperatingSystem.IsLinux())
					PInvokeGTK.gtk_window_move(_gtkwindow, value.X, value.Y);
#endif
			}
		}

		/// <summary>
		/// Loads the page resource from the given URL or file path
		/// </summary>
		/// <param name="url_or_filepath">URL or file path of the page</param>
		public bool LoadPage(string url_or_filepath)
		{
			return _api.SciterLoadFile(_hwnd, url_or_filepath);
		}

		/// <summary>
		/// Loads HTML input from a string
		/// </summary>
		/// <param name="html">HTML of the page to be loaded</param>
		/// <param name="baseUrl">Base Url given to the loaded page</param>
		public bool LoadHtml(string html, string baseUrl = null)
		{
			var bytes = Encoding.UTF8.GetBytes(html);
			return _api.SciterLoadHtml(_hwnd, bytes, (uint)bytes.Length, baseUrl);
		}

		public void Show(bool show = true)
		{
#if !OSX
			if(OperatingSystem.IsWindows())
			{
				PInvokeWindows.ShowWindow(_hwnd, show ? PInvokeWindows.ShowWindowCommands.Show : PInvokeWindows.ShowWindowCommands.Hide);
			}
			else if(OperatingSystem.IsLinux())
			{
				if(show)
					PInvokeGTK.gtk_window_present(_gtkwindow);
				else
					PInvokeGTK.gtk_widget_hide(_hwnd);
			}
#else
			if(show)
			{
				_nsview.Window.MakeMainWindow();
				_nsview.Window.MakeKeyAndOrderFront(null);
			} else {
				_nsview.Window.OrderOut(_nsview.Window);// PerformMiniaturize?
			}
#endif
		}

		public void ShowModal()
		{
			Show();
			PInvokeUtils.RunMsgLoop();
		}


		/// <summary>
		/// Close the window. Posts WM_CLOSE message on Windows.
		/// </summary>
		public void Close()
		{
#if OSX
			_nsview.Window.Close();
#else
			if(OperatingSystem.IsWindows())
				PInvokeWindows.PostMessage(_hwnd, PInvokeWindows.Win32Msg.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
			else if(OperatingSystem.IsLinux())
				PInvokeGTK.gtk_window_close(_gtkwindow);
#endif
		}

		public bool IsVisible
		{
			get
			{
#if OSX
				return _nsview.Window.IsVisible;
#else
				if(OperatingSystem.IsWindows())
					return PInvokeWindows.IsWindowVisible(_hwnd);
				if(OperatingSystem.IsLinux())
					return PInvokeGTK.gtk_widget_get_visible(_gtkwindow) != 0;
				throw new PlatformNotSupportedException();
#endif
			}
		}

		/*public IntPtr VM
		{
			get { return _api.SciterGetVM(_hwnd); }
		}*/

#if WINDOWS // System.Drawing.Icon: net8.0-windows only
		public Icon Icon
		{
            // instead of using this property, you can use View.windowIcon on all platforms
			set
			{
				// larger icon
				PInvokeWindows.SendMessageW(_hwnd, PInvokeWindows.Win32Msg.WM_SETICON, new IntPtr(1), value.Handle);
				// small icon
				PInvokeWindows.SendMessageW(_hwnd, PInvokeWindows.Win32Msg.WM_SETICON, IntPtr.Zero, new Icon(value, 16, 16).Handle);
			}
		}
#endif

        public string Title
		{
			set
			{
				Debug.Assert(_hwnd != IntPtr.Zero);
#if OSX
				_nsview.Window.Title = value;
#else
				if(OperatingSystem.IsWindows())
				{
					IntPtr strPtr = Marshal.StringToHGlobalUni(value);
					PInvokeWindows.SendMessageW(_hwnd, PInvokeWindows.Win32Msg.WM_SETTEXT, IntPtr.Zero, strPtr);
					Marshal.FreeHGlobal(strPtr);
				}
				else if(OperatingSystem.IsLinux())
				{
					PInvokeGTK.gtk_window_set_title(_gtkwindow, value);
				}
#endif
			}

			get
			{
				Debug.Assert(_hwnd != IntPtr.Zero);
#if OSX
				return _nsview.Window.Title;
#else
				if(OperatingSystem.IsWindows())
				{
					IntPtr unmanagedPointer = Marshal.AllocHGlobal(2048);
					IntPtr chars_copied = PInvokeWindows.SendMessageW(_hwnd, PInvokeWindows.Win32Msg.WM_GETTEXT, new IntPtr(2048), unmanagedPointer);
					string title = Marshal.PtrToStringUni(unmanagedPointer, chars_copied.ToInt32());
					Marshal.FreeHGlobal(unmanagedPointer);
					return title;
				}
				if(OperatingSystem.IsLinux())
				{
					IntPtr str_ptr = PInvokeGTK.gtk_window_get_title(_gtkwindow);
					return Marshal.PtrToStringUTF8(str_ptr);// GTK strings are UTF-8
				}
				throw new PlatformNotSupportedException();
#endif
			}
		}

		public SciterElement RootElement
		{
			get
			{
				Debug.Assert(_hwnd != IntPtr.Zero);
				IntPtr he;
				var r = _api.SciterGetRootElement(_hwnd, out he);
				Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);

				if(he == IntPtr.Zero)
					return null;// no page loaded yet?
				return new SciterElement(he);
			}
		}

		/// <summary>
		/// Find element at point x/y of the window, client area relative
		/// </summary>
		public SciterElement ElementAtPoint(int x, int y)
		{
			PInvokeUtils.POINT pt = new PInvokeUtils.POINT() { X = x, Y = y };
			IntPtr outhe;
			var r = _api.SciterFindElement(_hwnd, pt, out outhe);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);

			if(outhe == IntPtr.Zero)
				return null;
			return new SciterElement(outhe);
		}

		/// <summary>
		/// Searches this windows DOM tree for element with the given UID
		/// </summary>
		/// <returns>The element, or null if it doesn't exists</returns>
		public SciterElement ElementByUID(uint uid)
		{
			IntPtr he;
			var r = _api.SciterGetElementByUID(_hwnd, uid, out he);
			Debug.Assert(r == SciterXDom.SCDOM_RESULT.SCDOM_OK);

			if(he != IntPtr.Zero)
				return new SciterElement(he);
			return null;
		}

		public uint GetMinWidth()
		{
			Debug.Assert(_hwnd != IntPtr.Zero);
			return _api.SciterGetMinWidth(_hwnd);
		}

		public uint GetMinHeight(uint for_width)
		{
			Debug.Assert(_hwnd != IntPtr.Zero);
			return _api.SciterGetMinHeight(_hwnd, for_width);
		}

		/// <summary>
		/// Update pending changes in Sciter window and forces painting if necessary
		/// </summary>
		public bool UpdateWindow()
		{
			return _api.SciterUpdateWindow(_hwnd);
		}

		public SciterValue CallFunction(string name, params SciterValue[] args)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Create the window first");
			Debug.Assert(name != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterCall(_hwnd, name, (uint)args.Length, SciterValue.ToVALUEArray(args), out vret);
			return new SciterValue(vret);
		}

		public SciterValue EvalScript(string script)
		{
			Debug.Assert(_hwnd != IntPtr.Zero, "Create the window first");
			Debug.Assert(script != null);

			SciterXValue.VALUE vret = new SciterXValue.VALUE();
			_api.SciterEval(_hwnd, script, (uint)script.Length, out vret);
			return new SciterValue(vret);
		}

		/// <summary>
		/// For example media type can be "handheld", "projection", "screen", "screen-hires", etc.
		/// By default sciter window has "screen" media type.
		/// Media type name is used while loading and parsing style sheets in the engine so
		/// you should call this function* before* loading document in it.
		/// </summary>
		public bool SetMediaType(string mediaType)
		{
			return _api.SciterSetMediaType(_hwnd, mediaType);
		}

		/// <summary>
		/// For example media type can be "handheld:true", "projection:true", "screen:true", etc.
		/// By default sciter window has "screen:true" and "desktop:true"/"handheld:true" media variables.
		/// Media variables can be changed in runtime. This will cause styles of the document to be reset.
		/// </summary>
		/// <param name="mediaVars">Map that contains name/value pairs - media variables to be set</param>
		public bool SetMediaVars(SciterValue mediaVars)
		{
			SciterXValue.VALUE v = mediaVars.ToVALUE();
			return _api.SciterSetMediaVars(_hwnd, ref v);
		}

		// Windows only: window procedure given to SciterCreateWindow()
		private IntPtr InternalProcessSciterWindowMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr pParam, ref bool handled)
		{
			Debug.Assert(pParam.ToInt32() == 0);
			Debug.Assert(_hwnd.ToInt32() == 0 || hwnd == _hwnd);

			IntPtr lResult = IntPtr.Zero;
			handled = ProcessWindowMessage(hwnd, msg, wParam, lParam, ref lResult);
			return lResult;
		}

		protected virtual bool ProcessWindowMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, ref IntPtr lResult)// overrisable
		{
			return false;
		}


		public IntPtr Handle
		{
			get
			{
				return _hwnd;
			}
		}
	}
}