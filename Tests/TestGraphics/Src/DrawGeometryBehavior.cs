using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SciterSharp;
using SciterSharp.Interop;

namespace TestGraphics
{
	class DrawGeometryBehavior : SciterEventHandler
	{
		protected override bool OnDraw(SciterElement se, SciterXBehaviors.DRAW_PARAMS prms)
		{
			if(prms.cmd == SciterXBehaviors.DRAW_EVENTS.DRAW_CONTENT)
			{
				using(SciterGraphics g = new SciterGraphics(prms.gfx))
				{
					g.StateSave();
					g.Translate(prms.area.left, prms.area.top);

					List<Tuple<float, float>> points = new List<Tuple<float, float>>
					{
						Tuple.Create(100.0f, 0.0f),
						Tuple.Create(150.0f, 150.0f),
						Tuple.Create(50.0f, 150.0f)
					};

					g.LineColor = new RGBAColor(0, 0, 255);
					g.FillColor = new RGBAColor(255, 0, 0);
					g.LineWidth = 5;
					g.Polygon(points);
					g.Ellipse(200, 50, 50, 50);

					g.StateRestore();
				}

				return true;
			}
			return false;
		}
	}
}