using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Player.Weapon
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float explosionRadius = 1.5f;
        [SerializeField] private Vector2 explosionOffset = Vector2.zero; // ⭐ 위치 오프셋
        [SerializeField] private int explosionDamage = 10;
        [SerializeField] private LayerMask enemyLayer;

        private void Start()
        {
            // ⭐ 오프셋 적용한 위치로 폭발 범위 계산
            Vector2 center = (Vector2)transform.position + explosionOffset;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                center, explosionRadius, enemyLayer);

            foreach (var collider in colliders)
            {
                collider.GetComponent<IDamageable>()?.TakeDamage(explosionDamage);
            }

            Animator animator = GetComponent<Animator>();
            float duration = animator != null ?
                animator.GetCurrentAnimatorStateInfo(0).length : 1f;
            Destroy(gameObject, duration);
        }

        private void OnDrawGizmos()
        {
            Vector2 center = (Vector2)transform.position + explosionOffset;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(center, explosionRadius); // ⭐ 오프셋 반영
        }
    }
}