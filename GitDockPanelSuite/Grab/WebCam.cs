using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GitDockPanelSuite.Grab
{
    class WebCam : GrabModel
    {
        private VideoCapture _capture = null;
        private Mat _frame = null;

        #region Private Field
        private bool _disposed = false;
        #endregion

        #region Method

        internal override bool Create(string strIpAddr = null)
        {
            _capture = new VideoCapture(0); // 0번 카메라 (기본 웹캠)
            if (_capture == null)
                return false;

            return true;
        }

        internal override bool Grab(int bufferIndex, bool waitDone)
        {
            if (_frame is null)
                _frame = new Mat();

            _capture.Read(_frame);
            if (!_frame.Empty())
            {
                OnGrabCompleted(bufferIndex); // 그랩 완료

                //int bufSize = _frame.Width * _frame.Height * (pixelBpp / 8);
                //int bufSize = _frame.Width * _frame.Height * _frame.ElemSize(); // ElemSize() = pixelBpp * 8
                int bufSize = (int)(_frame.Total() * _frame.ElemSize());

                if (_userImageBuffer != null && _userImageBuffer.Length > BufferIndex)
                { // _userImageBuffer가 초기화되어 있고, BufferIndex(기본 0)가 접근 가능한 범위일 때
                    if (_userImageBuffer[BufferIndex].ImageBuffer.Length >= bufSize)
                    { // 할당된 실제 메모리 크기(Byte)가 이번 프레임이 필요로 하는 메모리 크기(Byte)보다 큰가?
                        Marshal.Copy(_frame.Data, _userImageBuffer[BufferIndex].ImageBuffer, 0, bufSize);
                        // Mat의 데이터를 byte 배열로 복사
                    }
                    //else
                    //{
                    //    SLogger.Write("Error: Buffer size is too small.", SLogger.LogType.Error);
                    //}
                }

                OnTransferCompleted(BufferIndex); // 전송 완료

                if (IncreaseBufferIndex) // 버퍼 인덱스를 이번 프레임에서 증가시킬지 여부(기본 false)
                {
                    BufferIndex++; // 다음 버퍼 슬롯으로 이동
                    if (BufferIndex >= _userImageBuffer.Count()) // 배열(또는 리스트) 끝에 도달하면
                        BufferIndex = 0; // 처음으로 복귀
                }
            }
            return true;
        }

        internal override bool Close()
        {
            if (_capture != null)
                _capture.Release();

            return true;
        }

        internal override bool Open()
        {
            if (_capture == null)
                return false;

            // BGR 포맷을 강제 설정 (산업용 카메라나 특정 드라이버에 따라 가능)

            // BGR24 포맷 (컬러)
            int fourccBGR3 = VideoWriter.FourCC('B', 'G', 'R', '3');
            _capture.Set(VideoCaptureProperties.CodecPixelFormat, fourccBGR3);

            return true;
        }

        internal override bool Reconnect()
        {
            Close();
            return Open();
        }

        internal override bool GetPixelBpp(out int pixelBpp)
        {
            pixelBpp = 8;

            if (_capture == null)
                return false;

            if (_frame is null)
            {
                _frame = new Mat();
                _capture.Read(_frame); // 프레임 캡처
            }

            pixelBpp = _frame.ElemSize() * 8; // 픽셀당 비트수 계산
            // ElemSize(): 픽셀 하나가 갖는 메모리 수 = 채널 수 × 채널당 바이트 수 (Byte / pixel)
            // x 8 : Bpp (Bit / pixel)
            // 즉, ElemSize()는 "픽셀 하나가 갖는 바이트 수"이기 때문에 8을 곱해서 비트 수로 변경함!

            return true;
        }
        #endregion

        #region Parameter Setting
        internal override bool SetExposureTime(long exposure)
        {
            if (_capture == null)
                return false;

            _capture.Set(VideoCaptureProperties.Exposure, exposure);
            return true;
        }

        internal override bool GetExposureTime(out long exposure)
        {
            exposure = 0;

            if (_capture == null)
                return false;

            exposure = (long)_capture.Get(VideoCaptureProperties.Exposure);
            return true;
        }

        internal override bool SetGain(float gain)
        {
            if (_capture == null)
                return false;

            _capture.Set(VideoCaptureProperties.Gain, gain);
            return true;
        }

        internal override bool GetGain(out float gain)
        {
            gain = 0;
            if (_capture == null)
                return false;

            gain = (long)_capture.Get(VideoCaptureProperties.Gain);
            return true;
        }

        internal override bool GetResolution(out int width, out int height, out int stride)
        {
            width = 0;
            height = 0;
            stride = 0;

            if (_capture == null)
                return false;

            width = (int)_capture.Get(VideoCaptureProperties.FrameWidth);
            height = (int)_capture.Get(VideoCaptureProperties.FrameHeight);

            int bpp = 8;
            GetPixelBpp(out bpp);

            if (bpp == 8)
                stride = width * 1; // 1 채널
            else
                stride = width * 3; // 3 채널
            
            return true;
        }

        internal override bool SetTriggerMode(bool hardwareTrigger)
        {
            if (_capture is null)
                return false;

            HardwareTrigger = hardwareTrigger;

            return true;
        }

        #endregion

        #region Dispose
        internal override void Dispose()
        {
            Dispose(disposing: true);
        }

        internal void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_capture != null)
                    _capture.Release();
            }
            _disposed = true;
        }
        #endregion
    }
}
