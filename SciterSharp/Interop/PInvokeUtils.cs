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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SciterSharp.Interop
{
	public static class PInvokeUtils
	{
		public static void RunMsgLoop()
		{
#if WINDOWS
			PInvokeWindows.MSG msg;
			while(PInvokeWindows.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
			{
				PInvokeWindows.TranslateMessage(ref msg);
				PInvokeWindows.DispatchMessage(ref msg);
			}
#elif GTKMONO
			PInvokeGTK.gtk_main();
#elif OSX
            throw new Exception("Do not call PInvokeUtils.RunMsgLoop() on OSX.");
#endif
		}

		// PInvoke marshaling utils ===============================================================
		public static IntPtr NativeUtf16FromString(string managedString, int minlen)
		{
			// Marshal.StringToHGlobalUni() -- does not gives the buffer size
			byte[] strbuffer = Encoding.Unicode.GetBytes(managedString);

			minlen = Math.Max(strbuffer.Length, minlen);
			byte[] zerobuffer = new byte[minlen];
			Buffer.BlockCopy(strbuffer, 0, zerobuffer, 0, strbuffer.Length);

			IntPtr nativeUtf16 = Marshal.AllocHGlobal(minlen);
			Marshal.Copy(zerobuffer, 0, nativeUtf16, minlen);
			return nativeUtf16;
		}
		public static void NativeUtf16FromString_FreeBuffer(IntPtr buffer)
		{
			Marshal.FreeHGlobal(buffer);
		}

		public static string StringFromNativeUtf16(IntPtr nativeUtf16)
		{
			return Marshal.PtrToStringUni(nativeUtf16);
		}

		// PInvoke structs ===============================================================
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left, top, right, bottom;

			public int Width { get { return right - left; } }
			public int Height { get { return bottom - top; } }
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE
		{
			public int cx;
			public int cy;

			public SIZE(int x, int y)
			{
				cx = x;
				cy = y;
			}
		}
	}
}