using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LittleSword.Player
{
    public class SelfDestructSkill : MonoBehaviour
    {
        [Header("Skill Objects")]
        [SerializeField] private SpriteRenderer warriorRenderer;
        [SerializeField] private GameObject warriorSkillEffect;

        [Header("Skill Settings")]
        [SerializeField] private GameObject explosionPrefab;

        [Header("UI")]
        [SerializeField] private Image cooldownPanel;
        [SerializeField] private GameObject cooldownPanelObject;

        private BasePlayer basePlayer;
        private bool isSkillActive = false;
        private bool isOnCooldown = false;
        private float cooldownTime = 10f;
        private float currentCooldown = 0f;

        private void Awake()
        {
            warriorRenderer = GetComponent<SpriteRenderer>();
            basePlayer = GetComponent<BasePlayer>();
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

            // 스킬 활성화 중 클릭 → 폭발 발동
            if (isSkillActive && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Explode();
            }
        }

        private void ActivateSkill()
        {
            isSkillActive = true;

            // Warrior 색깔 변경
            warriorRenderer.color = new Color(72f / 255f, 166f / 255f, 251f / 255f, 1f);

            if (warriorSkillEffect != null)
                warriorSkillEffect.SetActive(true);
        }

        private void Explode()
        {
            isSkillActive = false;

            // ⭐ 워리어에게 99 데미지
            basePlayer.TakeDamage(99);

            // Explosions 프리팹 소환
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // 색깔 복구
            warriorRenderer.color = Color.white;

            if (warriorSkillEffect != null)
                warriorSkillEffect.SetActive(false);

            StartCooldown();
        }

        private void StartCooldown()
        {
            isOnCooldown = true;
            currentCooldown = cooldownTime;
            cooldownPanelObject.SetActive(true);
            cooldownPanel.fillAmount = 1f;
        }
    }
}