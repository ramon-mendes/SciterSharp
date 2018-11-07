#if WINDOWS || GTKMONO
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;

namespace TestGTK
{
	class Program
	{
		public static Window AppWindow { get; private set; }// must keep a reference to survive GC
		public static Host AppHost { get; private set; }

		[STAThread]
		static void Main(string[] args)
		{
			MessageBox.Show (IntPtr.Zero, "ola", "mnundo");

			if(IntPtr.Size == 4)
			{
				Debug.Assert(false, "sciter.dll that comes bundled in TestGTK is the x64 version, make sure to change it to the x86 version if building for x86 (Windows only)");
			}

#if WINDOWS
			// Sciter needs this for drag'n'drop support; STAThread is required for OleInitialize succeess
			int oleres = PInvokeWindows.OleInitialize(IntPtr.Zero);
			Debug.Assert(oleres == 0);
#endif
#if GTKMONO
			PInvokeGTK.gtk_init(IntPtr.Zero, IntPtr.Zero);
			Mono.Setup();
#endif

			/*
				NOTE:
				In Linux, if you are getting a System.TypeInitializationException below, it is because you don't have 'libsciter-gtk-64.so' in your LD_LIBRARY_PATH.
				Run 'sudo bash install-libsciter.sh' contained in this package to install it in your system.
			*/
			// Create the window
			AppWindow = new Window();

			// Prepares SciterHost and then load the page
			AppHost = new Host(AppWindow);

			// Run message loop
			PInvokeUtils.RunMsgLoop();
		}
	}
}
#endif