#if OSX
using System;
using System.Diagnostics;
using AppKit;
using Foundation;
using SciterSharp;
using SciterSharp.Interop;

namespace TestGTK
{
	class Program
	{
		static void Main(string[] args)
		{
			// Default GFX in Sciter v4 is Skia, switch to CoreGraphics (seems more stable)
			SciterX.API.SciterSetOption(IntPtr.Zero, SciterXDef.SCITER_RT_OPTIONS.SCITER_SET_GFX_LAYER, new IntPtr((int) SciterXDef.GFX_LAYER.GFX_LAYER_CG));

			NSApplication.Init();

			using(var p = new NSAutoreleasePool())
			{
				var application = NSApplication.SharedApplication;
				application.Delegate = new AppDelegate();
				application.Run();
			}
		}
	}

	[Register("AppDelegate")]// needed?
	class AppDelegate : NSApplicationDelegate
	{
		static readonly SciterMessages sm = new SciterMessages();
		public static Window AppWindow { get; private set; }
		public static Host AppHost { get; private set; }

		public override void DidFinishLaunching(NSNotification notification)
		{
			Mono.Setup();

			// Create the window
			AppWindow = new Window();

			// Prepares SciterHost and then load the page
			AppHost = new Host(AppWindow);

			// Set our custom menu with Cocoa
			if(true)
			{
				// From XIB/NIB file (editable in Xcode Interface Builder)
				NSArray arr;
				bool res = NSBundle.MainBundle.LoadNibNamed("MainMenu", NSApplication.SharedApplication, out arr);
				Debug.Assert(res);
			}
			else
			{
				// Or we can create it programatically
				var menu1 = new NSMenu();
				menu1.AddItem(new NSMenuItem("Hi there"));
				menu1.AddItem(NSMenuItem.SeparatorItem);
				menu1.AddItem(new NSMenuItem("Exit", (sender, e) => NSApplication.SharedApplication.Terminate(menu1)));

				var menu2 = new NSMenu("Second menu");
				menu2.AddItem(new NSMenuItem("What"));
				menu2.AddItem(new NSMenuItem("Hey"));

				var menubar = new NSMenu();
				menubar.AddItem(new NSMenuItem { Submenu = menu1 });
				menubar.AddItem(new NSMenuItem { Submenu = menu2 });

				NSApplication.SharedApplication.MainMenu = menubar;
			}
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
		{
			return false;
		}

		public override void WillTerminate(NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}

	// In OSX/Xamarin Studio, make Sciter messages be shown at 'Application Output' panel
    class SciterMessages : SciterDebugOutputHandler
    {
        protected override void OnOutput(SciterSharp.Interop.SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterSharp.Interop.SciterXDef.OUTPUT_SEVERITY severity, string text)
        {
            Console.WriteLine(text);
        }
    }
}
#endif