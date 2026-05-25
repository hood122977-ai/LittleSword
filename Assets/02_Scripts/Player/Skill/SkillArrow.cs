using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Player.Weapon
{
    public class SkillArrow : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Transform target;
        private float force;
        private int damage;
        private Vector2 currentDirection; // ⭐ 현재 방향
        [SerializeField] private float homingStrength = 2f; // ⭐ 유도 강도 (낮을수록 천천히 꺾임)

        [SerializeField] private float homingDelay = 0.5f; // ⭐ 유도 시작 딜레이
        private float elapsedTime = 0f;

        [SerializeField] private GameObject explosionPrefab;

        public void Init(float force, int damage, Transform target)
        {
            this.force = force;
            this.damage = damage;
            this.target = target;
            rb = GetComponent<Rigidbody2D>();
            currentDirection = transform.right;
            elapsedTime = 0f; // ⭐ 타이머 초기화
            Destroy(gameObject, 3f);
        }

        private void FixedUpdate()
        {
            elapsedTime += Time.fixedDeltaTime;

            // ⭐ 딜레이 이후부터 유도 시작
            if (target != null && elapsedTime >= homingDelay)
            {
                Vector2 targetDirection = (target.position - transform.position).normalized;
                currentDirection = Vector2.Lerp(currentDirection, targetDirection,
                    homingStrength * Time.fixedDeltaTime).normalized;
            }

            rb.linearVelocity = currentDirection * force;

            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.GetComponent<IDamageable>()?.TakeDamage(damage);

                // ⭐ 폭발 생성
                if (explosionPrefab != null)
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }


    }
}