using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciterSharp.Interop;

namespace SciterSharp.WinForms
{
	/// <summary>
	/// Represents a SciterWindow control.
	/// </summary>
	public class SciterControl : Control
	{
		public SciterWindow SciterWnd { get; private set; }

		public SciterControl()
		{
			SciterWnd = new SciterWindow();
		}
		
		#region Overrided Methods
		protected override void OnHandleCreated(EventArgs e)
		{
			SciterWnd.CreateChildWindow(Handle);
			SciterWnd.LoadHtml(
				"<body>" +
					"<code>Add an event handler to the <b>HandleCreated</b> event for any needed initialization (e.g.: load the HTML)</code><br /><br />" +
					"<code>In the handler, use the <b>SciterWnd</b> property of this control to access the SciterWindow instance.</code>" +
				"</body>"
				);
			SciterWnd.Show();
			base.OnHandleCreated(e);
		}

		protected override void OnClientSizeChanged(EventArgs e)
		{
			if(SciterWnd._hwnd.ToInt32()!=0)
			{
				var sz = this.Size;
				PInvokeWindows.MoveWindow(SciterWnd._hwnd, 0, 0, sz.Width, sz.Height, true);
			}
			base.OnClientSizeChanged(e);
		}
		#endregion
	}
}