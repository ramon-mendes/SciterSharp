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
	class DrawTextBehavior : SciterEventHandler
	{
		protected override bool OnDraw(SciterElement se, SciterXBehaviors.DRAW_PARAMS prms)
		{
			if(prms.cmd == SciterXBehaviors.DRAW_EVENTS.DRAW_CONTENT)
			{
				SciterText txt = SciterText.Create("hi", new SciterXGraphics.SCITER_TEXT_FORMAT
				{
					fontFamily = "Arial", 
					fontSize = 15,
					fontWeight = 400,
					lineHeight = 30
				});

				using(SciterGraphics g = new SciterGraphics(prms.gfx))
				{
					g.DrawText(txt, 0, 0, 7);
				}

				return true;
			}
			return false;
		}
	}
}