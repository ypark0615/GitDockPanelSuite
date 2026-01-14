using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Teach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace GitDockPanelSuite.Inspect
{
    public class InspectBoard
    {
        public InspectBoard()
        {
        }

        public bool Inspect(InspWindow window)
        {
            if (window is null) return false;

            if (!InspectWindow(window)) return false;

            return true;
        }

        private bool InspectWindow(InspWindow window)
        {
            window.ResetInspResult();
            foreach (InspAlgorithm algo in window.AlgorithmList)
            {
                if (!algo.IsUse) continue;

                if(!algo.DoInspect()) return false;

                string resultInfo = string.Join("\r\n", algo.ResultString);

                InspResult inspResult = new InspResult
                {
                    ObjectID = window.UID,
                    InspType = algo.InspectType,
                    IsDefect = algo.IsDefect,
                    ResultInfos = resultInfo
                };

                switch (algo.InspectType)
                {
                    case InspectType.InspMatch:
                        MatchAlgorithm matchAlgo = algo as MatchAlgorithm;
                        inspResult.ResultValue = $"{matchAlgo.OutScore}";
                        break;
                    case InspectType.InspBinary:
                        BlobAlgorithm blobAlgo = algo as BlobAlgorithm;
                        int min = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].min;
                        int max = blobAlgo.BlobFilters[blobAlgo.FILTER_COUNT].max;

                        inspResult.ResultValue = $"{blobAlgo.OutBlobCount}/{min}~{max}";
                        break;
                }

                List<DrawInspectInfo> resultArea = new List<DrawInspectInfo>(); //검사 결과 영역 정보 빈 리스트
                int resultCnt = algo.GetResultRect(out resultArea); //검사 결과 영역 정보 얻기(총 갯수, 영역 리스트)
                inspResult.ResultRectList = resultArea; //검사 결과 영역 정보 저장

                window.AddInspResult(inspResult);
            }
            return true;
        }

        public bool InspectWindowList(List<InspWindow> windowList)
        {
            if (windowList.Count <= 0) return false;

            Point alignOffset = new Point(0, 0);
            InspWindow idWindow = windowList.Find(w => w.InspWindowType == InspWindowType.ID);
            if(idWindow != null)
            {
                MatchAlgorithm matchAlgo = (MatchAlgorithm) idWindow.FindInspAlgorithm(InspectType.InspMatch) as MatchAlgorithm;
                if(matchAlgo != null && matchAlgo.IsUse)
                {
                    if (!InspectWindow(idWindow)) return false;

                    if (matchAlgo.IsInspected)
                    {
                        alignOffset = matchAlgo.GetOffset();
                        idWindow.InspArea = idWindow.WindowArea + alignOffset;
                    }
                }
            }

            foreach(InspWindow window in windowList)
            {
                window.SetInspOffset(alignOffset);

                if (!InspectWindow(window)) return false;
            }

            return true;
        }
    }
}
