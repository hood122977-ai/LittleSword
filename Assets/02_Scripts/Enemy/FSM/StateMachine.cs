
namespace LittleSword.Enemy.FSM
{
    // Enemy용 간단한 상태 머신 구현체 
    // 하나의 현재 상태를 보유 상태 전환시 이전 상태를 호출 신규 상태 호출
    // 매 프레임 호출되는 Updat는 현재 상태의 Upate를 위임함
    public class StateMachine
    {
        // 상태 머신이 동작할 컨텍스트
        private Enemy enemy;

        // 생성자: 상태 머신에 동작할 에너미 컨텍스트를 주입함
        public StateMachine(Enemy enemy)
        {
            this.enemy = enemy;
        }

        // 현재 활성 상태를 나타내는 프로퍼티(외부에서 읽기 가능, 내부에서만 변경 가능)
        // null일 수 있으며 상태 전환 시 ChangeState에서 갱신되고 Update에서 사용됨

        public IState currentState { get; private set; }

        // 상태 전환을 수행/ 현재 사태있으면 Exit 호출 정리
        // currentState를 newState로 교체하고 Enter(enemy)를 호출하여 진입 동작을 수행함.
        public void ChangeState(IState newState)
        {
            currentState?.Exit(enemy); // 이전 상태 종료
            currentState = newState; // 상태 교체
            currentState.Enter(enemy); // 새 상태 진입
        }

        //매프레임 현재 상태의 업데이트를 호출함
        // currentState가 null이면 아무 동작도 안함

        public void Update()
        {
            currentState.Update(enemy); 
        }


    }
}
