using LittleSword.Enemy.FSM;
using LittleSword.Enemy.Stats;
using LittleSword.Enemy.Weapon;
using UnityEngine;

namespace LittleSword.Enemy
{
    // Enemy를 상속받은 원거리 적 아처
    public class EnemyArcher : Enemy
    {
        [SerializeField] private GameObject arrowPrefab;  // 화살 프리팹
        [SerializeField] private Transform firePoint;      // 발사 위치

        // Awake 오버라이드 - 상태 딕셔너리에 RangedAttackState 추가
        protected override void InitStates()
        {
            base.InitStates();
            // AttackState 대신 RangedAttackState 사용
            states[typeof(RangedAttackState)] = new RangedAttackState(enemyStats.attackCooldown);
        }

        // 애니메이션 이벤트에서 호출
        public void OnArcherAttackEvent()
        {
            FireArrow();
        }

        private void FireArrow()
        {
            if (arrowPrefab == null || firePoint == null) return;

            // flipX 방향으로 화살 회전 설정
            Quaternion rot = Quaternion.Euler(0, spriteRenderer.flipX ? 180 : 0, 0);

            // 화살 생성
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rot);

            // 화살 초기화
            arrow.GetComponent<EnemyArrow>()?.Init(enemyStats.arrowForce, enemyStats.attackDamage);
        }
    }
}