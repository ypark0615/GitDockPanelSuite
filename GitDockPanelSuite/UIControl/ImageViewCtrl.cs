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
using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Teach;

namespace GitDockPanelSuite
{
    public enum EntityActionType
    {
        None = 0,
        Select,
        Inspect,
        Add,
        Copy,
        Move,
        Resize,
        Delete,
        DeleteList,
        UpdateImage
    }

    public partial class ImageViewCtrl : UserControl
    {
        public event EventHandler<DiagramEntityEventArgs> DiagramEntityEvent;

        private bool _isInitialized = false;

        private Bitmap _bitmapImage = null; // 이미지 저장용 비트맵 객체

        private Bitmap Canvas = null; // 더블 버퍼링: 이미지가 깜빡이는 현상을 줄이기 위해 사용

        private RectangleF ImageRect = new RectangleF(0, 0, 0, 0); // 이미지가 그려질 영역. 이미지 그랩으로 인한 이동 및 줌에 따라 변경됨

        private float _curZoom = 1.0f; // 현재 줌 레벨
        private float _zoomFactor = 1.1f; // 줌 배율

        private float MinZoom = 1.0f; // 최소 줌 레벨
        private float MaxZoom = 100.0f; // 최대 줌 레벨

        private List<DrawInspectInfo> _rectInfos = new List<DrawInspectInfo>();


        /* ROI 편집에 필요한 변수 선언 */
        private Point _roiStart = Point.Empty; // ROI 선택 시작 위치
        private Rectangle _roiRect = Rectangle.Empty; // 현재 생성하거나 편집중인 ROI 영역
        private bool _isSelectingRoi = false; // ROI 선택 중인지 여부
        private bool _isResizingRoi = false; // ROI 리사이즈 중인지 여부
        private bool _isMovingRoi = false; // ROI 이동 중인지 여부
        private Point _resizeStart = Point.Empty; // 리사이즈 시작 위치
        private Point _moveStart = Point.Empty; // 이동 시작 위치
        private int _resizeDirection = -1; // 리사이즈 방향
        private const int _ResizeHandleSize = 10; // 리사이즈 핸들 크기

        private InspWindowType _newRoiType = InspWindowType.None; // 새로 추가되는 ROI의 타입

        private List<DiagramEntity> _diagramEntityList = new List<DiagramEntity>(); // 전체 ROI 리스트

        private List<DiagramEntity> _multiSelectedEntities = new List<DiagramEntity>(); // 현재 선택된 ROI 리스트
        private List<DiagramEntity> _copyBuffer = new List<DiagramEntity>(); // 복사된 ROI 리스트
        private Point _mousePos; // 마우스 현재 위치

        private DiagramEntity _selEntity; // 현재 선택된 ROI
        private Color _selColor = Color.White; // 현재 선택된 ROI의 색상

        /* 복사를 위한 변수 */
        private Rectangle _selectionBox = Rectangle.Empty; // 선택 박스
        private bool _isBoxSelecting = false; // 선택 중인지 여부
        private bool _isCtrlPressed = false; // Ctrl 키 눌림 여부
        private Rectangle _screenSelectedRect = Rectangle.Empty; // 화면 좌표계에서 선택된 영역

        private Size _extSize = new Size(0, 0); // 이미지 외곽 여백 크기

        private ContextMenuStrip _contextMenu; // 팝업 메뉴


        public ImageViewCtrl()
        {
            InitializeComponent();
            InitializeCanvas();

            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Delete", null, OnDeleteClicked);
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("Teaching", null, OnTeachingClicked);
            _contextMenu.Items.Add("Unlock", null, OnUnlockClicked);


            MouseWheel += new MouseEventHandler(ImageViewCtrl_MouseWheel);
        }

        private void InitializeCanvas()
        {
            ResizeCanvas();

            //DoubleBuffered = true;
        }

        public Bitmap GetCurBitmap()
        {
            return _bitmapImage;
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

            float aspectRatio = (float)imageSize.Height / (float)imageSize.Width;
            float clientAspect = (float)Height / (float)Width;

            float ratio;
            if (aspectRatio <= clientAspect)
                ratio = (float)Width / (float)imageSize.Width;
            else
                ratio = (float)Height / (float)imageSize.Height;

            float minZoom = ratio;
            MinZoom = minZoom;

            _curZoom = Math.Max(MinZoom, Math.Min(MaxZoom, ratio));

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_bitmapImage != null && Canvas != null)
            {
                using (Graphics g = Graphics.FromImage(Canvas))
                {
                    g.Clear(Color.Transparent);

                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(_bitmapImage, ImageRect);

                    DrawDiagram(g);

                    e.Graphics.DrawImage(Canvas, 0, 0); // 화면에 표시
                }
            }
        }

        public void DrawDiagram(Graphics g)
        {
            if (_rectInfos != null)
            {
                foreach (DrawInspectInfo rectInfo in _rectInfos)
                {
                    Color lineColor = Color.LightCoral;
                    if (rectInfo.decision == DecisionType.Defect)
                        lineColor = Color.Red;
                    else if (rectInfo.decision == DecisionType.Good)
                        lineColor = Color.LightGreen;

                    Rectangle rect = new Rectangle(rectInfo.rect.X, rectInfo.rect.Y, rectInfo.rect.Width, rectInfo.rect.Height);
                    Rectangle screenRect = VirtualToScreen(rect);

                    using (Pen pen = new Pen(lineColor, 2))
                    {
                        if (rectInfo.UseRotatedRect)
                        {
                            PointF[] screenPoints = rectInfo.rotatedPoints.Select(p => VirtualToScreen(new PointF(p.X, p.Y))).ToArray();

                            if (screenPoints.Length == 4)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    g.DrawLine(pen, screenPoints[i], screenPoints[(i + 1) % 4]);
                                }
                            }
                        }
                        else
                        {
                            g.DrawRectangle(pen, screenRect);
                        }
                    }

                    if (rectInfo.info != "")
                    {
                        float baseFontSize = 20.0f;

                        if (rectInfo.decision == DecisionType.Info)
                        {
                            baseFontSize = 3.0f;
                            lineColor = Color.LightBlue;
                        }

                        float fontSize = baseFontSize * _curZoom;

                        string infoText = rectInfo.info;
                        PointF textPos = new PointF(screenRect.Left, screenRect.Top); // 위로 약간 띄우기

                        if (rectInfo.inspectType == InspectType.InspBinary
                            && rectInfo.decision != DecisionType.Info)
                        {
                            textPos.Y = screenRect.Bottom - fontSize;
                        }

                        DrawText(g, infoText, textPos, fontSize, lineColor);
                    }
                }
            }
        }

        private void DrawText(Graphics g, string text, PointF position, float fontSize, Color color)
        {
            using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
            // 테두리용 검정색 브러시
            using (Brush outlineBrush = new SolidBrush(Color.Black))
            // 본문용 노란색 브러시
            using (Brush textBrush = new SolidBrush(color))
            {
                // 테두리 효과를 위해 주변 8방향으로 그리기
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue; // 가운데는 제외
                        PointF borderPos = new PointF(position.X + dx, position.Y + dy);
                        g.DrawString(text, font, outlineBrush, borderPos);
                    }
                }

                // 본문 텍스트
                g.DrawString(text, font, textBrush, position);
            }
        }


        private void AddSelectedROI(DiagramEntity entity)
        {
            if (entity == null)
                return;
            if (!_multiSelectedEntities.Contains(entity))
                _multiSelectedEntities.Add(entity);
        }




        public void LoadBitmap(Bitmap bitmap)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<Bitmap>(LoadBitmap), bitmap);
                return;
            }

            if (_bitmapImage != null)
            {
                if (_bitmapImage.Width == bitmap.Width && _bitmapImage.Height == bitmap.Height)
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

            if (_isInitialized == false)
            {
                _isInitialized = true;
                ResizeCanvas();
            }

            FitImageToScreen();
        }

        private void ImageViewCtrl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FitImageToScreen();
        }

        private void ImageViewCtrl_Resize(object sender, EventArgs e)
        {
            ResizeCanvas();
            Invalidate();
        }



        private PointF GetScreenOffset()
        {
            return new PointF(ImageRect.X, ImageRect.Y); ;
        }

        private Rectangle ScreenToVirtual(Rectangle screenRect)
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
        }

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


        private void ImageViewCtrl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                ZoomMove(_curZoom / _zoomFactor, e.Location);
            else
                ZoomMove(_curZoom * _zoomFactor, e.Location);

            if (_bitmapImage != null)
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
            if (_curZoom <= MinZoom) return;

            PointF zoomedOrigin = VirtualToScreen(virtualOrigin);

            float dx = zoomedOrigin.X - zoomOrigin.X;
            float dy = zoomedOrigin.Y - zoomOrigin.Y;

            ImageRect.X -= dx;
            ImageRect.Y -= dy;
        }
        public void AddRect(List<DrawInspectInfo> rectInfos)
        {
            _rectInfos.AddRange(rectInfos);
            Invalidate();
        }

        public void ResetEntity()
        {
            _rectInfos.Clear();
            Invalidate();
        }

        public bool SetDiagramEntityList(List<DiagramEntity> diagramEntitiyList)
        {
            _diagramEntityList = diagramEntitiyList.OrderBy(r => r.EntityROI.Width * r.EntityROI.Height).ToList();
            _selEntity = null;
            Invalidate();
            return true;
        }

        public void SelectDiagramEntity(InspWindow window)
        {
            DiagramEntity entity = _diagramEntityList.Find(e => e.LinkedWindow == window);
            if (entity != null)
            {
                _multiSelectedEntities.Clear();
                AddSelectedROI(entity);

                _selEntity = entity;
                _roiRect = entity.EntityROI;
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            DeleteSelEntity();
        }

        private void OnTeachingClicked(object sender, EventArgs e)
        {
            if (_selEntity is null) return;

            InspWindow window = _selEntity.LinkedWindow;

            if (window is null) return;

            window.IsTeach = true;
            _selEntity.IsHold = true;
        }

        private void OnUnlockClicked(object sender, EventArgs e)
        {
            if (_selEntity is null) return;

            InspWindow window = _selEntity.LinkedWindow;

            if (window is null) return;

            _selEntity.IsHold = false;
        }


        private void DeleteSelEntity()
        {
            List<InspWindow> selected = _multiSelectedEntities.Where(d => d.LinkedWindow != null).Select(d => d.LinkedWindow).ToList();

            if (selected.Count > 0)
            {
                DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.DeleteList, selected));
                return;
            }

            if(_selEntity != null)
            {
                InspWindow linkedWindow = _selEntity.LinkedWindow;
                if (linkedWindow is null) return;

                DiagramEntityEvent?.Invoke(this, new DiagramEntityEventArgs(EntityActionType.Delete, linkedWindow));
            }
        }


    }


    #region EventArgs
    public class DiagramEntityEventArgs : EventArgs
    {
        public EntityActionType ActionType { get; private set; }

        public InspWindow InspWindow { get; private set; }

        public InspWindowType WindowType { get; private set; }

        public List<InspWindow> InspWindowList { get; private set; }

        public OpenCvSharp.Rect Rect { get; private set; }

        public OpenCvSharp.Point OffsetMove { get; private set; }

        public DiagramEntityEventArgs(EntityActionType actionType, InspWindow inspWindow)
        {
            ActionType = actionType;
            InspWindow = inspWindow;
        }

        public DiagramEntityEventArgs(EntityActionType actionType, InspWindow inspWindow, InspWindowType windowType, Rectangle rect, Point offsetMove)
        {
            ActionType = actionType;
            InspWindow = inspWindow;
            WindowType = windowType;
            Rect = new OpenCvSharp.Rect(rect.X, rect.Y, rect.Width, rect.Height);
            OffsetMove = new OpenCvSharp.Point(offsetMove.X, offsetMove.Y);
        }

        public DiagramEntityEventArgs(EntityActionType actionType, List<InspWindow> inspWindowList, InspWindowType windowType = InspWindowType.None)
        {
            ActionType = actionType;
            InspWindow = null;
            InspWindowList = inspWindowList;
            WindowType = windowType;
        }
    }
    
    #endregion
}
