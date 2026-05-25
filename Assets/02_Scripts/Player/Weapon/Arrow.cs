using UnityEngine;
using LittleSword.Interfaces;

namespace LittleSword.Player.Weapon
{
    // 발사체(화살) 동작을 담당하는 컴포넌트
    // - 생성 후 초기화(Init)로 힘과 데미지를 설정하고 Start에서 발사됨
    // - 충돌 시 IDamageable을 통해 피해를 전달하고 자신을 제거함
    public class Arrow : MonoBehaviour
    {
        private Rigidbody2D rigidbody; // 화살의 물리 제어용 Rigidbody2D 캐시

        public float force = 10.0f;     // 화살의 발사 함(초기값)
        public float damage = 30;     // 화살이 입히는 데미지(초기값)

        // 화살 초기화: 발사 힘과 데미지를 설정
        public void Init(float force, int damage)
        {
            this.force = force; // 전달받은 힘으로 덮어씀 
            this.damage = damage;   // 전달받은 뎅미지로 덮어씀
        }

        private void Start()
        {
            // Rigidbody2D를 캐싱하고 발사 직후 임펄스 힘을 가함
            rigidbody = GetComponent<Rigidbody2D>();

            //transform.right 방향으로 상대 힘을 가함(화살 프리팹의 오른쪽을 앞으로 가정)
            // AddRelativeForce를 사용하면 transform의 회전에 따라 힘 방향이 결정됨
            rigidbody.AddRelativeForce(transform.right * force, ForceMode2D.Impulse);

            // 일정 시간 후(3초) 자동 제거하여 씬 정리
            Destroy(gameObject, 3.0f);
        }

        // 트리거 콜백: 충돌한 대상이 적인지 검사하고 적이면 IDamageable을 통해 피해를 전달한 뒤 화살을 제거함
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 충돌한 대상이 "에너미" 태그를 가지고 있으면 피해를 전달
            if (other.CompareTag("Enemy"))
            {
                //IDamageable을 구현한 컴포넌트에 데미지 전달(널 체크 안전 호출)
                other.GetComponent<IDamageable>()?.TakeDamage((int)damage);

                // 적중 시 화살 제거
                Destroy(gameObject);
            }
        }
    }
}
