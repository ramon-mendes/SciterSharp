// Copyright 2016 Ramon F. Mendes
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

#pragma warning disable 0169

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
	public static class SciterXDef
	{
		public enum LoadResult : uint
		{
			/// <summary>
			/// do default loading if data not set
			/// </summary>
			LOAD_OK = 0,

			/// <summary>
			/// discard request completely
			/// </summary>
			LOAD_DISCARD = 1,

			/// <summary>
			/// data will be delivered later by the host
			/// Host application must call SciterDataReadyAsync(,,, requestId) on each LOAD_DELAYED request to avoid memory leaks.
			/// </summary>
			LOAD_DELAYED = 2,

			/// <summary>
			/// you return LOAD_MYSELF result to indicate that your (the host) application took or will take care about HREQUEST in your code completely.
			/// Use sciter-x-request.h[pp] API functions with SCN_LOAD_DATA::requestId handle.
			/// </summary>
			LOAD_MYSELF = 3,
		}

		public const uint SC_LOAD_DATA = 0x01;
		public const uint SC_DATA_LOADED = 0x02;
		public const uint SC_ATTACH_BEHAVIOR = 0x04;
		public const uint SC_ENGINE_DESTROYED = 0x05;
		public const uint SC_POSTED_NOTIFICATION = 0x06;
		public const uint SC_GRAPHICS_CRITICAL_FAILURE = 0x07;

		[StructLayout(LayoutKind.Sequential)]
		public struct SCITER_CALLBACK_NOTIFICATION
		{
			public uint code;// SC_LOAD_DATA or SC_DATA_LOADED or ..
			public IntPtr hwnd;
		}

		public delegate uint FPTR_SciterHostCallback(IntPtr ns /*SCITER_CALLBACK_NOTIFICATION*/, IntPtr callbackParam);

		[StructLayout(LayoutKind.Sequential)]
		public struct SCN_LOAD_DATA
		{
			public uint code;				// UINT - [in] one of the codes above.
			public IntPtr hwnd;				// HWINDOW - [in] HWINDOW of the window this callback was attached to.

			[MarshalAs(UnmanagedType.LPWStr)]
			public string uri;				// LPCWSTR - [in] Zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".

			public IntPtr outData;								// LPCBYTE - [in,out] pointer to loaded data to return. if data exists in the cache then this field contain pointer to it
			public uint outDataSize;							// UINT - [in,out] loaded data size to return.
			public SciterXRequest.SciterResourceType dataType;	// UINT - [in] SciterResourceType

			public IntPtr requestId;        // HREQUEST - [in] request handle that can be used with sciter-x-request API

			public IntPtr principal;		// HELEMENT
			public IntPtr initiator;		// HELEMENT
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCN_DATA_LOADED
		{
			public uint code;	// UINT - [in] one of the codes above.
			public IntPtr hwnd;	// HWINDOW - [in] HWINDOW of the window this callback was attached to.

			[MarshalAs(UnmanagedType.LPWStr)]
			public string uri;  // LPCWSTR - [in] Zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".

			public IntPtr data;									// LPCBYTE - [in] pointer to loaded data.
			public uint dataSize;								// UINT - [in] loaded data size (in bytes).
			public SciterXRequest.SciterResourceType dataType;	// UINT - [in] SciterResourceType
			public uint status;									// UINT - [in] 
																// status = 0 (dataSize == 0) - unknown error. 
																// status = 100..505 - http response status, Note: 200 - OK! 
																// status > 12000 - wininet error code, see ERROR_INTERNET_*** in wininet.h
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCN_ATTACH_BEHAVIOR
		{
			public uint code;			// UINT - [in] one of the codes above.
			public IntPtr hwnd;			// HWINDOW - [in] HWINDOW of the window this callback was attached to.

			public IntPtr elem;			// HELEMENT - [in] target DOM element handle
			public IntPtr behaviorName;	// LPCSTR - [in] zero terminated string, string appears as value of CSS behavior:"???" attribute.

			public SciterXBehaviors.FPTR_ElementEventProc elementProc;	// ElementEventProc - [out] pointer to ElementEventProc function.
			public IntPtr elementTag;	// LPVOID - [out] tag value, passed as is into pointer ElementEventProc function.
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct SCN_ENGINE_DESTROYED
		{
			public uint code;	// UINT [in] one of the codes above.
			public IntPtr hwnd;	// HWINDOW - [in] HWINDOW of the window this callback was attached to.
		}

		public struct SCN_POSTED_NOTIFICATION
		{
			public uint	code;		// UINT - [in] one of the codes above.
			public IntPtr	hwnd;	// HWINDOW - [in] HWINDOW of the window this callback was attached to.
			public IntPtr	wparam;	// UINT_PTR
			public IntPtr	lparam;	// UINT_PTR
			public IntPtr	lreturn;// UINT_PTR
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SCN_GRAPHICS_CRITICAL_FAILURE
		{
			public uint code;   // UINT [in] SC_GRAPHICS_CRITICAL_FAILURE.
			public IntPtr hwnd; // HWINDOW - [in] HWINDOW of the window this callback was attached to.
		}

		public enum SCRIPT_RUNTIME_FEATURES : uint
		{
			ALLOW_FILE_IO = 0x00000001,
			ALLOW_SOCKET_IO = 0x00000002,
			ALLOW_EVAL = 0x00000004,
			ALLOW_SYSINFO = 0x00000008
		}

		public enum GFX_LAYER : uint
		{
			GFX_LAYER_GDI = 1, GFX_LAYER_CG = 1, GFX_LAYER_CAIRO = 1,
			GFX_LAYER_WARP = 2,
			GFX_LAYER_D2D = 3,
			GFX_LAYER_SKIA = 4,
			GFX_LAYER_SKIA_OPENGL = 5,
			GFX_LAYER_AUTO = 0xFFFF,
		}

		public enum SCITER_RT_OPTIONS : uint
		{
			/// <summary>value:TRUE - enable, value:FALSE - disable, enabled by default</summary>
			SCITER_SMOOTH_SCROLL = 1,

			/// <summary>value: milliseconds, connection timeout of http client</summary>
			SCITER_CONNECTION_TIMEOUT = 2,

			/// <summary>value: 0 - drop connection, 1 - use builtin dialog, 2 - accept connection silently</summary>
			SCITER_HTTPS_ERROR = 3,

			/// <summary>value: 0 - system default, 1 - no smoothing, 2 - std smoothing, 3 - clear type</summary>
			SCITER_FONT_SMOOTHING = 4,

			/// <summary>
			/// Windows Aero support, value: 
			/// 0 - normal drawing, 
			/// 1 - window has transparent background after calls DwmExtendFrameIntoClientArea() or DwmEnableBlurBehindWindow()
			/// </summary>
			SCITER_TRANSPARENT_WINDOW = 6,  // Windows Aero support, value: 
											// 0 - normal drawing, 
											// 1 - window has transparent background after calls DwmExtendFrameIntoClientArea() or DwmEnableBlurBehindWindow().

			/// <summary>
			/// hWnd = NULL,
			/// value = LPCBYTE, json - GPU black list, see: gpu-blacklist.json resource.
			/// </summary>
			SCITER_SET_GPU_BLACKLIST = 7,

			/// <summary>value - combination of SCRIPT_RUNTIME_FEATURES flags.</summary>
			SCITER_SET_SCRIPT_RUNTIME_FEATURES = 8,

			/// <summary>hWnd = NULL, value - GFX_LAYER</summary>
			SCITER_SET_GFX_LAYER = 9,

			/// <summary>hWnd, value - TRUE/FALSE</summary>
			SCITER_SET_DEBUG_MODE = 10,

			/// <summary>
			/// hWnd = NULL, value - BOOL, TRUE - the engine will use "unisex" theme that is common for all platforms.
			/// That UX theme is not using OS primitives for rendering input elements. Use it if you want exactly
			/// the same (modulo fonts) look-n-feel on all platforms.
			/// </summary>
			SCITER_SET_UX_THEMING = 11,

			/// <summary>hWnd, value - TRUE/FALSE - window uses per pixel alpha (e.g. WS_EX_LAYERED/UpdateLayeredWindow() window)</summary>
			SCITER_ALPHA_WINDOW = 12,

			/// <summary>
			///	hWnd - N/A , value LPCSTR - UTF-8 encoded script source to be loaded into each view before any other script execution.
			/// The engine copies this string inside the call.
			/// </summary>
			SCITER_SET_INIT_SCRIPT = 13,
		}

#if WINDOWS
		public delegate IntPtr FPTR_SciterWindowDelegate(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, IntPtr pParam, ref bool handled);
#elif OSX
		public delegate void FPTR_SciterWindowDelegate();// void*		Obj-C id, NSWindowDelegate and NSResponder
#elif GTKMONO
		public delegate void FPTR_SciterWindowDelegate();// void*
#endif


		public enum SCITER_CREATE_WINDOW_FLAGS : uint
		{
			/// <summary>Child window only, if this flag is set all other flags ignored</summary>
			SW_CHILD = (1 << 0),
			/// <summary>Toplevel window, has titlebar</summary>
			SW_TITLEBAR = (1 << 1),
			/// <summary>Has resizeable frame</summary>
			SW_RESIZEABLE = (1 << 2),
			/// <summary>Is tool window</summary>
			SW_TOOL = (1 << 3),
			/// <summary>Has minimize / maximize buttons</summary>
			SW_CONTROLS = (1 << 4),
			/// <summary>Glassy window ( DwmExtendFrameIntoClientArea on windows )</summary>
			SW_GLASSY = (1 << 5),
			/// <summary>Transparent window ( e.g. WS_EX_LAYERED on Windows )</summary>
			SW_ALPHA = (1 << 6),
			/// <summary>Main window of the app, will terminate app on close</summary>
			SW_MAIN = (1 << 7),
			///<summary> The window is created as topmost</summary>
			SW_POPUP = (1 << 8),
			/// <summary>Make this window inspector ready</summary>
			SW_ENABLE_DEBUG = (1 << 9),
			/// <summary>It has its own script VM</summary>
			SW_OWNS_VM = (1 << 10)
		}

		public enum OUTPUT_SUBSYTEM : uint
		{
			OT_DOM = 0,       // html parser & runtime
			OT_CSSS,          // csss! parser & runtime
			OT_CSS,           // css parser
			OT_TIS,           // TIS parser & runtime
		}
		public enum OUTPUT_SEVERITY : uint
		{
			OS_INFO,
			OS_WARNING,
			OS_ERROR,
		}


		// alias DEBUG_OUTPUT_PROC = VOID function(LPVOID param, UINT subsystem /*OUTPUT_SUBSYTEMS*/, UINT severity, LPCWSTR text, UINT text_length);
		public delegate IntPtr FPTR_DEBUG_OUTPUT_PROC(IntPtr param, uint subsystem /*OUTPUT_SUBSYTEMS*/, uint severity /*OUTPUT_SEVERITY*/, IntPtr text_ptr, uint text_length);

	}
}

#pragma warning restore 0169