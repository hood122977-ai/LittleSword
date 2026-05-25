using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using LittleSword.Player.Weapon;

namespace LittleSword.Player
{
    public class ArcherSkill : MonoBehaviour
    {
        [Header("Skill Objects")]
        [SerializeField] private SpriteRenderer archerRenderer;  // 원래 Archer SpriteRenderer
        [SerializeField] private GameObject archerSkillEffect;   // 스킬용 Archer 오브젝트

        [Header("Skill Settings")]
        [SerializeField] private GameObject arrowPrefab;         // SkillArrow 프리팹
        [SerializeField] private Transform firePoint;            // 발사 위치
        [SerializeField] private int arrowCount = 7;             // 화살 수
        [SerializeField] private float spreadAngle = 60f;        // 부채꼴 각도

        [Header("UI")]
        [SerializeField] private Image cooldownPanel;
        [SerializeField] private GameObject cooldownPanelObject;

        private Archer archer;
        private bool isSkillActive = false;
        private bool isOnCooldown = false;
        private float cooldownTime = 10f;
        private float currentCooldown = 0f;

        private void Awake()
        {
            archer = GetComponent<Archer>();
        }

        private void Update()
        {
            // 쿨타임 처리
            if (isOnCooldown)
            {
                currentCooldown -= Time.deltaTime;
                cooldownPanel.fillAmount = currentCooldown / cooldownTime;

                if (currentCooldown <= 0)
                {
                    isOnCooldown = false;
                    cooldownPanelObject.SetActive(false);
                }
            }

            // E 키 → 스킬 활성화
            if (Keyboard.current.eKey.wasPressedThisFrame && !isSkillActive && !isOnCooldown)
            {
                ActivateSkill();
            }

            // 스킬 활성화 중 클릭 → 화살 발사
            if (isSkillActive && Mouse.current.leftButton.wasPressedThisFrame)
            {
                FireSkill();
            }
        }

        private void ActivateSkill()
        {
            isSkillActive = true;

            // 원래 Archer 투명
            Color c = archerRenderer.color;
            c.a = 0f;
            archerRenderer.color = c;

            // 스킬 Archer 활성화
            archerSkillEffect.SetActive(true);
        }

        private void FireSkill()
        {
            Vector2 baseDirection = archerRenderer.flipX ? Vector2.left : Vector2.right;
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

            float angleStep = arrowCount > 1 ? spreadAngle / (arrowCount - 1) : 0f;
            float startAngle = baseAngle - spreadAngle / 2f;

            // ⭐ 적 목록 미리 가져오기
            LittleSword.Enemy.Enemy[] enemies =
                FindObjectsByType<LittleSword.Enemy.Enemy>(FindObjectsSortMode.None);

            for (int i = 0; i < arrowCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Quaternion rot = Quaternion.Euler(0, 0, angle);

                GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rot);

                // ⭐ 각 화살 방향에서 가장 가까운 적 찾기
                Vector2 arrowDirection = rot * Vector2.right;
                Transform nearestEnemy = FindNearestEnemyInDirection(enemies, arrowDirection);

                arrow.GetComponent<SkillArrow>()?.Init(
                    archer.playerStats.fireForce,
                    archer.playerStats.attackDamage,
                    nearestEnemy);
            }

            DeactivateSkill();
        }

        private void DeactivateSkill()
        {
            isSkillActive = false;

            // 원래 Archer 복구
            Color c = archerRenderer.color;
            c.a = 255f;
            archerRenderer.color = c;

            // 스킬 Archer 비활성화
            archerSkillEffect.SetActive(false);

            // 쿨타임 시작
            isOnCooldown = true;
            currentCooldown = cooldownTime;
            cooldownPanelObject.SetActive(true);
            cooldownPanel.fillAmount = 1f;
        }

        // ⭐ 화살 방향 기준으로 가장 가까운 적 찾기
        private Transform FindNearestEnemyInDirection(LittleSword.Enemy.Enemy[] enemies, Vector2 direction)
        {
            Transform nearest = null;
            float minDist = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;

                Vector2 toEnemy = (enemy.transform.position - transform.position);

                // ⭐ 화살 방향과 비슷한 방향에 있는 적만 타겟
                float dot = Vector2.Dot(direction.normalized, toEnemy.normalized);
                if (dot < 0) continue; // 반대 방향 적 무시

                float dist = toEnemy.sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy.transform;
                }
            }

            return nearest;
        }
    }
}