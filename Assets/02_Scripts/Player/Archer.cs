using UnityEngine;
using LittleSword.Player.Weapon;

namespace LittleSword.Player
{
    // 궁수 플레이어 클래스: BasePlayer를 상속하여 화살 발사 기능을 구현
    public class Archer : BasePlayer
    {
        [SerializeField] private GameObject ArrowPrefab; // 화살 프리팹 (인스펙터에서 연결)
        [SerializeField] private Transform firePoint; // 화살 생성 및 발사 지점

        // 애니메이터 이벤트에서 호출됨 - 공격 애니메이션 타이밍에 맞춰 실행
        public void OnArcherAttackEvent()
        {
            FierArrow();    // 실제 화살 생성/발사 로직 호출
        }

        // 화살 생성 및 초기화
        private void FierArrow()
        {
            // 스프라이트가 반전 되어 있으면 Y축 180도 회전으로 발사 방향 보정
            Quaternion rot = Quaternion.Euler(0, spriteRenderer.flipX ? 180 : 0, 0);

            // 화살 프리팹을 발사 위치에 인스턴스화
            GameObject arrow = Instantiate(ArrowPrefab, firePoint.position, rot);

            // 생성된 화살 초기화: 발사력과 데미지를 플레이어 스탯에서 전달
            arrow.GetComponent<Arrow>().Init(playerStats.fireForce, playerStats.attackDamage);
        }

    }
}
