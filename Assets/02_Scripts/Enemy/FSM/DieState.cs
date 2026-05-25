using UnityEngine;

namespace LittleSword.Enemy.FSM
{
    public class DieState : IState
    {
        public void Enter(Enemy enemy)
        {
            enemy.animator.SetTrigger(Enemy.hashDie); // 사망 애니메이션 트리거 재생

            enemy.StopMoving();                       // 이동을 멈추도록 물리/모션을 정지

            // 충돌체를 비활성화하여 더 이상 충돌 처리나 공격을 받지 않도록 함
            enemy.GetComponent<Collider2D>().enabled = false;

            enemy.rigidbody.bodyType = RigidbodyType2D.Kinematic; // 물리 연산을 중단(충돌 반응 없앰)
            
            // 일정 시간 후 게임오브젝트 제거(잔여 이펙트/ 애니메이션 재생 허용)
            Object.Destroy(enemy.gameObject, 5.0f);
        }

        public void Update(Enemy enemy)
        {

        }

        public void Exit(Enemy enemy)
        {

        }
       
    }
}
