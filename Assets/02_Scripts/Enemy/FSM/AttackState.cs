using Logger = LittleSword.Common.Logger;
using System;
using UnityEngine;
using LittleSword.Player;

namespace LittleSword.Enemy.FSM
{
    // 적의 공격 상태를 나타내는 상태 구현체
    // enter: 상태 진입 시 초기화
    // Update: 상태 동안 매 프레임 실행될 로직
    // Exit: 산태 종료 시 정리
    public class AttackState : IState
    {
        private readonly float attackCooldown;
        private float lastAttackTime;

        // 생성자
        public AttackState(float attackCooldown = 1.0f)
        {
            this.attackCooldown = attackCooldown;
            lastAttackTime = Time.time - this.attackCooldown;
        }


        // 공격 상태로 진입할 때 한번 호출
        // 에니메이션 재생 또는 공격 준비 동작 추가

        public void Enter(Enemy enemy)
        {
            // 진입 로그. 실제 게임 로직은 애니메이터 트리거 설정 등을 대체 가능
            Logger.Log("Acttack 진입");
            enemy.StopMoving();
            // 공격 애니메이터 트리거를 설정하여 공격 애니메이션을 즉시 재생함
            //enemy.animator.SetTrigger(Enemy.hashAttack);
            enemy.animator.SetBool(Enemy.hashIsRun, false);
            lastAttackTime = Time.time - attackCooldown;
        }

        //상태가 활성화되어 있는 동안 주기적으로 호출됨.
        // 공격 판정, 타겟추격, 상태 전환 조건 검사 등을 수행할 것
        public void Update(Enemy enemy)
        {
            // 갱신 로그. 실제로직(공격, 재장전 타이머)을 여기에 구현할것
            Logger.Log("Acttack 갱신");

            enemy.DetectPlayer();

            // 공격 쿨타임이 지났는지 확인
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                // 마지막 공격 시간을 현재 시간으로 갱신
                lastAttackTime = Time.time;


                // 타겟이 없거나 사망했을 경우 idle 상태로 전환
                if (enemy.Target == null || enemy.Target.GetComponent<BasePlayer>()?.IsDead == true)
                {
                    enemy.ChangeState<Idlestate>();
                    return;
                }

                if (enemy.IsInAttackRange())
                {
                    // 달리기 애니메이션을 중지하고 공격 애니메이션 실행
                    enemy.animator.SetBool(Enemy.hashIsRun, false);

                    // 공격 직전: Enemy의 SetFacing을 실행하도록 FaceTarget 호출(타겟 방향으로 스프라이트 맞춤)
                    enemy.SetFacing();

                    enemy.animator.SetTrigger(Enemy.hashAttack);
                }
                else
                {
                    // 공격 범위 빡이면 추적 상태로 전환
                    enemy.ChangeState<ChaseState>();
                }
            }
        }
        //public void Update(Enemy enemy)
        //{
        //    Logger.Log("Acttack 갱신");

        //    enemy.DetectPlayer();

        //    // ⭐ 1. 상태 체크 먼저 (쿨타임 밖으로 이동)
        //    if (enemy.Target == null || enemy.Target.GetComponent<BasePlayer>()?.IsDead == true)
        //    {
        //        enemy.ChangeState<Idlestate>();
        //        return;
        //    }

        //    if (!enemy.IsInAttackRange())
        //    {
        //        enemy.ChangeState<ChaseState>();
        //        return;
        //    }

        //    // ⭐ 2. 쿨타임 체크는 그 다음
        //    if (Time.time - lastAttackTime < attackCooldown)
        //        return;

        //    // ⭐ 3. 공격 실행
        //    lastAttackTime = Time.time;

        //    enemy.animator.SetBool(Enemy.hashIsRun, false);
        //    enemy.SetFacing();
        //    enemy.animator.SetTrigger(Enemy.hashAttack);
        //}


        // 상태를 종료할 때 한 번 호출됨.
        // 공격 관련 리소스 정리
        public void Exit (Enemy enemy)
        {
            // 종료 로그. 상태 전환 전 필요한 정리 작업을 추가할 것
            Logger.Log("Acttack 종료");

            enemy.animator.ResetTrigger(Enemy.hashAttack);

        }

    }
}
