using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Teach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GitDockPanelSuite.Inspect
{
    public class InspResult
    {
        public InspWindow InspObject { get; set; } // 검사한 ROI 정보
        public string GroupID { get; set; } // ROI가 여러개 있을 때, 기준이 되는 ID
        public string ObjectID { get; set; } // 실제 검사한 ROI ID

        public InspWindowType ObjectType { get; set; } // 검사한 ROI 타입

        public InspectType InspType { get; set; } // 검사한 알고리즘 타입
        public int ErrorCode { get; set; } // 검사 결과 코드
        public bool IsDefect { get; set; } // 불량 여부
        public string ResultValue { get; set; } // 검사 결과 값
        public string ResultInfos { get; set; } // 세부적인 검사 결과 정보

        [XmlIgnore]
        public List<DrawInspectInfo> ResultRectList { get; set; } = null; //검사 결과로 찾은 불량 위치
        // 검사 결과는 휘발성이기 때문에 xml로 저장하지 않음

        public InspResult()
        {
            InspObject = new InspWindow();
            GroupID = string.Empty;
            ObjectID = string.Empty;
            ObjectType = InspWindowType.None;
            ErrorCode = 0;
            IsDefect = false;
            ResultValue = "";
            ResultInfos = string.Empty;
        }

        public InspResult(InspWindow window, string baseID, string objectID, InspWindowType objectType)
        {
            InspObject = window;
            GroupID = baseID;
            ObjectID = objectID;
            ObjectType = objectType;
            ErrorCode = 0;
            IsDefect = false;
            ResultValue = "";
            ResultInfos = string.Empty;
        }
    }
}
