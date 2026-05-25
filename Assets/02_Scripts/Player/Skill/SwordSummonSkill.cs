using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using LittleSword.Player.Weapon;

namespace LittleSword.Player
{
    public class SwordSummonSkill : MonoBehaviour
    {
        [Header("Skill Settings")]
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private Transform[] summonPoints;

        [Header("UI")]
        [SerializeField] private Image cooldownPanel;
        [SerializeField] private GameObject cooldownPanelObject;

        private GameObject[] summonedSwords;
        private bool isSkillActive = false;
        private bool isOnCooldown = false;
        private float cooldownTime = 10f;
        private float currentCooldown = 0f;

        private void Update()
        {
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

            if (Keyboard.current.eKey.wasPressedThisFrame && !isSkillActive && !isOnCooldown)
            {
                ActivateSkill();
            }

            if (isSkillActive && Mouse.current.leftButton.wasPressedThisFrame)
            {
                FireSwords();
            }
        }

        private void ActivateSkill()
        {
            isSkillActive = true;
            summonedSwords = new GameObject[summonPoints.Length];

            for (int i = 0; i < summonPoints.Length; i++)
            {
                GameObject sword = Instantiate(swordPrefab, summonPoints[i].position, Quaternion.identity);

                sword.GetComponent<SwordProjectile>()?.SetOwner(
                    transform,
                    summonPoints[i].localPosition
                );

                summonedSwords[i] = sword;
            }
        }

        private void FireSwords()
        {
            isSkillActive = false;

            LittleSword.Enemy.Enemy[] enemies =
                FindObjectsByType<LittleSword.Enemy.Enemy>(FindObjectsSortMode.None);

            foreach (var sword in summonedSwords)
            {
                if (sword == null) continue;

                Transform nearestEnemy = FindNearestEnemy(enemies, sword.transform.position);
                sword.GetComponent<SwordProjectile>()?.Launch(nearestEnemy);
            }

            StartCooldown();
        }

        private Transform FindNearestEnemy(LittleSword.Enemy.Enemy[] enemies, Vector3 from)
        {
            Transform nearest = null;
            float minDist = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;

                float dist = (from - enemy.transform.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy.transform;
                }
            }

            return nearest;
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