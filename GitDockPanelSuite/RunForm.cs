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
    public partial class RunForm: DockContent
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
                //#13_SET_IMAGE_BUFFER#4 그랩시 이미지 버퍼를 먼저 설정하도록 변경
                Global.Inst.InspStage.CheckImageBuffer();
                Global.Inst.InspStage.Grab(0);
            }
        }
    }
}
