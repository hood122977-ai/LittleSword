using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Enemy.Weapon
{
    public class FireProjectile : MonoBehaviour
    {
        private Rigidbody2D rb;
        private float damage;

        public void Init(float speed, float damage)
        {
            this.damage = damage;
            rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * speed;
            Destroy(gameObject, 3f);
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