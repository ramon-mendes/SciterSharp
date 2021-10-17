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
using SciterSharp.WinForms;

namespace TestWinForms
{
    public partial class Form1 : Form
    {
        private SciterControl ctrl;

        public Form1()
        {
            InitializeComponent();

            ctrl = new SciterControl();
            ctrl.Dock = DockStyle.Fill;
			ctrl.HandleCreated += Ctrl_HandleCreated;
            Controls.Add(ctrl);
        }

		private void Ctrl_HandleCreated(object sender, EventArgs e)
		{
            ctrl.SciterWnd.LoadHtml("teste");
        }
	}
}
