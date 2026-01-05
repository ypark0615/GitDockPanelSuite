using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Setting;
using WeifenLuo.WinFormsUI.Docking;
//using WeifenLuo.WinFormsUI.Docking.VisualStyles; // 이전 방식. 현재에선 사용하지 않음

namespace GitDockPanelSuite
{
    public partial class MainForm : Form
    {
        private static DockPanel _dockPanel;
        public MainForm()
        {
            InitializeComponent();

            _dockPanel = new DockPanel // DockPanel 객체 생성
            {
                Dock = DockStyle.Fill // 폼 전체에 도킹
            };
            Controls.Add(_dockPanel); // 폼에 DockPanel 추가

            //VIsual Studio 2015 테마 적용
            _dockPanel.Theme = new VS2015BlueTheme();

            LoadDockingWindows();

            Global.Inst.Initialize();
        }

        private void LoadDockingWindows()
        {
            //도킹해제 금지 설정
            _dockPanel.AllowEndUserDocking = false;

            //메인폼 설정
            var cameraWindow = new CameraForm();
            cameraWindow.Show(_dockPanel, DockState.Document);

            //검사 결과창 30% 비율로 추가
            //ResultForm resultWindow = new ResultForm();
            //resultWindow.Show(camerWindow.Pane, DockAlignment.Bottom, 0.3);

            var runWindow = new RunForm();
            runWindow.Show(cameraWindow.Pane, DockAlignment.Bottom, 0.2);
            //runWindow.Show(cameraWindow.Pane, DockAlignment.Right, 0.3);

            //속성 창 추가
            var propWindow = new PropertiesForm();
            propWindow.Show(cameraWindow.Pane, DockAlignment.Right, 0.3);

            /*//속성창과 같은 탭에 추가하기
            var statisticWindow = new StatisticForm();
            statisticWindow.Show(_dockPanel, DockState.DockRight);

            //로그창 50% 비율로 추가
            var logWindow = new LogForm();
            logWindow.Show(propWindow.Pane, DockAlignment.Bottom, 0.5);*/
        }

        //제네릭 함수 사용를 이용해 입력된 타입의 폼 객체 얻기
        public static T GetDockForm<T>() where T : DockContent
        {
            var findForm = _dockPanel.Contents.OfType<T>().FirstOrDefault();
            return findForm;
        }

        private void imageOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraForm cameraForm = GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "이미지 파일 선택";
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    cameraForm.LoadImage(openFileDialog.FileName);
                }
            }
        }

        private void SetupMenuItem_Click(object sender, EventArgs e)
        {
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }
    }
}
