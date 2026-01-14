using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Teach;
using GitDockPanelSuite.Util;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitDockPanelSuite.Inspect
{
    public class InspWorker
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private InspectBoard _inspectBoard = new InspectBoard();

        public bool IsRunning { get; set; } = false;

        public InspWorker() { }

        public void Stop()
        {
            _cts.Cancel();
        }

        public void StartCycleInspectImage()
        {
            _cts = new CancellationTokenSource();
            Task.Run(() => InspectionLoop(this, _cts.Token));
        }

        private void InspectionLoop(InspWorker inspWorker, CancellationToken token)
        {
            Global.Inst.InspStage.SetWorkingState(WorkingState.INSPECT);

            SLogger.Write($"검사 루프 시작");
        
            IsRunning = true;

            while (!token.IsCancellationRequested) // 토큰이 취소되지 않은 동안 반복
            {
                Global.Inst.InspStage.TryInspection();
                Thread.Sleep(100); //루프 딜레이
            }

            IsRunning = false;

            SLogger.Write($"검사 루프 종료");
        }


        public bool RunInspect(out bool isDefect)
        {
            isDefect = false;
            Model curModel = Global.Inst.InspStage.CurModel;
            List<InspWindow> inspWindowList = curModel.InspWindowList;
            foreach(var inspWindow in inspWindowList)
            {
                if(inspWindow is null) continue;
            
                UpdateInspData(inspWindow);
            }

            _inspectBoard.InspectWindowList(inspWindowList);

            int totalCnt = 0;
            int okCnt = 0;
            int ngCnt = 0;
            foreach(var inspWindow in inspWindowList)
            {
                totalCnt++;

                if (inspWindow.IsDefect())
                {
                    if(!isDefect) isDefect = true;

                    ngCnt++;
                }
                else
                    okCnt++;

                DisplayResult(inspWindow, InspectType.InspNone);
            }

            if (totalCnt > 0)
            {
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if (cameraForm != null)
                    cameraForm.SetInspResultCount(totalCnt, okCnt, ngCnt);
            }

            return true;
        }

        public bool TryInspect(InspWindow inspObj, InspectType inspType)
        {
            if (inspObj != null)
            {
                if(!UpdateInspData(inspObj)) return false;

                _inspectBoard.Inspect(inspObj);

                DisplayResult(inspObj, inspType);
            }
            else
            {
                bool isDefect = false;
                RunInspect(out isDefect);
            }

            ResultForm resultForm = MainForm.GetDockForm<ResultForm>();
            if(resultForm != null)
            {
                if(inspObj != null)
                    resultForm.AddWindowResult(inspObj);
                else
                {
                    Model curMode = Global.Inst.InspStage.CurModel;
                    resultForm.AddModelResult(curMode);
                }
            }

            return true;
        }

        private bool UpdateInspData(InspWindow inspWindow)
        {
            if (inspWindow is null) return false;
            
            Rect windowArea = inspWindow.WindowArea;

            inspWindow.PatternLearn();

            foreach (var inspAlgo in inspWindow.AlgorithmList)
            {
                inspAlgo.TeachRect = windowArea;
                inspAlgo.InspRect = windowArea;

                Mat srcImage = Global.Inst.InspStage.ImageSpace.GetMat(0, inspAlgo.ImageChannel);
                inspAlgo.SetInspData(srcImage);
            }

            return true;
        }

        private bool DisplayResult(InspWindow inspObj, InspectType inspType)
        {
            if(inspObj is null) return false;

            List<DrawInspectInfo> totalArea = new List<DrawInspectInfo>();

            List<InspAlgorithm> inspAlgorithmList = inspObj.AlgorithmList;
            foreach (var algorithm in inspAlgorithmList)
            {
                if (algorithm.InspectType != inspType && inspType != InspectType.InspNone) continue;

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>();
                int resultCnt = algorithm.GetResultRect(out resultArea);
                if (resultCnt > 0)
                    totalArea.AddRange(resultArea);
            }

            if (totalArea.Count > 0)
            { 
                var cameraForm = MainForm.GetDockForm<CameraForm>();
                if(cameraForm != null)
                    cameraForm.AddRect(totalArea);
            }

            return true;
        }

    }
}
