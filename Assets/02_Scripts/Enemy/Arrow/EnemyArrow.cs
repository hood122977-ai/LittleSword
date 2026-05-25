using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Enemy.Weapon
{
    public class EnemyArrow : MonoBehaviour
    {
        private Rigidbody2D rigidbody;
        private float damage;
        private float force;
        private Transform target; // 추적 대상

        public void Init(float force, float damage)
        {
            this.force = force;
            this.damage = damage;
            rigidbody = GetComponent<Rigidbody2D>();

            // 레이어로 플레이어 찾기
            Collider2D playerCollider = Physics2D.OverlapCircle(
                transform.position, 100f, LayerMask.GetMask("Player"));

            if (playerCollider != null)
            {
                target = playerCollider.transform;
            }

            Destroy(gameObject, 3.0f);
        }

        private void FixedUpdate()
        {
            if (target == null) return;

            // 타겟 방향으로 회전
            Vector2 direction = (target.position - transform.position).normalized;
            rigidbody.linearVelocity = direction * force;

            // 화살 스프라이트 방향도 타겟 쪽으로 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.GetComponent<IDamageable>()?.TakeDamage((int)damage);
                Destroy(gameObject);
            }
        }
    }
}