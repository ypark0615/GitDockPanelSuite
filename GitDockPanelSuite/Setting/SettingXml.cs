using System;
using System.IO;
using Common.Util.Helpers;
using GitDockPanelSuite.Grab;

namespace GitDockPanelSuite.Setting
{
    public class SettingXml
    {
        private const string SETTING_DIR = "Setup";
        private const string SETTING_FILE_NAME = @"Setup\Setting.xml";

        private static SettingXml _setting;

        public static SettingXml Inst
        {
            get
            {
                if (_setting is null) Load();

                return _setting;
            }
        }

        public static void Load()
        {
            if (_setting != null) return;

            string settingFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, SETTING_FILE_NAME);
            if (File.Exists(settingFilePath))
            {
                _setting = XmlHelper.LoadXml<SettingXml>(settingFilePath);
            }

            if (_setting is null)
            {
                _setting = CreateDefaultInstance();
            }
        }

        public static void Save()
        {
            string settingFilePath = Path.Combine(Environment.CurrentDirectory, SETTING_FILE_NAME);
            if (!File.Exists(settingFilePath))
            {
                string settingDirPath = Path.Combine(Environment.CurrentDirectory, SETTING_DIR);

                if (!Directory.Exists(settingDirPath))
                    Directory.CreateDirectory(settingDirPath);

                FileStream fs = File.Create(settingFilePath);
                fs.Close();
            }

            XmlHelper.SaveXml<SettingXml>(settingFilePath, Inst);
        }

        private static SettingXml CreateDefaultInstance()
        {
            SettingXml setting = new SettingXml();
            setting.ModelDir = @"d:\model";
            return setting;
        }

        public SettingXml() { }

        public string MachineName { get; set; } = "Jidam";

        public string ModelDir { get; set; } = "";
        public string ImageDir { get; set; } = "";

        public CameraType CamType { get; set; } = CameraType.WebCam;
    }
}
