using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Grab;
using GitDockPanelSuite.Teach;
using GitDockPanelSuite.Setting;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using GitDockPanelSuite.Inspect;
using GitDockPanelSuite.Util;
using Microsoft.Win32;

namespace GitDockPanelSuite.Core
{
    public class InspStage : IDisposable // 검사 스테이지(Inspection Stage)
    {
        public static readonly int MAX_GRAB_BUF = 1; // 전역적으로 공유되는 최대 Grab 버퍼 수

        private ImageSpace _imageSpace = null; // Grab된 원본 이미지 및 분할 이미지 관리

        //private HikRobotCam _grabManager = null; // HikRobot 카메라 제어 클래스
        private GrabModel _grabManager = null;
        private CameraType _camType = CameraType.WebCam;
        
        SaigeAI _saigeAI; // AI 모듈

        //  BlobAlgorithm _blobAlgorithm = null; // 블롭 알고리즘 인스턴스
        private PreviewImage _previewImage = null; // 미리보기 이미지 변수

        private Model _model = null;

        private InspWindow _selectedInspWindow = null;


        private InspWorker _inspWorker = null;
        private ImageLoader _imageLoader = null;

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

        public PreviewImage PreView
        {
            get => _previewImage;
        }

        public InspWorker InspWorker
        {
            get => _inspWorker;
        }


        public Model CurModel
        {
            get => _model;
        }

        public bool LiveMode { get; set; } = false; // 라이브 모드 여부

        public int SelBufferIndex { get; set; } = 0;
        public eImageChannel SelImageChannel { get; set; } = eImageChannel.Gray;


        public bool Initialize()
        {
            SLogger.Write("InspStage 초기화!");
            _imageSpace = new ImageSpace(); // ImageSpace 생성

            //_blobAlgorithm = new BlobAlgorithm(); // BlobAlgorithm 생성
            _previewImage = new PreviewImage(); // PreviewImage 생성

            _inspWorker = new InspWorker();
            _imageLoader = new ImageLoader();

            //_regKey = Registry.CurrentUser.CreateSubKey("Software\\GitDockPanelSuite");

            _model = new Model(); // Model 생성

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


            if (_grabManager != null && _grabManager.InitGrab()) // Grab 초기화
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
            int inspectionStride;
            _grabManager.GetResolution(out inspectionWidth, out inspectionHeight, out inspectionStride); // 해상도/Stride 받아오기

            if (_imageSpace != null)
            {
            	// ImageSpace에 이미지 정보 설정
                _imageSpace.SetImageInfo(pixelBpp, inspectionWidth, inspectionHeight, inspectionStride);
            }

            SetBuffer(bufferCount); // 버퍼 개수 설정

            //_grabManager.SetExposureTime(25000);
        }
        public void SetImageBuffer(string filePath)
        {
            SLogger.Write($"Load Image : {filePath}");

            Mat matImage = Cv2.ImRead(filePath);

            int pixelBpp = 8;
            int imageWidth;
            int imageHeight;
            int imageStride;

            if (matImage.Type() == MatType.CV_8UC3)
                pixelBpp = 24;

            imageWidth = (matImage.Width + 3) / 4 * 4;
            imageHeight = matImage.Height;

            // 4바이트 정렬된 새로운 Mat 생성
            Mat alignedMat = new Mat();
            Cv2.CopyMakeBorder(matImage, alignedMat, 0, 0, 0, imageWidth - matImage.Width, BorderTypes.Constant, Scalar.Black);

            imageStride = imageWidth * matImage.ElemSize();

            if (_imageSpace != null)
            {
                _imageSpace.SetImageInfo(pixelBpp, imageWidth, imageHeight, imageStride);
            }

            SetBuffer(1);

            int bufferIndex = 0;

            // Mat의 데이터를 byte 배열로 복사
            int bufSize = (int)(alignedMat.Total() * alignedMat.ElemSize());
            Marshal.Copy(alignedMat.Data, ImageSpace.GetInspectionBuffer(bufferIndex), 0, bufSize);

            _imageSpace.Split(bufferIndex);

            DisplayGrabImage(bufferIndex);

            if (_previewImage != null)
            {
                Bitmap bitmap = ImageSpace.GetBitmap(0);
                _previewImage.SetImage(BitmapConverter.ToMat(bitmap));
            }
        }

        public void CheckImageBuffer()
        {
            if (_grabManager != null && SettingXml.Inst.CamType != CameraType.None)
            {
                int imageWidth;
                int imageHeight;
                int imageStride;
                _grabManager.GetResolution(out imageWidth, out imageHeight, out imageStride);

                if (_imageSpace.ImageSize.Width != imageWidth || _imageSpace.ImageSize.Height != imageHeight)
                {
                    int pixelBpp = 8;
                    _grabManager.GetPixelBpp(out pixelBpp);

                    _imageSpace.SetImageInfo(pixelBpp, imageWidth, imageHeight, imageStride);
                    SetBuffer(_imageSpace.BufferCount);
                }
            }
        }


        //#10_INSPWINDOW#11 속성창 업데이트 기준을 알고리즘에서 InspWindow로 변경
        private void UpdateProperty(InspWindow inspWindow)
        {
            if (inspWindow is null)
                return;

            PropertiesForm propertiesForm = MainForm.GetDockForm<PropertiesForm>();
            if (propertiesForm is null)
                return;

            propertiesForm.UpdateProperty(inspWindow);
        }

        //#11_MATCHING#6 패턴매칭 속성창과 연동된 패턴 이미지 관리 함수
        public void UpdateTeachingImage(int index)
        {
            if (_selectedInspWindow is null)
                return;

            SetTeachingImage(_selectedInspWindow, index);
        }

        public void DelTeachingImage(int index)
        {
            if (_selectedInspWindow is null)
                return;

            InspWindow inspWindow = _selectedInspWindow;

            inspWindow.DelWindowImage(index);

            MatchAlgorithm matchAlgo = (MatchAlgorithm)inspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                UpdateProperty(inspWindow);
            }
        }

        public void SetTeachingImage(InspWindow inspWindow, int index = -1)
        {
            if (inspWindow is null)
                return;

            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm is null)
                return;

            Mat curImage = cameraForm.GetDisplayImage();
            if (curImage is null)
                return;

            if (inspWindow.WindowArea.Right >= curImage.Width ||
                inspWindow.WindowArea.Bottom >= curImage.Height)
            {
                SLogger.Write("ROI 영역이 잘못되었습니다!");
                return;
            }

            Mat windowImage = curImage[inspWindow.WindowArea];

            if (index < 0)
                inspWindow.AddWindowImage(windowImage);
            else
                inspWindow.SetWindowImage(windowImage, index);

            inspWindow.IsPatternLearn = false;

            MatchAlgorithm matchAlgo = (MatchAlgorithm)inspWindow.FindInspAlgorithm(InspectType.InspMatch);
            if (matchAlgo != null)
            {
                UpdateProperty(inspWindow);
            }
        }

        public void SetBuffer(int bufferCount)
        {
            _imageSpace.InitImageSpace(bufferCount);

            if (_grabManager != null)
            {
                _grabManager.InitBuffer(bufferCount);

                for (int i = 0; i < bufferCount; i++)
                {
                    _grabManager.SetBuffer(
                        _imageSpace.GetInspectionBuffer(i),
                        _imageSpace.GetnspectionBufferPtr(i),
                        _imageSpace.GetInspectionBufferHandle(i),
                        i);
                }
            }

            SLogger.Write("버퍼 초기화 성공!");
        }


        public void TryInspection(InspWindow inspWindow = null)
        {
            if (inspWindow is null)
            {
                if (_selectedInspWindow is null)
                    return;

                inspWindow = _selectedInspWindow;
            }

            UpdateDiagramEntity();

            inspWindow.ResetInspResult();

            List<DrawInspectInfo> totalArea = new List<DrawInspectInfo>();

            Rect windowArea = inspWindow.WindowArea;

            foreach (var inspAlgo in inspWindow.AlgorithmList)
            {
                if (!inspAlgo.IsUse)
                    continue;

                //검사 영역 초기화
                inspAlgo.TeachRect = windowArea;
                inspAlgo.InspRect = windowArea;

                Mat srcImage = Global.Inst.InspStage.GetMat();
                inspAlgo.SetInspData(srcImage);

                if(!inspAlgo.DoInspect()) continue;

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                int resultCnt = inspAlgo.GetResultRect(out resultArea);
                if (resultCnt > 0)
                {
                    totalArea.AddRange(resultArea);
                }

                InspectType inspType = inspAlgo.InspectType;

                string resultInfo = string.Join("\r\n", inspAlgo.ResultString);

                InspResult inspResult = new InspResult
                {
                    ObjectID = inspWindow.UID,
                    InspType = inspAlgo.InspectType,
                    IsDefect = inspAlgo.IsDefect,
                    ResultInfos = resultInfo
                };

                switch (inspType)
                {
                    case InspectType.InspMatch:
                        {
                            MatchAlgorithm matchAlgo = inspAlgo as MatchAlgorithm;
                            inspResult.ResultValue = $"{matchAlgo.OutScore}";
                            break;
                        }
                    case InspectType.InspBinary:
                        {
                            BlobAlgorithm blobAlgo = (BlobAlgorithm)inspAlgo;
                            int min = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].min;
                            int max = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].max;
                            inspResult.ResultValue = $"{blobAlgo.OutBlobCount}/{min}~{max}";
                            break;
                        }
                }

                inspWindow.AddInspResult(inspResult);
            }

            if (totalArea.Count > 0)
            {
                //찾은 위치를 이미지상에서 표시
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null)
                {
                    cameraForm.AddRect(totalArea);
                }
            }

            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            if (resultForm != null)
            {
                resultForm.AddWindowResult(inspWindow);
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

                //속성창을 현재 선택된 ROI에 대한 것으로 변경
                propForm.ShowProperty(inspWindow);
            }

            UpdateProperty(inspWindow);

            Global.Inst.InspStage.PreView.SetInspWindow(inspWindow);
        }

        //ImageViwer에서 ROI를 추가하여, InspWindow생성하는 함수
        public void AddInspWindow(InspWindowType windowType, Rect rect)
        {
            InspWindow inspWindow = _model.AddInspWindow(windowType);
            if (inspWindow is null)
                return;

            inspWindow.WindowArea = rect;
            inspWindow.IsTeach = false;

            //#11_MATCHING#7 새로운 ROI가 추가되면, 티칭 이미지 추가
            SetTeachingImage(inspWindow);
            UpdateProperty(inspWindow);
            UpdateDiagramEntity();

            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SelectDiagramEntity(inspWindow);
                SelectInspWindow(inspWindow);
            }
        }

        public bool AddInspWindow(InspWindow sourceWindow, OpenCvSharp.Point offset)
        {
            InspWindow cloneWindow = sourceWindow.Clone(offset);
            if (cloneWindow is null)
                return false;

            if (!_model.AddInspWindow(cloneWindow))
                return false;

            UpdateProperty(cloneWindow);
            UpdateDiagramEntity();

            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.SelectDiagramEntity(cloneWindow);
                SelectInspWindow(cloneWindow);
            }

            return true;
        }


        //입력된 윈도우 이동
        public void MoveInspWindow(InspWindow inspWindow, OpenCvSharp.Point offset)
        {
            if (inspWindow == null)
                return;

            inspWindow.OffsetMove(offset);
            UpdateProperty(inspWindow);
        }

        //#MODEL#10 기존 ROI 수정되었을때, 그 정보를 InspWindow에 반영
        public void ModifyInspWindow(InspWindow inspWindow, Rect rect)
        {
            if (inspWindow == null)
                return;

            inspWindow.WindowArea = rect;
            inspWindow.IsTeach = false;

            UpdateProperty(inspWindow);
        }

        //#MODEL#11 InspWindow 삭제하기
        public void DelInspWindow(InspWindow inspWindow)
        {
            _model.DelInspWindow(inspWindow);
            UpdateDiagramEntity();
        }


        public void DelInspWindow(List<InspWindow> inspWindowList)
        {
            _model.DelInspWindowList(inspWindowList);
            UpdateDiagramEntity();
        }

        public void Grab(int bufferIndex)
        {
            if (_grabManager == null) return;

            _grabManager.Grab(bufferIndex, true);
        }

        //영상 취득 완료 이벤트 발생시 후처리
        private async void _multiGrab_TransferCompleted(object sender, object e)
        {
            int bufferIndex = (int)e;
            SLogger.Write($"TransferCompleted {bufferIndex}");

            _imageSpace.Split(bufferIndex); // Grab된 이미지 분할 처리
            DisplayGrabImage(bufferIndex);  // 화면 갱신

            if (_previewImage != null)
            {
                Bitmap bitmap = ImageSpace.GetBitmap(0);
                _previewImage.SetImage(BitmapConverter.ToMat(bitmap));
            }

            //#8_LIVE#2 LIVE 모드일때, Grab을 계속 실행하여, 반복되도록 구현
            //이 함수는 await를 사용하여 비동기적으로 실행되어, 함수를 async로 선언해야 합니다.
            if (LiveMode)
            {
                SLogger.Write("Grab");
                await Task.Delay(100);  // 비동기 대기
                _grabManager.Grab(bufferIndex, true);  // 다음 촬영 시작
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

        public Bitmap GetBitmap(int bufferIndex = -1)
        {
            // Global을 통해 다시 InspStage → ImageSpace 접근
            if (Global.Inst.InspStage.ImageSpace is null)
                return null;

            return Global.Inst.InspStage.ImageSpace.GetBitmap();
        }

        //#7_BINARY_PREVIEW#4 이진화 프리뷰를 위해, ImageSpace에서 이미지 가져오기
        public Mat GetMat(int bufferIndex = -1, eImageChannel imageChannel = eImageChannel.None)
        {
            if (bufferIndex >= 0)
                SelBufferIndex = bufferIndex;

            //#BINARY FILTER#14 채널 정보가 유지되도록, eImageChannel.None 타입을 추가
            if (imageChannel != eImageChannel.None)
                SelImageChannel = imageChannel;

            return Global.Inst.InspStage.ImageSpace.GetMat(SelBufferIndex, SelImageChannel);
        }

        public void UpdateDiagramEntity()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateDiagramEntity();
            }

            ModelTreeForm modelTreeForm = MainForm.GetDockForm<ModelTreeForm>();
            if (modelTreeForm != null)
            {
                modelTreeForm.UpdateDiagramEntity();
            }
        }

        public void RedrawMainView()
        {
            CameraForm cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
            {
                cameraForm.UpdateImageViewer();
            }
        }
        public bool LoadModel(string filePath)
        {
            SLogger.Write($"모델 로딩:{filePath}");
                
            _model = _model.Load(filePath);

            if (_model is null)
            {
                SLogger.Write($"모델 로딩 실패:{filePath}", SLogger.LogType.Error);
                return false;
            }

            string inspImagePath = _model.InspectImagePath;
            if (File.Exists(inspImagePath))
            {
                Global.Inst.InspStage.SetImageBuffer(inspImagePath);
            }

            UpdateDiagramEntity();

            return true;
        }

        public void SaveModel(string filePath)
        {
            SLogger.Write($"모델 저장:{filePath}");

            //입력 경로가 없으면 현재 모델 저장
            if (string.IsNullOrEmpty(filePath))
                Global.Inst.InspStage.CurModel.Save();
            else
                Global.Inst.InspStage.CurModel.SaveAs(filePath);
        }

        public void SetWorkingState(WorkingState workingState)
        {
            var cameraForm = MainForm.GetDockForm<CameraForm>();
            if (cameraForm != null)
                cameraForm.SetWorkingState(workingState);
        }

        #region Disposable
        private bool disposed = false; // Dispose 호출 여부

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_saigeAI != null)
                    {
                        _saigeAI.Dispose(); // AI 모듈 해제
                        _saigeAI = null;
                    }
                    if(_grabManager != null)
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

        #endregion //Disposable
    }
}
