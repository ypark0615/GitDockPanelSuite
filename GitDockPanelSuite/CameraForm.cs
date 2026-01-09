using GitDockPanelSuite.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using OpenCvSharp;
using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Teach;

namespace GitDockPanelSuite
{
    public partial class CameraForm : DockContent
    {
        public CameraForm()
        {
            InitializeComponent();
        }

        public void LoadImage(string filePath)
        {
            if(File.Exists(filePath) == false) return;

            Image bitmap = Image.FromFile(filePath);
            ImageViewer.LoadBitmap(new Bitmap(bitmap));
        }

        private void CameraForm_Resize(object sender, EventArgs e)
        {
            int margin = 0;
            ImageViewer.Width = this.Width - margin * 2;
            ImageViewer.Height = this.Height - margin * 2;

            ImageViewer.Location = new System.Drawing.Point(margin, margin);
        }
        public void UpdateDiagramEntity()
        {
            imageViewer.ResetEntity();

            Model model = Global.Inst.InspStage.CurModel;
            List<DiagramEntity> diagramEntityList = new List<DiagramEntity>();

            foreach (InspWindow window in model.InspWindowList)
            {
                if (window is null)
                    continue;

                DiagramEntity entity = new DiagramEntity()
                {
                    LinkedWindow = window,
                    EntityROI = new Rectangle(
                        window.WindowArea.X, window.WindowArea.Y,
                            window.WindowArea.Width, window.WindowArea.Height),
                    EntityColor = imageViewer.GetWindowColor(window.InspWindowType),
                    IsHold = window.IsTeach
                };
                diagramEntityList.Add(entity);
            }

            imageViewer.SetDiagramEntityList(diagramEntityList);
        }

        public void SelectDiagramEntity(InspWindow window)
        {
            imageViewer.SelectDiagramEntity(window);
        }

        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (bitmap == null) // bitmap이 전달되지 않았으면
            {
                bitmap = Global.Inst.InspStage.GetBitmap(); // Global.Inst.InspStage에서 현재 Grab된 버퍼 이미지 획득
                if (bitmap == null) return; // 표시할 이미지가 없으면 종료
            }

            if(ImageViewer != null)
                ImageViewer.LoadBitmap(bitmap); // ImageViewer가 존재할 경우 Bitmap을 로드하여 화면 갱신
        
            Mat curImage = Global.Inst.InspStage.GetMat();
            Global.Inst.InspStage.PreView.SetImage(curImage);
        }
        public Bitmap GetDisplayImage()
        {
            Bitmap curImage = null;

            if (ImageViewer != null) // ImageViewer가 존재하면
                curImage = ImageViewer.GetCurBitmap(); // 현재 화면에 표시 중인 Bitmap 반환

            return curImage;
        }

        public void UpdateImageViewer()
        {
            ImageViewer.Invalidate(); // ImageViewer 컨트롤을 무효화하여 다시 그리도록 요청
        }
        public void ResetDisplay()
        {
            ImageViewer.ResetEntity();
        }

        public void AddRect(List<DrawInspectInfo> rectInfos)
        {
            ImageViewer.AddRect(rectInfos);
        }
    }
}
