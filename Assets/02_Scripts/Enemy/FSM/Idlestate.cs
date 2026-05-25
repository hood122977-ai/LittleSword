using Logger = LittleSword.Common.Logger;
using UnityEngine;

namespace LittleSword.Enemy.FSM
{
    // 적 대기 상태 표현 상태 표현
    // Enter: 상태 진입 시 초기화
    // Update: 대기중 반복 검사
    // Exit: 상태 종료시 필요한 정리 작업
    public class Idlestate : IState
    {
        private readonly float detecInterval; // 감지 검사 사이의 최소 간격
        private float lastDetectTime; // 마지막 감지 검사 수행 타임.타임 값

        public Idlestate(float detecInterval = 0.3f) // 생성자: 감지 간격 지정
        {
            this.detecInterval = detecInterval; // 전달된 감지 간격 내부필드에 저장
            lastDetectTime = Time.time - detecInterval; // 초기화 시점 감지 타이머 만료 상태로 시작
        }
        // 상태로 진입할 때 호출
        // 애니메이션 전환/대기 초기값 설정
        public void Enter(Enemy enemy)
        {
            // 개발용 진입 로직/ 실제로는 애니메이터 트리거나 목표 초기화 로직을 호출할 것
            Logger.Log("Idle 진입");
            enemy.animator.SetBool(Enemy.hashIsRun, false);
        }

        // 상태가 활성화된 동안 주기적 호출
        // 시야 검사, 플레이어 감지 등 경량 작업을 수행
        public void Update(Enemy enemy)
        {
            // 마지막 감지 시각 이후 detectInterval(초) 이상 경과했는지 확인 - 경과 시 감지 로직 실행
            if (Time.time - lastDetectTime >= detecInterval)
            {
                // 개발용 갱신 로직 / 실제로직(이동, 거리 체크, 상태 전환 등)을 구현 
                Logger.Log("Idle 갱신");

                lastDetectTime = Time.time; // 감지 검사 시각을 현재 시각으로 갱신

                // 플레이어가 주변에 있는지 검사 (감지되면 true)
                if (enemy.DetectPlayer())
                {
                    // 플레이어 감지되면 추적(Chase) 상태로 전환 요청
                    enemy.ChangeState<ChaseState>();
                }
            }
        }

        // 상태를 종료할 때 호출됨
        // 다음 상태 진입 전 필요한 정리 작업을 수행
        public void Exit(Enemy enemy)
        {
            // 개발용 종료 로그/  상태 전환 전 필요한 정리 적업을 추가할 것.
            Logger.Log("Idle 종료");

        }
    }
}
