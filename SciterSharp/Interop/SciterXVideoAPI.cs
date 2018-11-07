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

// Will be very troublesome to implement it
// http://www.cnetion.com/call-unmanaged-function-in-struct-from-vtable-qq-AUvBjo22ivICeoL1jypO.php
namespace SciterSharp.Interop
{
    public static class SciterXVideoAPI
    {
		public enum COLOR_SPACE : int
		{
			COLOR_SPACE_UNKNOWN,
			COLOR_SPACE_YV12,
			COLOR_SPACE_IYUV, // a.k.a. I420  
			COLOR_SPACE_NV12,
			COLOR_SPACE_YUY2,
			COLOR_SPACE_RGB24,
			COLOR_SPACE_RGB555,
			COLOR_SPACE_RGB565,
			COLOR_SPACE_RGB32 // with alpha, sic!
		}

		const string VIDEO_SOURCE_INAME = "source.video.sciter.com";
		const string VIDEO_DESTINATION_INAME = "destination.video.sciter.com";
		const string FRAGMENTED_VIDEO_DESTINATION_INAME = "fragmented.destination.video.sciter.com";


		// FPTRs delegates - aux::iasset
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate long FPTR_add_ref(IntPtr ptrThis);// virtual long  add_ref() = 0;
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate long FPTR_release(IntPtr ptrThis);// virtual long  release() = 0;
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_get_interface(IntPtr ptrThis, [MarshalAs(UnmanagedType.LPStr)]string name, out IntPtr ptrout);// virtual bool  get_interface(const char* name, iasset** out) = 0;

		// FPTRs delegates - sciter::video_destination
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_is_alive(IntPtr ptrThis);// virtual bool is_alive() = 0;
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_start_streaming(IntPtr ptrThis, int frame_width, int frame_height, COLOR_SPACE color_space, IntPtr src/*can be null/zero*/);// virtual bool start_streaming( int frame_width, int frame_height, int color_space, video_source* src = 0 ) = 0;
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_stop_streaming(IntPtr ptrThis);// virtual bool stop_streaming() = 0;
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_render_frame(IntPtr ptrThis, IntPtr frame_data, uint frame_data_size);// virtual bool render_frame(const BYTE* frame_data, UINT frame_data_size) = 0;

		// FPTRs delegates - sciter::fragmented_video_destination
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		public delegate bool FPTR_render_frame_part(IntPtr ptrThis, byte[] frame_data, uint frame_data_size, int x, int y, int width, int height);// virtual bool render_frame_part(const BYTE* frame_data, UINT frame_data_size, int x, int y, int width, int height) = 0;

		
		[StructLayout(LayoutKind.Sequential)]
		public struct video_destination_vtable
		{
			public FPTR_add_ref add_ref;
			public FPTR_release release;
			public FPTR_get_interface get_interface;
			public FPTR_is_alive is_alive;
			public FPTR_start_streaming start_streaming;
			public FPTR_stop_streaming stop_streaming;
			public FPTR_render_frame render_frame;
		}
    }
}