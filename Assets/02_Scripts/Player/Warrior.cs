using UnityEngine;
using LittleSword.Player;
using LittleSword.Interfaces;

namespace LittleSword.Player
{
    public class Warrior : BasePlayer
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Vector2 size = new Vector2(1.0f, 2.0f);

        // 공격 판정 영역의 중심 오프셋. 예: (0.5f, 0)
        // -> 플레이어 오른쪽으로 0.5 유닛 떨어진 위치에 공격 판정이 생성됨
        [SerializeField] private float offset = 0.5f;

        // 애니메이션 이벤트에서 호출 되는 공격 판정 메서드
        public void OnWarriorAttack()
        {
            // 플레이어가 왼쪽을 바라보고 있으면 공격 판정이 왼쪽으로,
            // 오른쪽을 바라보고 있으면 오른쪽으로 생성되도록 방향 계산
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            // 공격 판정 로직 구현
            Vector2 center = (Vector2)transform.position + direction * offset;

            // OverlapBoxAll을 사용하여 공격 판정 영역 내의 모든 
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0, enemyLayer);

            foreach (var collider in colliders)
            {
                collider.GetComponent<IDamageable>()?.TakeDamage(playerStats.attackDamage);
            }
        }

        private void OnDrawGizmos()
        {
            // 편집 모드에서도 spriteRenderer 참조를 시도하여 공격 판정 영역을 시각적으로 표시
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            // 플레이어가 왼쪽을 바라보고 있으면 공격 판정이 왼쪽으로,
            Vector2 direction = spriteRenderer.flipX ? Vector2.left: Vector2.right;

            // 공격 판정 영역의 중심 위치 계산
            Vector2 center = (Vector2)transform.position + direction * offset;

            // 공격 판정 영역을 시각적으로 표시하기 위한 Gizmos 그리기
            // 빨강색으로 반명 설정
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);    

            // 공격 판정 영역을 와이어프레임으로 표시
            Gizmos.DrawCube(center, new Vector3(size.x, size.y, 0.0f));

            //Vector2 direction = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
            //Vector2 center = (Vector2)transform.position + direction * offset;

            //Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            //Gizmos.DrawCube(center, new Vector3(size.x, size.y, 0.0f));

        }

        public Collider2D[] GetAttackColliders()
        {
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)transform.position + direction * offset;
            return Physics2D.OverlapBoxAll(center, size, 0, enemyLayer);
        }
    }
}
