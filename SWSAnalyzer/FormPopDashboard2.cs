using DevExpress.XtraEditors;
using SWSAnalyzer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWSAnalyzer
{
	public partial class FormPopDashboard2 : DevExpress.XtraEditors.XtraForm
	{
		public FormPopDashboard2()
		{
			InitializeComponent();
		}

		private void FormPopDashboard2_Load(object sender, EventArgs e)
		{
			setImg();
		}

		private void setImg()
		{
			if (Program.Option.cultureName == "en")
				pictureEdit1.Image = Properties.Resources.Dashboard2EN;
			else
				pictureEdit1.Image = Properties.Resources.Dashboard2KR;
		}
	}
}