using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using OpenCvSharp;

namespace GitDockPanelSuite.Teach
{
    public class InspWindow
    {
        public InspWindowType inspWindowType { get; set; }

        public string Name { get; set; }
        public string UID { get; set; }

        public Rect WindowArea { get; set; }
        public Rect InspArea { get; set; }
        public bool IsTeach { get; set; } = false;

        public List<InspAlgorithm> AlgorithmList { get; set; } = new List<InspAlgorithm>();

        public InspWindow()
        {
        }

        public InspWindow(InspWindowType windowType, string name)
        {
            inspWindowType = windowType;
            Name = name;
        }

        public InspWindow Clone(Point offset, bool includeChildren = true)
        {
            InspWindow cloneWindow = InspWindowFactory.Inst.Create(this.inspWindowType, false);
            cloneWindow.WindowArea = this.WindowArea + offset;
            cloneWindow.IsTeach = false;

            foreach (InspAlgorithm algo in AlgorithmList)
            {
                var cloneAlgo = algo.Clone();
                cloneWindow.AlgorithmList.Add(cloneAlgo);
            }

            return cloneWindow;
        }

        public bool AddInspAlgorithm(InspectType inspType)
        {
            InspAlgorithm inspAlgo = null;

            switch (inspType)
            {
                case InspectType.InspBinary:
                    inspAlgo = new BlobAlgorithm();
                    break;
            }

            if (inspAlgo is null) return false;

            AlgorithmList.Add(inspAlgo);

            return true;
        }

        public InspAlgorithm FindInspAlgorithm(InspectType inspType)
        {
            return AlgorithmList.Find(algo => algo.InspectType == inspType);
        }

        public virtual bool DoInspect(InspectType inspType)
        {
            foreach (var inspAlgo in AlgorithmList)
            {
                if (inspAlgo.InspectType == inspType || inspType == InspectType.InspNone)
                    inspAlgo.DoInspect();
            }

            return true;
        }

        public bool IsDefect()
        {
            foreach (InspAlgorithm algo in AlgorithmList)
            {
                if (!algo.IsInspected) continue;

                if (algo.IsDefect) return true;
            }
            
            return false;
        }

        public virtual bool OffsetMove(Point offset)
        {
            Rect windowRect = WindowArea;
            windowRect.X += offset.X;
            windowRect.Y += offset.Y;
            WindowArea = windowRect;
            return true;
        }

        public bool SetInspOffset(Point offset)
        {
            InspArea = WindowArea + offset;
            AlgorithmList.ForEach(algo => algo.InspRect = algo.TeachRect + offset);
            return true;
        }
    }
}
