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
        CameraType _cameraType;
        public RunForm()
        {
            InitializeComponent();

            ComboBox_CameraType.DataSource = Enum.GetValues(typeof(CameraType)).Cast<CameraType>().ToList();
            ComboBox_CameraType.SelectedIndex = 0;
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            if(_cameraType == CameraType.None)
            {
                MessageBox.Show("카메라 타입을 지정해주세요.");
                return;
            }

            Global.Inst.InspStage.Grab(0);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            Global.Inst.InspStage._camType = _cameraType;
            Global.Inst.InspStage.Initialize();
        }

        private void ComboBox_CameraType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _cameraType = (CameraType) ComboBox_CameraType.SelectedIndex;
        }
    }
}
