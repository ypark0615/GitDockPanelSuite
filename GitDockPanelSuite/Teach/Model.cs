using System;
using System.Collections.Generic;
using System.IO;
using GitDockPanelSuite.Core;
using System.Xml.Serialization;
using Common.Util.Helpers;

namespace GitDockPanelSuite.Teach
{
    public class Model
    {
        public string ModelName { get; set; } = "";
        public string ModelInfo { get; set; } = "";
        public string ModelPath { get; set; } = "";

        public string InspectImagePath { get; set; } = "";

        public List<InspWindow> InspWindowList { get; set; }

        public Model()
        {
            InspWindowList = new List<InspWindow>();
        }

        public InspWindow AddInspWindow(InspWindowType windowType)
        {
            InspWindow inspWindow = InspWindowFactory.Inst.Create(windowType);
            InspWindowList.Add(inspWindow);

            return inspWindow;
        }

        public bool AddInspWindow(InspWindow inspWindow)
        {
            if(inspWindow is null) return false;

            InspWindowList.Add(inspWindow);

            return true;
        }

        public bool DelInspWindow(InspWindow inspWindow)
        {
            if (InspWindowList.Contains(inspWindow))
            {
                InspWindowList.Remove(inspWindow);
                return true;
            }
            return false;
        }

        public bool DelInspWindowList(List<InspWindow> inspWindowList)
        {
            int before = InspWindowList.Count;
            InspWindowList.RemoveAll(w => inspWindowList.Contains(w));
            return InspWindowList.Count < before;
        }

        public void CreateModel(string path, string modelName, string modelInfo)
        {
            ModelPath = path;
            ModelName = modelName;
            ModelInfo = modelInfo;
        }

        public Model Load(string path)
        {
            Model model = XmlHelper.LoadXml<Model>(path);
            if(model == null) return null;

            foreach(var window in model.InspWindowList)
            {
                window.LoadInspWindow(model);
            }

            return model;
        }

        public void Save()
        {
            if(ModelPath == "") return;

            XmlHelper.SaveXml(ModelPath, this);

            foreach(var window in InspWindowList)
            {
                window.SaveInspWindow(this);
            }
        }

        public void SaveAs(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if(Directory.Exists(filePath))
            {
                ModelPath = Path.Combine(filePath, fileName + ".xml");
                ModelName = fileName;
                Save();
            }
        }
    }
}
