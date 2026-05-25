using UnityEngine;
using Logger = LittleSword.Common.Logger;
using LittleSword.Player;
using LittleSword.Enemy.Weapon;

namespace LittleSword.Enemy.FSM
{
    public class RangedAttackState : IState
    {
        private readonly float attackCooldown;
        private float lastAttackTime;

        public RangedAttackState(float attackCooldown = 2.0f)
        {
            this.attackCooldown = attackCooldown;
            lastAttackTime = Time.time - attackCooldown;
        }

        public void Enter(Enemy enemy)
        {
            Logger.Log("RangedAttack 진입");
            enemy.StopMoving();
            enemy.animator.SetBool(Enemy.hashIsRun, false);
            lastAttackTime = Time.time - attackCooldown;
        }

        public void Update(Enemy enemy)
        {
            Logger.Log("RangedAttack 갱신");

            enemy.DetectPlayer();

            if (enemy.Target == null || enemy.Target.GetComponent<BasePlayer>()?.IsDead == true)
            {
                enemy.ChangeState<Idlestate>();
            }
            else if (!enemy.IsInChaseRange()) // 추적 범위 벗어나면 Idle
            {
                enemy.ChangeState<Idlestate>();
            }
            else if (enemy.IsInAttackRange()) // 근접 범위 안에 들어오면 뒤로 물러남
            {
                enemy.MoveAwayFromPlayer();
            }
            else if (Time.time - lastAttackTime >= attackCooldown)
            {
                enemy.SetFacing();
                enemy.animator.SetTrigger(Enemy.hashAttack);
                lastAttackTime = Time.time;
            }
        }

        public void Exit(Enemy enemy)
        {
            Logger.Log("RangedAttack 종료");
            enemy.animator.ResetTrigger(Enemy.hashAttack);
        }
    }
}