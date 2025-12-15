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

        public void UpdateDisplay(Bitmap bitmap = null)
        {
            if (bitmap == null) // bitmap이 전달되지 않았으면
            {
                bitmap = Global.Inst.InspStage.GetBitmap(); // Global.Inst.InspStage에서 현재 Grab된 버퍼 이미지 획득
                if (bitmap == null) return; // 표시할 이미지가 없으면 종료
            }

            if(ImageViewer != null)
                ImageViewer.LoadBitmap(bitmap); // ImageViewer가 존재할 경우 Bitmap을 로드하여 화면 갱신
        }
        public Bitmap GetDisplayImage()
        {
            Bitmap curImage = null;

            if (ImageViewer != null) // ImageViewer가 존재하면
                curImage = ImageViewer.GetCurBitmap(); // 현재 화면에 표시 중인 Bitmap 반환

            return curImage;
        }
    }
}
