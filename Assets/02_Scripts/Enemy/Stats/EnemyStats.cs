using UnityEngine;

namespace LittleSword.Enemy.Stats
{
    // 적 기본 스탯을 저장하는 ScriptableObject
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "LittleSword/EnemyStats", order = 0)]
    public class EnemyStats : ScriptableObject
    {
        [Header("Enemy Basic Stats")]
        public int maxHP = 100; // 최대체력
        public float moveSpeed = 3f; // 이동속도

        [Header("Enemy Detection Stats")]
        public float detecInterval = 0.1f; // 플레이어 감지 간격
        
        [Header("Enemy Combat Stats")]
        public float chaseDistance = 5f; // 추적 시작 거리
        public float attackDistance = 1.5f; // 공격 가능 거리
        public int attackDamage = 10; // 공격 피해
        public float attackCooldown = 1f; // 공격 쿨다운

        [Header("Ranged Stats")]
        public float arrowForce = 10f;

    }

}