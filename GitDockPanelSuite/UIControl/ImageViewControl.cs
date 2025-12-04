using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitDockPanelSuite
{
    public partial class ImageViewControl : UserControl
    {
        private bool _isInitialized = false;

        private Bitmap _bitmapImage = null; // 이미지 저장용 비트맵 객체

        private Bitmap Canvas = null; // 더블 버퍼링: 이미지가 깜빡이는 현상을 줄이기 위해 사용

        private RectangleF ImageRect = new RectangleF(0, 0, 0, 0); // 이미지가 그려질 영역. 이미지 그랩으로 인한 이동 및 줌에 따라 변경됨

        private float _curZoom = 1.0f; // 현재 줌 레벨
        private float _zoomFactor = 1.1f; // 줌 배율

        private float MinZoom = 1.0f; // 최소 줌 레벨
        private float MaxZoom = 100.0f; // 최대 줌 레벨

        private Rectangle _screenSelectedRect = Rectangle.Empty;

        public ImageViewControl()
        {
            InitializeComponent();
            InitializeCanvas();

            MouseWheel += new MouseEventHandler(ImageViewControl_MouseWheel);
        }

        private void InitializeCanvas()
        {
            ResizeCanvas();

            //DoubleBuffered = true;
        }

        private void ResizeCanvas()
        {
            if (Width <= 0 || Height <= 0 || _bitmapImage == null)
                return;

            Canvas = new Bitmap(Width, Height);
            if (Canvas == null)
                return;

            float virtualWidth = _bitmapImage.Width * _curZoom;
            float virtualHeight = _bitmapImage.Height * _curZoom;

            float offsetX = virtualWidth < Width ? (Width - virtualWidth) / 2 : 0;
            float offsetY = virtualHeight < Height ? (Height - virtualHeight) / 2 : 0;

            ImageRect = new RectangleF(offsetX, offsetY, virtualWidth, virtualHeight);
        }

        private void FitImageToScreen()
        {
            RecalcZoomRatio(); // 이미지가 스크린에 꽉 차는 줌 배율 계산

            float NewWidth = _bitmapImage.Width * _curZoom;
            float NewHeight = _bitmapImage.Width * _curZoom;


            // 이미지가 UserControl 중앙에 배치되도록 정렬
            ImageRect = new RectangleF(
                (Width - NewWidth) / 2,
                (Height - NewHeight) / 2,
                NewWidth,
                NewHeight
            );

            Invalidate();
        }

        private void RecalcZoomRatio()
        {
            if (_bitmapImage == null || Width <= 0 || Height <= 0)
                return;

            Size imageSize = new Size(_bitmapImage.Width, _bitmapImage.Height);

            float aspectRatio = (float) imageSize.Height / (float) imageSize.Width;
            float clientAspect = (float) Height / (float) Width;

            float ratio;
            if(aspectRatio <= clientAspect)
                ratio = (float) Width / (float) imageSize.Width;
            else
                ratio = (float) Height / (float) imageSize.Height;

            float minZoom = ratio;
            MinZoom = minZoom;

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, ratio));

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(_bitmapImage != null && Canvas != null)
            {
                using (Graphics g = Graphics.FromImage(Canvas))
                {
                    g.Clear(Color.Transparent);

                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(_bitmapImage, ImageRect);

                    // DrawDiagram(g);

                    e.Graphics.DrawImage(Canvas, 0, 0); // 화면에 표시
                }
            }
        }


        public void LoadBitmap(Bitmap bitmap)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<Bitmap>(LoadBitmap), bitmap);
                return;
            }

            if(_bitmapImage != null)
            {
                if(_bitmapImage.Width == bitmap.Width && _bitmapImage.Height == bitmap.Height)
                {
                    _bitmapImage.Dispose();
                    _bitmapImage = bitmap;
                    Invalidate();
                    return;
                }

                _bitmapImage.Dispose();
                _bitmapImage = null;
            }

            _bitmapImage = bitmap;

            if(_isInitialized == false)
            {
                _isInitialized = true;
                ResizeCanvas();
            }

            FitImageToScreen();
        }

        private void ImageViewControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FitImageToScreen();
        }

        private void ImageViewControl_Resize(object sender, EventArgs e)
        {
            ResizeCanvas();
            Invalidate();
        }



        private PointF GetScreenOffset()
        {
            return new PointF(ImageRect.X, ImageRect.Y);;
        }

        /* private Rectangle ScreenToVirtual(Rectangle screenRect)
        {
            PointF offset = GetScreenOffset();
            return new Rectangle(
                (int)((screenRect.X - offset.X) / _curZoom + 0.5f),
                (int)((screenRect.Y - offset.Y) / _curZoom + 0.5f),
                (int)(screenRect.Width / _curZoom + 0.5f),
                (int)(screenRect.Height / _curZoom + 0.5f)
            );
        }

        private Rectangle VirtualToScreen(Rectangle virtualRect)
        {
            PointF offset = GetScreenOffset();
            return new Rectangle(
                (int)(virtualRect.X * _curZoom + offset.X + 0.5f),
                (int)(virtualRect.Y * _curZoom + offset.Y + 0.5f),
                (int)(virtualRect.Width * _curZoom + 0.5f),
                (int)(virtualRect.Height * _curZoom + 0.5f));
        } */

        private PointF ScreenToVirtual(PointF screenPos)
        {
            PointF offset = GetScreenOffset();
            return new PointF(
                (screenPos.X - offset.X) / _curZoom,
                (screenPos.Y - offset.Y) / _curZoom);
        }

        private PointF VirtualToScreen(PointF virtualPos)
        {
            PointF offset = GetScreenOffset();
            return new PointF(
                virtualPos.X * _curZoom + offset.X,
                virtualPos.Y * _curZoom + offset.Y);
        }


        private void ImageViewControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta < 0)
                ZoomMove(_curZoom / _zoomFactor, e.Location);
            else
                ZoomMove(_curZoom * _zoomFactor, e.Location);

            if(_bitmapImage != null)
            {
                ImageRect.Width = _bitmapImage.Width * _curZoom;
                ImageRect.Height = _bitmapImage.Height * _curZoom;
            }
            
            Invalidate();
        }

        private void ZoomMove(float zoom, Point zoomOrigin)
        {
            PointF virtualOrigin = ScreenToVirtual(new PointF(zoomOrigin.X, zoomOrigin.Y));

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, zoom));
            if(_curZoom <= MinZoom) return;

            PointF zoomedOrigin = VirtualToScreen(virtualOrigin);

            float dx = zoomedOrigin.X - zoomOrigin.X;
            float dy = zoomedOrigin.Y - zoomOrigin.Y;

            ImageRect.X -= dx;
            ImageRect.Y -= dy;
        }
    }
}
