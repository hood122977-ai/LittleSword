using UnityEngine;
using TMPro;
using LittleSword.Player;

namespace LittleSword.UI
{
    public class HpUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private BasePlayer player;

        private void Awake()
        {
            if (player == null) return;
            player.OnDie += OnPlayerDie;
        }

        private void OnDestroy()
        {
            if (player == null) return;
            player.OnDie -= OnPlayerDie;
        }

        private void Update()
        {
            if (player == null || hpText == null) return;
            if (player.IsDead) return; // ⭐ 사망 시 업데이트 중지

            hpText.text = $"HP: {player.CurrentHP}";
        }

        private void OnPlayerDie()
        {
            if (hpText == null) return;
            hpText.gameObject.SetActive(false);
        }
    }
}