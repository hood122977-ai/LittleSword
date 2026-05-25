using UnityEngine;

// ScriptableObject로 플레이어 기본 스탯을 에디터에서 쉽게 생성/편집하도록 함
[CreateAssetMenu(fileName = "PlayerStatsSo", menuName = "LittleSword/PlayerStats", order = 0)]
public class PlayerStats : ScriptableObject
{
    // 플레이어 최대 체력(HP), 게임 내에서 현재 체력의 상한으로 사용됨.
    public int maxHP = 100;

    // 플레이어 기본 이동속도. 이동 로직에서 곱셈에 사용됨
    public float moveSpeed = 5f;

    // 플레이어의 기본 공격력.공격 판정시 이값을 참조하여 데미지를 계산함
    public int attackDamage = 20;

    // 플레이어의 화살 발사 힘(유닛: 유니티 단위). 화살 발사 시 Rigidbody2D에 가하는 힘으로 사용됨.
    public float fireForce = 10f;
}
