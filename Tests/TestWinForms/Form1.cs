using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SciterSharp;
using SciterSharp.Interop;

namespace TestWinForms
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			sciterControl1.HandleCreated += SciterControl1_HandleCreated;
		}

		private SciterWindow AppWnd;
		private Host AppHost = new Host();

		private void SciterControl1_HandleCreated(object sender, EventArgs e)
		{
			var vm = SciterX.API.SciterGetVM(sciterControl1.Handle);
			//AppWnd = new SciterWindow(sciterControl1.Handle);
			//AppHost.Setup(AppWnd);
		}
	}
}