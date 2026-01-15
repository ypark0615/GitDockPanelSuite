using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using GitDockPanelSuite.Util;

namespace GitDockPanelSuite.Core
{
    public enum eImageChannel : int // 컬러 이미지 채널 구분용 enum
    {
    	None = -1,
        Color,          // 원본 컬러
        Gray,           // 그레이(변환)
        Red,            // R 채널
        Green,          // G 채널
        Blue,           // B 채널
        ChannelCount = 5,
    }

    //검사와 관련된 이미지 버퍼를 관리하는 클래스
    public class ImageSpace : IDisposable
    {
        private class ImageInfo : IDisposable // 버퍼 1개(원본 or 채널)의 이미지 메타/데이터/포인터를 관리
        {
            private Bitmap _bitmap;      // 최초 1회 Bitmap 포맷/메타 정보 설정용
            private Bitmap _tempBitmap;  // 현재 Buffer 포인터로 Bitmap을 생성해서 반환(래핑)

            private PixelFormat Format { get; set; } // Bitmap PixelFormat 저장

            public byte[] ImageData { get; set; } // 실제 픽셀 데이터(byte[])

            public int PixelBpp { get; set; }     // 8(Gray) 또는 24(Color)

            public int Width { get; set; }        // 이미지 가로

            public int Height { get; set; }       // 이미지 세로

            public SizeF PixelResolution { get; set; } // 픽셀 해상도(단위는 프로젝트 기준)

            public IntPtr Buffer { get; set; }          // ImageData를 Pinned한 포인터

            public GCHandle Handle { get; set; }        // Pinned 핸들(카메라 버퍼 연결용)

            public int Stride { get; set; }             // 한 줄 바이트 수

            public Bitmap ToBitmap() // ImageData 버퍼를 Bitmap으로 래핑해서 반환
            {
                if (_bitmap == null) // 최초 1회 Bitmap 포맷 세팅 및 Pinned 포인터 확보
                {
                    _bitmap = new Bitmap(Width, Height,
                        (PixelBpp == 8 ?
                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed :
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb));

                    Format = _bitmap.PixelFormat;
                    Width = _bitmap.Width;
                    Height = _bitmap.Height;

                    Handle = GCHandle.Alloc(ImageData, GCHandleType.Pinned); // 버퍼 고정
                    IntPtr pointer = Handle.AddrOfPinnedObject();            // 고정된 버퍼 포인터

                    var bufferAndStride = _bitmap.ToBufferAndStride(); // stride 정보 얻기
                    Buffer = pointer;                                   // Bitmap에 연결할 포인터 저장
                    Stride = bufferAndStride.Item2;
                }

                if (_tempBitmap != null) // 기존 뷰 제거 후 새로 래핑
                    _tempBitmap = null;

                _tempBitmap = new Bitmap(Width, Height, Stride, Format, Buffer); // 포인터 기반 Bitmap 생성

                if (_tempBitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed) // 8bpp 팔레트 세팅
                {
                    ColorPalette pal = _tempBitmap.Palette;
                    // Generate grayscale colours:
                    for (Int32 i = 0; i < 256; i++)
                        pal.Entries[i] = Color.FromArgb(i, i, i);
                    // Assign the edited palette to the bitmap.
                    _tempBitmap.Palette = pal;
                }
                return _tempBitmap;
            }

            public Mat ToMat() // ImageData를 OpenCV Mat으로 변환(래핑)
            {
                MatType matType = PixelBpp == 8 ? MatType.CV_8UC1 : MatType.CV_8UC3;
                Mat mat = Mat.FromPixelData(Height, Width, matType, ImageData);

                return mat;
            }


            #region Disposable

            private bool disposed = false; // to detect redundant calls

            protected virtual void Dispose(bool disposing)// ImageInfo 리소스 해제
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        if (ImageData != null)
                            ImageData = null; // 버퍼 참조 해제(관리 메모리)
                    }

                    disposed = true;
                }
            }

            public void Dispose() // 외부 Dispose 호출
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            #endregion //Disposable
        }

        public class ImagePtr // 포인터 기반 이미지 래퍼(Mat 변환용)
        {
            public IntPtr Ptr { get; set; } // 이미지 포인터
            public long Length { get; set; } // 버퍼 길이
            public int Width { get; set; } // 가로
            public int Height { get; set; } // 세로
            public int Step { get; set; } // stride 개념
            public int Bpp { get; set; } // 채널 수(1=Gray, 3=Color)

            public ImagePtr(IntPtr ptr, long length, int width, int height, int step, int bpp) // 포인터 정보 저장
            {
                Ptr = ptr;
                Length = length;
                Width = width;
                Height = height;
                Step = step;
                Bpp = bpp;
            }

            public Mat ToMat() // 포인터를 OpenCV Mat으로 래핑
            {
                var type = Bpp == 1 ? MatType.CV_8UC1 : MatType.CV_8UC3;
                //return new Mat(Height, Width, type, Ptr);
                return Mat.FromPixelData(Height, Width, type, Ptr);
            }

            private static readonly ImagePtr _zero = new ImagePtr(IntPtr.Zero, 0, 0, 0, 0, 0); // 빈 포인터 객체
            public static ImagePtr Zero => _zero; // Zero 반환
        }

        private ImageInfo _inspectionImage = new ImageInfo(); // 검사 이미지의 기준 정보(해상도/픽셀뎁스 등)
        private Dictionary<int, ImageInfo> _imageInfo = new Dictionary<int, ImageInfo>(); // 버퍼 인덱스별 원본 이미지 버퍼
        private Dictionary<int, Dictionary<eImageChannel, ImageInfo>> _imageByChannel = new Dictionary<int, Dictionary<eImageChannel, ImageInfo>>(); // 버퍼 인덱스별 채널 이미지 버퍼(24bpp용)

        protected byte[] _ImageData; // 외부에서 사용할 수 있는 공용 데이터(현재는 직접 사용 빈도 낮아 보임)

        public virtual byte[] ImageData // 외부 접근용 데이터 프로퍼티
        {
            get { return _ImageData; }
            set { _ImageData = value; }
        }

        public bool UseImageSplit { get; set; } = true; // 컬러 이미지 채널 분리 사용 여부
        public int BufferCount { get; set; } = 0;       // 현재 구성된 버퍼 수

        public ImageSpace() { } // 생성자

        public void SetImageInfo(int inspectionPixelBpp, int inspectionWidth, int inspectionHeight, int inspectionStride) // 이미지 기준 정보 설정
        {
            _inspectionImage.PixelBpp = inspectionPixelBpp;
            _inspectionImage.Width = inspectionWidth;
            _inspectionImage.Height = inspectionHeight;
            _inspectionImage.Stride = inspectionStride;
        }

        public void InitImageSpace(int bufferCount) // bufferCount 개수만큼 이미지 버퍼 생성 및 초기화
        {
            if (_inspectionImage.Width == 0 || _inspectionImage.Height == 0) // 이미지 정보 없으면 초기화 불가
                return;

            Dispose(); // 기존 버퍼/리소스 정리 후 재생성

            _imageByChannel.Clear();
            _imageInfo.Clear();

            Func<int, ImageInfo> newImageInfo = (x) => // ImageInfo 생성 함수(버퍼 생성 + Pinned)
            {
                var imageInfo = new ImageInfo();
                imageInfo.PixelBpp = x;
                imageInfo.Width = _inspectionImage.Width;
                imageInfo.Height = _inspectionImage.Height;

                int bpp = 1;
                if (imageInfo.PixelBpp == 24)
                    bpp = 3;

                imageInfo.Stride = imageInfo.Width * bpp;
                imageInfo.PixelResolution = _inspectionImage.PixelResolution;
                imageInfo.ImageData = new byte[imageInfo.Stride * imageInfo.Height]; // 버퍼 생성

                GCHandle Handle = GCHandle.Alloc(imageInfo.ImageData, GCHandleType.Pinned); // 버퍼 고정
                imageInfo.Buffer = Handle.AddrOfPinnedObject();                             // 포인터 확보
                imageInfo.Handle = Handle;                                                  // 핸들 저장

                return imageInfo;
            };

            for (int i = 0; i < bufferCount; ++i) // 버퍼 인덱스별 버퍼 생성
            {
                #region Origin Image buffer Set

                if (_imageInfo.ContainsKey(i) == true) // 이미 존재하면 skip
                    continue;

                var imageInfo = newImageInfo(_inspectionImage.PixelBpp); // 원본 버퍼 생성
                _imageInfo.Add(i, imageInfo); // 원본 버퍼 등록

                if (_inspectionImage.PixelBpp == 24) // 컬러면 채널 버퍼도 추가 생성
                {
                    Dictionary<eImageChannel, ImageInfo> imageByChannel = new Dictionary<eImageChannel, ImageInfo>
                    {
                        { eImageChannel.Color, imageInfo },      // Color는 원본 버퍼 공유
                        { eImageChannel.Red, newImageInfo(8) },  // R 채널 버퍼
                        { eImageChannel.Green, newImageInfo(8) },// G 채널 버퍼
                        { eImageChannel.Blue, newImageInfo(8) }, // B 채널 버퍼
                        { eImageChannel.Gray, newImageInfo(8) }  // Gray 버퍼
                    };

                    _imageByChannel.Add(i, imageByChannel); // 채널 버퍼 등록
                }

                #endregion Origin Image Buffer Set
            }

            BufferCount = bufferCount; // 현재 버퍼 수 저장
        }

        #region Property
        public byte[] this[int i]// 인덱스별 원본 ImageData 접근자
        { 
            get { return _imageInfo[i].ImageData; }
            set { value.CopyTo(_imageInfo[i].ImageData, 0); } // 외부 배열 데이터를 내부 버퍼로 복사
        }


        public System.Drawing.Size ImageSize { get => new System.Drawing.Size(_inspectionImage.Width, _inspectionImage.Height); } // 이미지 크기

        public int PixelBpp { get => _inspectionImage.PixelBpp; } // 픽셀뎁스

        public SizeF PixelResolution { get => _inspectionImage.PixelResolution; } // 픽셀 해상도

        public int Width { get => _inspectionImage.Width; } // 가로

        public int Height { get => _inspectionImage.Height; } // 세로

        public int Stride { get => _inspectionImage.Stride; } // stride

        public long Length { get => _inspectionImage.Stride * _inspectionImage.Height; } // 전체 바이트 길이

        #endregion

        public Dictionary<int, Dictionary<eImageChannel, ImagePtr>> GetImageByChannelToClone() // 채널별 이미지 포인터 사전(복사형 IntPtr 생성)
        {
            if (_inspectionImage.PixelBpp == 8) // 그레이면 Gray만 제공
            {
                return _imageInfo.ToDictionary(kvp => kvp.Key,
                    kvp => new Dictionary<eImageChannel, ImagePtr>{{ eImageChannel.Gray,
                            new ImagePtr(
                                new IntPtr(kvp.Value.Buffer.ToInt64()),
                                kvp.Value.ImageData.Length,
                                kvp.Value.Width,
                                kvp.Value.Height,
                                kvp.Value.Stride,
                                (int)kvp.Value.PixelBpp) }
                    });
            }
            else // 컬러면 채널 전체 제공
            {
                return _imageByChannel.ToDictionary(x => x.Key, y => y.Value.ToDictionary(
                k => k.Key, z => new ImagePtr(new IntPtr(z.Value.Buffer.ToInt64()), z.Value.ImageData.Length, z.Value.Width, z.Value.Height, z.Value.Stride, (int)z.Value.PixelBpp)));
            }
        }

        public Dictionary<int, Dictionary<eImageChannel, ImagePtr>> GetImageByChannel() // 채널별 이미지 포인터 사전(원본 포인터 그대로 제공)
        {
            if (_inspectionImage.PixelBpp == 8) // 그레이면 Gray만 제공
            {
                return _imageInfo.ToDictionary(kvp => kvp.Key,
                    kvp => new Dictionary<eImageChannel, ImagePtr>{{ eImageChannel.Gray,
                            new ImagePtr(
                                kvp.Value.Buffer,
                                kvp.Value.ImageData.Length,
                                kvp.Value.Width,
                                kvp.Value.Height,
                                kvp.Value.Stride,
                                (int)kvp.Value.PixelBpp) }
                    });
            }
            else // 컬러면 채널 전체 제공
            {
                return _imageByChannel.ToDictionary(x => x.Key, y => y.Value.ToDictionary(
                k => k.Key, z => new ImagePtr(z.Value.Buffer, z.Value.ImageData.Length, z.Value.Width, z.Value.Height, z.Value.Stride, (int)z.Value.PixelBpp)));
            }
        }

        public byte[] GetInspectionBuffer(int index = 0) // 카메라에 연결할 원본 버퍼(byte[]) 반환
        {
            if (_imageInfo.Count <= index)
                return null;

            return _imageInfo[index].ImageData;
        }

        public IntPtr GetnspectionBufferPtr(int index = 0) // 카메라에 연결할 원본 버퍼 포인터(IntPtr) 반환
        {
            if (_imageInfo.Count <= index)
                return IntPtr.Zero;

            return _imageInfo[index].Buffer;
        }

        public GCHandle GetInspectionBufferHandle(int index = 0) // 카메라에 전달할 Pinned Handle 반환
        {
            if (_imageInfo.Count <= index)
                return new GCHandle();

            return _imageInfo[index].Handle;
        }

        public Bitmap GetBitmap(int index = 0, eImageChannel channel = eImageChannel.Color) // 버퍼의 Bitmap 반환(원본/채널 선택)
        {
            if (index < 0 || _imageInfo.Count <= index)
                return null;

            if (PixelBpp == 8 || channel == eImageChannel.Color) // 그레이 또는 Color면 원본 반환
            {
                return _imageInfo[index].ToBitmap();
            }
            else // 채널 Bitmap 반환
            {
                return _imageByChannel[index][channel].ToBitmap();
            }
        }

        public Mat GetMat(int index = 0, eImageChannel channel = eImageChannel.Gray) // 버퍼의 Mat 반환(원본/채널 선택)
        {
            if (_imageInfo.Count <= index)
                return null;

            if (channel == eImageChannel.Gray) // 기본은 Gray 반환(원본)
            {
                return _imageInfo[index].ToMat();
            }
            else // 채널 Mat 반환
            {
                if (_imageByChannel[index][channel] != null)
                    return _imageByChannel[index][channel].ToMat();
            }

            return null;
        }

        public Dictionary<int, Bitmap> GetBitmaps() // 모든 버퍼의 Bitmap을 Clone해서 반환(UI/외부 전달용)
        {
            Dictionary<int, Bitmap> listBitmap = new Dictionary<int, Bitmap>();
            if (PixelBpp == 8)
            {
                foreach (var light in _imageInfo)
                {
                    listBitmap.Add(light.Key,
                        (Bitmap)(light.Value.ToBitmap().Clone()));
                }
            }
            else
            {
                foreach (var light in _imageByChannel)
                {
                    listBitmap.Add(light.Key,
                        (Bitmap)(light.Value[eImageChannel.Color].ToBitmap().Clone()));
                }
            }

            return listBitmap;
        }

        public void Split(int index) // 컬러 이미지(24bpp)를 R/G/B/Gray 채널 버퍼로 분리 저장
        {
            if (!UseImageSplit) return;                 // 분리 기능 OFF면 return

            if (_imageInfo.Count <= index) return;      // 인덱스 범위 체크

            if (PixelBpp == 8) return;                  // 그레이면 분리 불필요

            var original = _imageInfo[index].ToBitmap(); // 원본 Bitmap 생성(버퍼 래핑)

            original.Split( // 원본 → 채널별 ImageData에 저장
                _imageByChannel[index][eImageChannel.Red].ImageData,
                _imageByChannel[index][eImageChannel.Green].ImageData,
                _imageByChannel[index][eImageChannel.Blue].ImageData,
                _imageByChannel[index][eImageChannel.Gray].ImageData);
        }

        #region Disposable


        private bool disposed = false; // to detect redundant calls

        protected virtual void Dispose(bool disposing) // ImageSpace 리소스 해제
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_inspectionImage != null) // 기준 정보 해제
                        _inspectionImage.Dispose();

                    if (_imageInfo != null) // 원본 버퍼들 해제
                    {
                        foreach (var image in _imageInfo)
                            image.Value.Dispose();
                    }

                    if (_imageByChannel != null) // 채널 버퍼들 해제
                    {
                        foreach (var image in _imageByChannel)
                        {
                            foreach (var innerImage in image.Value)
                                innerImage.Value.Dispose();
                        }
                    }
                }

                disposed = true;
            }
        }

        public void Dispose() // Dispose 호출
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion //Disposable
    }
}
