using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SciterSharp.Interop;

namespace SciterSharp
{
	public abstract class SciterDebugOutputHandler
	{
		private static SciterX.ISciterAPI _api = SciterX.API;
		private readonly SciterXDef.FPTR_DEBUG_OUTPUT_PROC _proc;// keep a copy of the delegate so it survives GC

		/// <summary>
		/// Setup a global debug output handler
		/// </summary>
		public SciterDebugOutputHandler()
		{
			_proc = this.DebugOutputProc;
			_api.SciterSetupDebugOutput(IntPtr.Zero, IntPtr.Zero, _proc);
		}

		/// <summary>
		/// Setup a Sciter window specific debug output handler
		/// </summary>
		/// <param name="hwnd"></param>
		public SciterDebugOutputHandler(IntPtr hwnd)
		{
			Debug.Assert(hwnd != IntPtr.Zero);

			_proc = this.DebugOutputProc;
			_api.SciterSetupDebugOutput(hwnd, IntPtr.Zero, _proc);
		}

		private IntPtr DebugOutputProc(IntPtr param, uint subsystem /*OUTPUT_SUBSYTEMS*/, uint severity /*OUTPUT_SEVERITY*/, IntPtr text_ptr, uint text_length)
		{
			string text = Marshal.PtrToStringUni(text_ptr, (int) text_length);
			OnOutput((SciterXDef.OUTPUT_SUBSYTEM) subsystem, (SciterXDef.OUTPUT_SEVERITY) severity, text);
			return IntPtr.Zero;
		}

		protected abstract void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text);
	}
}