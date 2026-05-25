using UnityEngine;

namespace LittleSword.Player.Controller
{
    public class AnimationController
    {
        private readonly Animator animator;

        // 애니메이션 파리메터 해시
        private static readonly int hashIsRun = Animator.StringToHash("IsRun");
        private static readonly int hashAttack = Animator.StringToHash("Attack");
        private static readonly int hashDie = Animator.StringToHash("Die");
        private static readonly int hashHit = Animator.StringToHash("Hit");

        // 생성자 
        public AnimationController(Animator animator)
        {
            this.animator = animator;
        }

        #region 애니메이션 변경 메서드
        public void Move(bool isMoving)
        {
            animator.SetBool(hashIsRun, isMoving);
        }

        public void Attack()
        {
            animator.SetTrigger(hashAttack);
        }

        public void Die()
        {
            animator.SetTrigger(hashDie);
        }

        public void Hit()
        {
            animator.SetTrigger(hashHit);
        }
        #endregion

    }
}
