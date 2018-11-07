using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SciterSharp;
using SciterSharp.Interop;

namespace UnitTests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void TestSciterElement()
		{
			SciterElement el = SciterElement.Create("div");
			SciterElement el2 = new SciterElement(el._he);
			Assert.IsTrue(el == el2);
		}

		[TestMethod]
		public void TestSciterValue()
		{
			//string[] arr = new string[] { "A", "B", "C" };
			int[] arr = new int[] { 1, 2, 3 };
			//SciterValue res = SciterValue.FromList(arr);
			SciterValue res = new SciterValue();
			res.Append(new SciterValue(1));
			res.Append(new SciterValue(1));
			res.Append(new SciterValue(1));
			string r = res.ToString();
			string r2 = res.ToString();
			string r3 = res.ToJSONString(SciterXValue.VALUE_STRING_CVT_TYPE.CVT_JSON_LITERAL);

			{
				// http://sciter.com/forums/topic/erasing-sequence-elements-with-scitervalue/
				SciterValue sv = SciterValue.FromJSONString("[1,2,3,4,5])");
				sv[0] = SciterValue.Undefined;
				sv[2] = SciterValue.Undefined;

				SciterValue sv2 = SciterValue.FromJSONString("{one: 1, two: 2, three: 3}");
				sv2["one"] = SciterValue.Undefined;
				Assert.IsTrue(sv2["two"].Get(0)==2);
			}

			// Datetime
			{
				var now = DateTime.Now;
				SciterValue sv = new SciterValue(now);
				Assert.IsTrue(sv.GetDate() == now);
			}

			// SciterValue.AsDictionary
			{
				SciterValue sv = SciterValue.FromJSONString("{ a: 1, b: true }");
				var dic = sv.AsDictionary();
				Assert.IsTrue(dic.Count == 2);
			}
		}

		[TestMethod]
		public void TestGraphics()
		{
			var gapi = SciterX.GraphicsAPI;

			IntPtr himg;
			var a = gapi.imageCreate(out himg, 400, 400, true);

			IntPtr hgfx;
			gapi.gCreate(himg, out hgfx);

			IntPtr hpath;
			gapi.pathCreate(out hpath);

			var b = gapi.gPushClipPath(hgfx, hpath, 0.5f);
			var c = gapi.gPopClip(hgfx);

			// RGBA
			var rgba = new RGBAColor(1, 2, 3, 4);
			Assert.IsTrue(rgba.R == 1);
			Assert.IsTrue(rgba.G == 2);
			Assert.IsTrue(rgba.B == 3);
			Assert.IsTrue(rgba.A == 4);
		}


		private class TestableDOH : SciterDebugOutputHandler
		{
			public List<Tuple<SciterXDef.OUTPUT_SUBSYTEM, SciterXDef.OUTPUT_SEVERITY, string>> msgs = new List<Tuple<SciterXDef.OUTPUT_SUBSYTEM, SciterXDef.OUTPUT_SEVERITY, string>>();

			protected override void OnOutput(SciterXDef.OUTPUT_SUBSYTEM subsystem, SciterXDef.OUTPUT_SEVERITY severity, string text)
			{
				msgs.Add(Tuple.Create(subsystem, severity, text));
			}
		}

		[TestMethod]
		public void TestThings()
		{
			SciterWindow wnd = new SciterWindow();
			wnd.CreateMainWindow(1500, 800);
			wnd.Title = "Wtf";

			SciterHost host = new SciterHost(wnd);
			wnd.LoadHtml("<html></html>");

			var sv = wnd.EvalScript("Utils.readStreamToEnd");
			Assert.IsTrue(!sv.IsUndefined);
		}

		[TestMethod]
		public void TestODH()
		{
			TestableDOH odh = new TestableDOH();

			SciterWindow wnd = new SciterWindow();
			wnd.CreateMainWindow(1500, 800);
			wnd.Title = "Wtf";
			bool res = wnd.LoadHtml(@"
<html>
<style>
	body { wtf: 123; }
</style>

<script type='text/tiscript'>
</script>
</html>
");
			Assert.IsTrue(res);

			PInvokeWindows.MSG msg;
			while(PInvokeWindows.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
			{
				wnd.Show();
				PInvokeWindows.TranslateMessage(ref msg);
				PInvokeWindows.DispatchMessage(ref msg);
			}

			Assert.IsTrue(odh.msgs.Count == 1);
		}
	}
}