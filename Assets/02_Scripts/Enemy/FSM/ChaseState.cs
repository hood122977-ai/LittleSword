using UnityEngine;
using Logger = LittleSword.Common.Logger;

namespace LittleSword.Enemy.FSM
{
    // 적 추적 상태 표현 상태 표현
    // Enter: 추적 시작 시 초기화
    // Update: 추적 중 매 프레임 실행되는 로직
    // Exit: 추적 종료시 정리
    public class ChaseState:IState
    {
        private readonly float detecInterval; // 감지 검사 사이의 최소 간격
        private float lastDetectTime; // 마지막 감지 검사 수행 타임.타임 값

        public ChaseState(float detecInterval = 0.3f) // 생성자: 감지 간격 지정
        {
            this.detecInterval = detecInterval; // 전달된 감지 간격 내부필드에 저장
            lastDetectTime = Time.time - detecInterval; // 초기화 시점 감지 타이머 만료 상태로 시작
        }

        // 상태로 진입할 때 호출
        // 타겟 추적 준비 작업
        public void Enter(Enemy enemy)
        {
            // 진입 로그 . 실제로는 애니메이터 트리거나 목표 초기화 로직을 호출할 것
            Logger.Log("Chase 진입");

            // 이동 애니메이션 플래그(IsRun)를 true로 설정하여 애니메이터에 '추적(달리는) 상태'임을 알림
            enemy.animator.SetBool(Enemy.hashIsRun, true);
        }

        // 상태가 활성화된 동안 주기적 호출
        // 경로 추적, 목표 이동등 반복 로직을 이것에서 구현
        // 가능한 빠르게 실행되도록/ 블로킹 작업 피할 것
        public void Update(Enemy enemy)
        {
            //// 마지막 감지 시각 이후 detectInterval(초) 이상 경과했는지 확인 - 경과 시 감지 로직 실행 
            //if (Time.time - lastDetectTime <= lastDetectTime) return;

            //lastDetectTime = Time.time;

            //  갱신 로직, 실제로직(이동, 거리 체크, 상태 전환 등)을 구현 
            Logger.Log("Chase 갱신");

            // 플레이어가 주변에 있는지 검사 (감지되면 true)
            if (enemy.DetectPlayer())
            {
                // 플레이어 쪽으로 이동
                enemy.MoveToPlayer();

                // 공격 사정거리 내에 들어온 경우
                if (enemy.IsInAttackRange())
                {
                    // 이동을 멈추고 공격 상태로 전환
                    enemy.StopMoving();
                    enemy.ChangeState<AttackState>();

                }
            }
            else
            {
                // 플레이어 감지되면 추적(Idle) 상태로 전환 요청
                enemy.StopMoving();
                enemy.ChangeState<Idlestate>();
            }

        }

        // 상태를 종료할 때 호출됨
        // 추적 관련 리소스 정리 및 다음 상태 진입 전 필요한 복구 작업 수행
        public void Exit(Enemy enemy)
        {
            //  종료 로그, 상태 전환 전 필요한 정리 적업을 추가할 것.
            Logger.Log("Chase 종료");

        }
    }

}
