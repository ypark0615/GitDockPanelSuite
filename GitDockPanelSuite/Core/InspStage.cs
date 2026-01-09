using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Grab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using GitDockPanelSuite.Setting;
using GitDockPanelSuite.Teach;

namespace GitDockPanelSuite.Core
{
    public class InspStage : IDisposable // 검사 스테이지(Inspection Stage)
    {
        public static readonly int MAX_GRAB_BUF = 5; // 전역적으로 공유되는 최대 Grab 버퍼 수

        private ImageSpace _imageSpace = null; // Grab된 원본 이미지 및 분할 이미지 관리
        //private HikRobotCam _grabManager = null; // HikRobot 카메라 제어 클래스
        private GrabModel _grabManager = null;
        public CameraType _camType = CameraType.WebCam;
        SaigeAI _saigeAI; // AI 모듈

        BlobAlgorithm _blobAlgorithm = null; // 블롭 알고리즘 인스턴스
        private PreviewImage _previewImage = null; // 미리보기 이미지 변수

        private Model _model = null;

        private InspWindow _selectedInspWindow = null;


        public InspStage() { }
        public ImageSpace ImageSpace // 외부에서 ImageSpace 객체를 직접 조작 가능
        {
            get => _imageSpace;
        }

        public SaigeAI AIModule // AI 모듈 접근 시 최초 1회 생성
        {
            get
            {
                if (_saigeAI is null)
                    _saigeAI = new SaigeAI();
                return _saigeAI;
            }
        }

        public BlobAlgorithm BlobAlgorithm
        {
            get => _blobAlgorithm;
        }

        public PreviewImage PreView
        {
            get => _previewImage;
        }

        public Model CurModel
        {
            get => _model;
        }

        public bool LiveMode { get; set; } = false; // 라이브 모드 여부

        public bool Initialize()
        {
            _imageSpace = new ImageSpace(); // ImageSpace 생성

            //_blobAlgorithm = new BlobAlgorithm(); // BlobAlgorithm 생성
            _previewImage = new PreviewImage(); // PreviewImage 생성

            _model = new Model(); // Model 생성

            LoadSetting();

            switch (_camType)
            {
                case CameraType.WebCam:
                    {
                        _grabManager = new WebCam(); // WebCam 생성
                        break;
                    }
                case CameraType.HikRobotCam:
                    {
                        _grabManager = new HikRobotCam(); // HikRobotCam 생성
                        break;
                    }
            }


            if (_grabManager != null && _grabManager.InitGrab() == true) // Grab 초기화
            {
                _grabManager.TransferCompleted += _multiGrab_TransferCompleted; // Grab 성공 시 이벤트 연결

                InitModelGrab(MAX_GRAB_BUF);
            }

            return true;
        }

        private void LoadSetting()
        {
            //카메라 설정 타입 얻기
            _camType = SettingXml.Inst.CamType;
        }

        public void InitModelGrab(int bufferCount) // 버퍼 개수 설정
        {
            if (_grabManager == null) return; // GrabManager null 체크

            int pixelBpp = 8; // Pixel BPP
            _grabManager.GetPixelBpp(out pixelBpp); // 현재 이미지에서 Pixel BPP 받아오기

            int inspectionWidth;
            int inspectionHeight;
            int inspectStride;
            _grabManager.GetResolution(out inspectionWidth, out inspectionHeight, out inspectStride); // 해상도/Stride 받아오기

            if (_imageSpace != null)
            {
                // ImageSpace에 이미지 정보 설정
                _imageSpace.SetImageInfo(pixelBpp, inspectionWidth, inspectionHeight, inspectStride);
            }

            SetBuffer(bufferCount); // 버퍼 개수 설정


            //UpdateProperty();
        }

        public void UpdateProperty(InspWindow inspWindow)
        {
            if (inspWindow is null)
                return;

            PropertiesForm propertiesForm = MainForm.GetDockForm<PropertiesForm>();
            if (propertiesForm is null)
                return;

            propertiesForm.UpdateProperty(inspWindow);
        }

        public void SetBuffer(int bufferCount)
        {
            if (_grabManager == null) return; // GrabManager null 체크

            if (_imageSpace.BufferCount == bufferCount) return; // 현재 연결된 버퍼 수와 입력받은 버퍼 수가 동일하면 return

            _imageSpace.InitImageSpace(bufferCount); // ImageSpace 버퍼 초기화
            _grabManager.InitBuffer(bufferCount); // GrabManager 버퍼 초기화

            for (int i = 0; i < bufferCount; i++) // 각 버퍼를 카메라에 바인딩
            {
                _grabManager.SetBuffer(
                    _imageSpace.GetInspectionBuffer(i),
                    _imageSpace.GetnspectionBufferPtr(i),
                    _imageSpace.GetInspectionBufferHandle(i),
                    i
                );
            }
        }


        public void TryInspection(InspWindow inspWindow)
        {
            if (inspWindow is null)
            {
                if (_selectedInspWindow is null)
                    return;

                inspWindow = _selectedInspWindow;
            }

            UpdateDiagramEntity();

            List<DrawInspectInfo> totalArea = new List<DrawInspectInfo>();

            Rect windowArea = inspWindow.WindowArea;

            foreach (var inspAlgo in inspWindow.AlgorithmList)
            {
                inspAlgo.TeachRect = windowArea;
                inspAlgo.InspRect = windowArea;

                InspectType insptype = inspAlgo.InspectType;

                switch (insptype)
                {
                    case InspectType.InspBinary:
                        {
                            BlobAlgorithm blobAlgo = (BlobAlgorithm)inspAlgo;

                            Mat srcImage = Global.Inst.InspStage.GetMat();
                            blobAlgo.SetInspData(srcImage);

                            break;
                        }
                }

                if (inspAlgo.DoInspect())
                {
                    List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                    int resultCnt = inspAlgo.GetResultRect(out resultArea);
                    if (resultCnt > 0)
                        totalArea.AddRange(resultArea);
                }
            }
        }

        public void SelectInspWindow(InspWindow inspWindow)
        {
            _selectedInspWindow = inspWindow;

            var propForm = MainForm.GetDockForm<PropertiesForm>();

            if (propForm != null)
            {
                if (inspWindow is null)
                {
                    propForm.ResetProperty();
                    return;
                }

                propForm.ShowProperty(inspWindow);
            }

            UpdateProperty(inspWindow);

            Global.Inst.InspStage.PreView.SetInspWindow(inspWindow);
        }

        /*private bool DisplayResult()
        {
            if (_blobAlgorithm is null)
                return false;

            List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
            int resultCnt = _blobAlgorithm.GetResultRect(out resultArea);
            if (resultCnt > 0)
            {
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null)
                {
                    cameraForm.ResetDisplay();
                    cameraForm.AddRect(resultArea);
                }
            }

            return true;
        }*/

        public void Grab(int bufferIndex) // 지정된 버퍼로 Grab 실행
        {
            if (_grabManager == null) return;

            _grabManager.Grab(bufferIndex, true);
        }

        private async void _multiGrab_TransferCompleted(object sender, object e)
        {
            int bufferIndex = (int)e;
            Console.WriteLine($"_multiGrab_TransferCompleted {bufferIndex}");

            _imageSpace.Split(bufferIndex); // Grab된 이미지 분할 처리
            DisplayGrabImage(bufferIndex);  // 화면 갱신

            if (_previewImage != null)
            {
                Bitmap bitmap = ImageSpace.GetBitmap(0);
                _previewImage.SetImage(BitmapConverter.ToMat(bitmap));
            }

            if (LiveMode)
            {
                await Task.Delay(100);
                _grabManager.Grab(bufferIndex, true);
            }
        }

        private void DisplayGrabImage(int bufferIndex) // Grab한 이미지 띄우기
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDisplay();
            }
        }

        public void UpdateDisplay(Bitmap bitmap) // 외부 Bitmap으로 화면 갱신
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDisplay(bitmap);
            }
        }

        public Bitmap GetCurrentImage() // 현재 화면에 표시 중인 이미지 반환
        {
            Bitmap bitmap = null;
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                bitmap = cameraForm.GetDisplayImage();
            }

            return bitmap;
        }

        public Bitmap GetBitmap(int bufferIndex = -1)
        {
            // Global을 통해 다시 InspStage → ImageSpace 접근
            if (Global.Inst.InspStage.ImageSpace is null)
                return null;

            return Global.Inst.InspStage.ImageSpace.GetBitmap();
        }

        public Mat GetMat()
        {
            return Global.Inst.InspStage.ImageSpace.GetMat();
        }

        public void UpdateDiagramEntity()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDiagramEntity();
            }

            /*ModelTreeForm modelTreeForm = MainForm.GetDockForm<ModelTreeForm>();
            if (modelTreeForm != null)
            {
                modelTreeForm.UpdateDiagramEntity();
            }*/
        }

        public void RedrawMainView()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateImageViewer();
            }
        }

        #region Disposable
        private bool disposed = false; // Dispose 호출 여부

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_saigeAI != null)
                    {
                        _saigeAI.Dispose(); // AI 모듈 해제
                        _saigeAI = null;
                    }

                    if (_grabManager != null)
                    {
                        _grabManager.Dispose(); // 카메라 Grab 리소스 해제
                        _grabManager = null;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}