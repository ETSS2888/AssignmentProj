using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Assignment25
{
    public partial class frmRptLog : Form
    {
        public frmRptLog()
        {
            InitializeComponent();
        }

        private void frmRptLog_Load(object sender, EventArgs e)
        {
            rptLog rpt = new rptLog();
            crystalReportViewer1.ReportSource = rpt;
        }
    }
}
