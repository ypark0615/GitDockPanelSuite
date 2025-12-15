using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDockPanelSuite.Core
{
    public class Global : IDisposable
    {
        
        #region Singleton Instance
        private static readonly Lazy<Global> _instance = new Lazy<Global>(() => new Global());
        /* 지연 초기화 객체(Lazy):
        * 후에 다른 파일에서 인스턴스.value를 호출하면 인스턴스 생성 실행.
        *   Ex) Global obj = _instance.Value; // 이때 람다식에 맞춰 인스턴스 생성 실행
        *   Lazy<T>는 .Value를 처음 읽는 순간에만 팩토리(람다)가 실행됨.
        * 불필요한 리소스 낭비를 줄임.
        * 프로그램 시작 시 로딩을 줄여 퍼포먼스 향상.
        * !! Lazy는 반드시 "어떻게 생성할지"를 알아야 함.
        *   new Lazy뒤의 괄호 안 람다식이 생성 방법을 정의해놓은 식.
        */

        /* readonly:
        * 변수에 새로운 값을 대입하는 것은 불가능하지만,
        * 변수에 들어있는 객체의 세부 데이터를 변경하는 것은 가능.
        * 즉, 이 변수명에 배정해준 참조된 값은 변경할 수 없으나,
        * 배정된 값은 참조 형식이기 때문에 값 내부의 데이터를 변경하는 것은 가능.
        *   1) _instance = new Lazy<Global>(...);  // ❌ 불가능 (필드 재대입 금지)
        *   2) _instance.Value.xxx = ...;          // ⭕ 가능 여부는 Value가 가리키는 객체의 설계에 따라 달라짐
        */

        public static Global Inst
        {
            get
            {
                return _instance.Value;
            }
        }
        #endregion


        private InspStage _stage = new InspStage();

        public InspStage InspStage
        {
            get { return _stage; }
        }


        public Global() // 생성자 호출 함수
        {
            //이러면 외부에서 new Global()을 마음대로 할 수 있어서 싱글톤 의미가 없어집니다.
            //➡️ 반드시 private Global() 이어야 합니다.
        }

        public void Initialize() // 초기화 함수
        {
            _stage.Initialize(); // InspStage 초기화
        }

        public void Dispose() // 소멸자 호출 함수
        {
            _stage.Dispose(); // InspStage 소멸자 호출
        }
    }
}
