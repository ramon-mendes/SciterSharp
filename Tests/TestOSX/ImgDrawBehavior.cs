using System;
using SciterSharp;
using AppKit;
using Foundation;
using CoreGraphics;

namespace TestOSX
{
	public class ImgDrawBehavior : SciterEventHandler
	{
		private SciterImage _simg;

		protected override void Attached(SciterElement se)
		{
			int width = 400;
			int height = 400;

			byte[] data = new byte[width * height * 4];
			var ctx = new CGBitmapContext(data, width, height, 8, width * 4, CGColorSpace.CreateGenericRgb(), CGImageAlphaInfo.PremultipliedLast);

			ctx.SetFillColor(new CGColor(100, 0, 0));
			ctx.SetStrokeColor(new CGColor(0, 0, 100));
			ctx.SetLineWidth(2);

			//ctx.AddPath(_svg._cgpath);
			//ctx.DrawPath(CGPathDrawingMode.FillStroke);

			_simg = new SciterImage(ctx.ToImage());
			//se.SetStyle("width", img.Width + "px");
			//se.SetStyle("height", img.Height + "px");
		}

		protected override bool OnDraw(SciterElement se, SciterSharp.Interop.SciterXBehaviors.DRAW_PARAMS prms)
		{
			if(prms.cmd != SciterSharp.Interop.SciterXBehaviors.DRAW_EVENTS.DRAW_CONTENT)
				return false;
			
			using(SciterGraphics gfx = new SciterGraphics(prms.gfx))
			{
				gfx.StateSave();
				gfx.Translate(prms.area.left, prms.area.top);

				gfx.FillColor = new RGBAColor(255, 0, 0);
				gfx.LineColor = RGBAColor.Black;
				gfx.LineWidth = 1;
				//gfx.DrawPath(_svg._spath, SciterSharp.Interop.SciterXGraphics.DRAW_PATH_MODE.DRAW_FILL_AND_STROKE);

				gfx.Translate(prms.area.left+10, prms.area.top+10);
				gfx.BlendImage(_simg, 0, 0);

				gfx.StateRestore();
			}
			return true;
		}
	}
}