using GitDockPanelSuite.Algorithm;
using GitDockPanelSuite.Core;
using GitDockPanelSuite.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using static GitDockPanelSuite.Property.BinaryProp;

namespace GitDockPanelSuite
{
    public enum PropertyType
    {
        Binary,
        Filter,
        AIModule
    }

    //public partial class PropertiesForm : Form
    public partial class PropertiesForm : DockContent
    {
        Dictionary<string, TabPage> _allTabs = new Dictionary<string, TabPage>();

        public PropertiesForm()
        {
            InitializeComponent();

            LoadOptionControl(PropertyType.Filter);
            LoadOptionControl(PropertyType.Binary);
            LoadOptionControl(PropertyType.AIModule);
        }

        private void LoadOptionControl(PropertyType propType)
        {
            string tabName = propType.ToString();

            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Text == tabName) return;
            }

            if (_allTabs.TryGetValue(tabName, out TabPage page))
            {
                tabPropControl.TabPages.Add(page);
                return;
            }

            UserControl _inspProp = CreateUserControl(propType);
            if (_inspProp == null) return;

            TabPage newTab = new TabPage(tabName)
            {
                Dock = DockStyle.Fill
            };

            _inspProp.Dock = DockStyle.Fill;
            newTab.Controls.Add(_inspProp);
            tabPropControl.TabPages.Add(newTab);
            tabPropControl.SelectedTab = newTab;

            _allTabs[tabName] = newTab;
        }

        private UserControl CreateUserControl(PropertyType propType)
        {
            UserControl curProp = null;
            switch (propType)
            {
                case PropertyType.Binary:
                    BinaryProp blobProp = new BinaryProp();
                    blobProp.RangeChanged += RangeSlider_RangeChanged;
                    blobProp.PropertyChanged += PropertyChanged;
                    curProp = blobProp;
                    break;
                case PropertyType.Filter:
                    ImageFilterProp filterProp = new ImageFilterProp();
                    curProp = filterProp;
                    break;
                case PropertyType.AIModule:
                    AIModuleProp AIModuleProp = new AIModuleProp();
                    curProp = AIModuleProp;
                    break;
                default:
                    MessageBox.Show("유효하지 않은 옵션입니다.");
                    return null;
            }
            return curProp;
        }

        public void UpdateProperty(BlobAlgorithm blobAlgorithm)
        {
            if (blobAlgorithm is null)
                return;

            foreach (TabPage tabPage in tabPropControl.TabPages)
            {
                if (tabPage.Controls.Count > 0)
                {
                    UserControl uc = tabPage.Controls[0] as UserControl;

                    if (uc is BinaryProp binaryProp)
                    {
                        binaryProp.SetAlgorithm(blobAlgorithm);
                    }
                }
            }
        }


        private void RangeSlider_RangeChanged(object sender, RangeChangedEventArgs e)
        {
            int lowerValue = e.LowerValue;
            int upperValue = e.UpperValue;
            bool invert = e.Invert;
            ShowBinaryMode showBinMode = e.ShowBinMode;

            Global.Inst.InspStage.PreView?.SetBinary(lowerValue, upperValue, invert, showBinMode);
        }

        private void PropertyChanged(object sender, EventArgs e)
        {
            Global.Inst.InspStage.RedrawMainView();
        }
    }
}
