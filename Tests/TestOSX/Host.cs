using System;
using SciterSharp;
using AppKit;
using Foundation;

namespace TestOSX
{
	public class Host : SciterHost
	{
		public Host(SciterWindow wnd)
			: base(wnd)
		{
			RegisterBehaviorHandler(typeof(ImgDrawBehavior));

			AttachEvh(new HostEVH());
			wnd.LoadPage("file:///Users/midiway/Documents/SciterSharp/Tests/TestOSX/res/index.html");
			wnd.CenterTopLevelWindow();
			wnd.Show();
		}

		protected override SciterSharp.Interop.SciterXDef.LoadResult OnLoadData(SciterSharp.Interop.SciterXDef.SCN_LOAD_DATA sld)
		{
			return base.OnLoadData(sld);
		}
	}

	class HostEVH : SciterEventHandler
	{

	}
}