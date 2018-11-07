using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SciterSharp;
using SciterSharp.Interop;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestGraphics
{
	class DrawBitmapBehavior : SciterEventHandler
	{
		protected override bool OnDraw(SciterElement se, SciterXBehaviors.DRAW_PARAMS prms)
		{
			var b = new Bitmap(406, 400);
			using(var g = Graphics.FromImage(b))
			{
				LinearGradientBrush linGrBrush = new LinearGradientBrush(
					new Point(0, 10),
					new Point(200, 10),
					Color.FromArgb(255, 255, 0, 0),   // Opaque red
					Color.FromArgb(255, 0, 0, 255));  // Opaque blue
				g.SmoothingMode = SmoothingMode.AntiAlias;
				g.FillEllipse(linGrBrush, 0, 30, 200, 100);
			}

			var img = new SciterImage(b);
			var gfx = new SciterGraphics(prms.gfx);
			gfx.BlendImage(img, 0, 0);
			return true;
		}
	}
}