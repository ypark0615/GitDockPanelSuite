using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitDockPanelSuite.Core;

namespace GitDockPanelSuite.Property
{
    public partial class AIModuleProp : UserControl
    {
        SaigeAI _saigeAI;
        string _modelPath = string.Empty;
        AIEngineType _engineType;
        /* _engineType을 선언한 이유는 다른 곳에서 경로를 입력한 이후에 모드를 변경할 수도 있기 때문 */

        public AIModuleProp()
        {
            InitializeComponent();

            // 콤보박스 초기값
            ComboBox_AIMode.DataSource = Enum.GetValues(typeof(AIEngineType)).Cast<AIEngineType>().ToList();
            ComboBox_AIMode.SelectedIndex = 0;
        }

        private void Btn_ModelInit_Click(object sender, EventArgs e) // 
        {
            if(_saigeAI != null)
                _saigeAI.Dispose();

            if (_modelPath != string.Empty)
            {
                TextBox_ModelPath.Text = null;
                _modelPath = string.Empty;
            }


            MessageBox.Show("초기화가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        private void Btn_ModelApply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_modelPath))
            {
                MessageBox.Show("모델 파일을 선택해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(_saigeAI == null)
            { // ai 모듈이 선언이 안 되어있으면 초기화 선언.
                _saigeAI = Global.Inst.InspStage.AIModule;
            }

            _saigeAI.LoadEngine(_modelPath, _engineType); // 엔진 로딩
            MessageBox.Show("모델이 성공적으로 로드되었습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_InspAI_Click(object sender, EventArgs e)
        {
            if (_saigeAI == null) // AI 모듈이 없으면 검사 불가
            {
                MessageBox.Show("AI 모듈이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = Global.Inst.InspStage.GetCurrentImage(); // 현재 이미지 가져오기
            if(bitmap == null)
            {
                MessageBox.Show("현재 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saigeAI.InspAIModule(bitmap); // 검사 모듈에 이미지 입력

            Bitmap resultImage = _saigeAI.GetResultImage(); // 결과 이미지 호출

            Global.Inst.InspStage.UpdateDisplay(resultImage); // 결과 이미지 표출
        }

        private void Btn_ModelFileLoad_Click(object sender, EventArgs e) // 모델 파일 선택
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Saige 모델 선택";
                openFileDialog.Filter = "Saige AI Files|*.saigeiad; *.saigeseg; *.saigedet; *.saigecls; *.saigeocr;"; // 일단은 모든 AI 파일들을 선택할 수 있도록 설정
                openFileDialog.Multiselect = false; // 모델 1개만 선택하도록 제한
                openFileDialog.InitialDirectory = @"C:\Saige\SaigeVision\engine\Examples\data\sfaw2023\models";

                switch (_engineType) // 현재 선택된 타입
                {
                    case AIEngineType.AnomalyDetection:
                        openFileDialog.Filter = "Anomaly Detection Files|*.saigeiad;";
                        break;
                    case AIEngineType.Segmentation:
                        openFileDialog.Filter = "Segmentation Files|*.saigeseg;";
                        break;
                    case AIEngineType.Detection:
                        openFileDialog.Filter = "Detection Files|*.saigedet;";
                        break;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _modelPath = openFileDialog.FileName;
                    TextBox_ModelPath.Text = _modelPath;
                }
            }
        }

        private void ComboBox_AIMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            AIEngineType engineType = (AIEngineType) ComboBox_AIMode.SelectedIndex;

            if (engineType != _engineType) // 선택된 엔진 타입과 현재 설정되어있는 엔진 타입이 다름
            {
                if (_saigeAI != null) // 그런데 AI 모듈이 null이 아니라면 다른 타입의 AI 모듈이 선언되어있는 것이므로
                    _saigeAI.Dispose(); // AI 소멸해서 비워줌
            }

            _engineType = engineType;
        }
    }
}
