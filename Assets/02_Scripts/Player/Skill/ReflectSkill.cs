using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using LittleSword.Enemy.Weapon;

namespace LittleSword.Player
{
    public class ReflectSkill : MonoBehaviour
    {
        [Header("Warrior Objects")]
        [SerializeField] private SpriteRenderer warriorRenderer;  // 원래 Warrior
        [SerializeField] private GameObject warriorSkillEffect;   // Warrior(1)

        [Header("Attack Settings")]
        [SerializeField] private Vector2 attackSize = new Vector2(1.0f, 2.0f);
        [SerializeField] private float attackOffset = 0.5f;
        [SerializeField] private LayerMask arrowLayer;            // 화살 레이어

        [Header("UI")]
        [SerializeField] private Image cooldownPanel;             // Fill Amount 이미지
        [SerializeField] private GameObject cooldownPanelObject;  // 판넬 오브젝트

        private SpriteRenderer spriteRenderer;
        private bool isSkillActive = false;
        private bool isOnCooldown = false;
        private float cooldownTime = 10f;
        private float currentCooldown = 0f;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // 쿨타임 처리
            if (isOnCooldown)
            {
                currentCooldown -= Time.deltaTime;
                cooldownPanel.fillAmount = currentCooldown / cooldownTime; // 0으로 감소

                if (currentCooldown <= 0)
                {
                    isOnCooldown = false;
                    cooldownPanelObject.SetActive(false); // 쿨타임 끝나면 판넬 숨김
                }
            }

            // E 키 입력
            if (Keyboard.current.eKey.wasPressedThisFrame && !isSkillActive && !isOnCooldown)
            {
                ActivateSkill();
            }

            // 스킬 활성화 중 공격 감지
            if (isSkillActive)
            {
                CheckArrowDeflect();
            }
        }

        private void ActivateSkill()
        {
            isSkillActive = true;

            // Warrior 투명
            Color c = warriorRenderer.color;
            c.a = 0f;  // ⭐ 이미 있음
            warriorRenderer.color = c;

            warriorSkillEffect.SetActive(true);
        }

        private void CheckArrowDeflect()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame &&
                !Keyboard.current.spaceKey.wasPressedThisFrame) return;

            Vector2 direction = warriorRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)transform.position + direction * attackOffset;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, attackSize, 0, arrowLayer);

            bool arrowDetected = false;

            foreach (var collider in colliders)
            {
                if (collider.GetComponent<EnemyArrow>() != null)
                {
                    Destroy(collider.gameObject); // ⭐ 범위 안 화살 전부 제거
                    arrowDetected = true;         // ⭐ 화살 감지 여부 기록
                }
            }

            // 화살이 하나라도 있었으면 스킬 종료
            if (arrowDetected)
            {
                DeactivateSkill();
            }
        }

        private void DeactivateSkill()
        {
            isSkillActive = false;

            // Warrior 불투명 복구
            Color c = warriorRenderer.color;
            c.a = 255f;
            warriorRenderer.color = c;

            // Warrior(1) 비활성화
            warriorSkillEffect.SetActive(false);

            // 쿨타임 시작
            isOnCooldown = true;
            currentCooldown = cooldownTime;
            cooldownPanelObject.SetActive(true);  // 판넬 활성화
            cooldownPanel.fillAmount = 1f;        // Fill Amount 1부터 시작
        }

        private void OnDrawGizmos()
        {
            if (warriorRenderer == null) return;

            Vector2 direction = warriorRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)transform.position + direction * attackOffset;

            // 스킬 활성화 중이면 초록색, 아니면 파란색
            Gizmos.color = isSkillActive ?
                new Color(0f, 1f, 0f, 0.3f) :
                new Color(0f, 0f, 1f, 0.3f);

            Gizmos.DrawCube(center, new Vector3(attackSize.x, attackSize.y, 0f));
        }
    }
}