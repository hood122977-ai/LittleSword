using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Player.Weapon
{
    public class SwordProjectile : MonoBehaviour
    {
        [SerializeField] private float force = 8f;
        [SerializeField] private int damage = 20;
        [SerializeField] private float homingStrength = 2f;
        [SerializeField] private float homingDelay = 0.5f;

        private Rigidbody2D rb;
        private Transform target;
        private Vector2 currentDirection;
        private float elapsedTime = 0f;
        private bool isLaunched = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void SetOwner(Transform owner, Vector3 localOffset)
        {
            transform.SetParent(owner);
            transform.localPosition = localOffset;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        public void Launch(Transform target)
        {
            this.target = target;
            isLaunched = true;
            elapsedTime = 0f;

            transform.SetParent(null);
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = false;

            if (target != null)
                currentDirection = (target.position - transform.position).normalized;
            else
                currentDirection = transform.right;

            Destroy(gameObject, 3f);
        }

        private void FixedUpdate()
        {
            if (!isLaunched) return;

            elapsedTime += Time.fixedDeltaTime;

            if (target != null && elapsedTime >= homingDelay)
            {
                Vector2 targetDirection = (target.position - transform.position).normalized;
                currentDirection = Vector2.Lerp(currentDirection, targetDirection,
                    homingStrength * Time.fixedDeltaTime).normalized;
            }

            rb.linearVelocity = currentDirection * force;

            // ⭐ 프리팹 Z가 -90이라 오프셋 추가
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isLaunched) return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.GetComponent<IDamageable>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}