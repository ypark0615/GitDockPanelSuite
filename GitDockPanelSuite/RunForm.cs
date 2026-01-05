using GitDockPanelSuite.Core;
using GitDockPanelSuite.Grab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace GitDockPanelSuite
{
    public partial class RunForm : DockContent
    {
        public RunForm()
        {
            InitializeComponent();
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.Grab(0);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage.TryInspection();
        }

        private void chkLive_CheckedChanged(object sender, EventArgs e)
        {
            Global.Inst.InspStage.LiveMode = chkLive.Checked;

            if (Global.Inst.InspStage.LiveMode)
            {
                Global.Inst.InspStage.Grab(0);
            }
        }
    }
}
