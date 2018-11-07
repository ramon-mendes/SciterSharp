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

#if true
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SciterSharp.Interop
{
	public static class PInvokeGTK
	{
		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_init(IntPtr argc, IntPtr argv);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_main();

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr gtk_widget_get_toplevel(IntPtr widget);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_set_title(IntPtr window, [MarshalAs(UnmanagedType.LPStr)]string title);

        [DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr gtk_window_get_title(IntPtr window);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_present(IntPtr window);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_widget_hide(IntPtr window);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_close(IntPtr window);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_window_get_size(IntPtr window, out int width, out int height);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gdk_screen_width();

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gdk_screen_height();

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gtk_window_move(IntPtr window, int x, int y);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern void gtk_widget_destroy(IntPtr widget);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gtk_widget_get_visible(IntPtr widget);

		[DllImport("libgtk-3.so.0", CallingConvention = CallingConvention.Cdecl)]
		public static extern int gtk_window_set_icon_from_file(IntPtr window, [MarshalAs(UnmanagedType.LPStr)]string title, IntPtr err);
	}
}
#endif