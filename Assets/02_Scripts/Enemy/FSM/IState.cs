namespace LittleSword.Enemy.FSM
{
    public interface IState
    {
        /// <summary>
        /// 상태로 진입할 때 한 번 호출됩니다.
        /// - 상태 전환 시 초기화 작업(애니메이션 재생, 변수 초기화, 타이머 설정 등)을 수행합니다.
        /// - 짧고 경량 작업만 수행하도록 하고, 장기 작업은 코루틴이나 타이머로 위임하세요.
        /// </summary>
        /// <param name="enemy">상태를 소유한 Enemy 인스턴스(컨텍스트)</param>
        void Enter (Enemy enemy );

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임(또는 FSM이 호출하는 간격)마다 호출됩니다.
        /// - 주로 AI 판단, 이동 처리, 타이머 체크 등 반복 실행 로직을 넣습니다.
        /// - 가능한 한 빠르게 실행되도록 하고 블로킹 작업을 피하세요.
        /// </summary>
        /// <param name="enemy">상태를 소유한 Enemy 인스턴스(컨텍스트)</param>
        void Update(Enemy enemy ); // Execute

        /// <summary>
        /// 상태가 활성화되어 있는 동안 매 프레임(또는 FSM이 호출하는 간격)마다 호출됩니다.
        /// - 주로 AI 판단, 이동 처리, 타이머 체크 등 반복 실행 로직을 넣습니다.
        /// - 가능한 한 빠르게 실행되도록 하고 블로킹 작업을 피하세요.
        /// </summary>
        /// <param name="enemy">상태를 소유한 Enemy 인스턴스(컨텍스트)</param>
        void Exit (Enemy enemy );
    }
}
