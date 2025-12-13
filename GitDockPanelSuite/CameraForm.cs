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
            if(bitmap == null)
            {
                bitmap = Global.Inst.InspStage.GetBitmap();
                if (bitmap == null) return;
            }

            if(ImageViewer != null)
                ImageViewer.LoadBitmap(bitmap);
        }
        public Bitmap GetDisplayImage()
        {
            Bitmap curImage = null;

            if (ImageViewer != null)
                curImage = ImageViewer.GetCurBitmap();

            return curImage;
        }
    }
}
