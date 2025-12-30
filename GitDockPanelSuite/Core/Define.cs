using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDockPanelSuite.Core
{
    public enum DecisionType
    {
        None = 0,
        Good,
        Defect,
        Info,
        Error,
        Timeout
    }

    public static class Define
    {
        public static readonly string ROI_IMAGE_NAME = "RoiImage.png";
    }
}
