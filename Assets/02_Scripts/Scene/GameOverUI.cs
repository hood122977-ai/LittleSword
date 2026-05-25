using LittleSword.Enemy;
using LittleSword.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject diePanel;      // YOU DIE 패널
    [SerializeField] private GameObject nextPanel;     // YOU WIN 패널
    [SerializeField] private string nextSceneName;     // 인스펙터에서 다음 씬 이름 지정

    private BasePlayer player;

    private void Start()
    {
        diePanel.SetActive(false);
        nextPanel.SetActive(false);

        player = FindFirstObjectByType<BasePlayer>();
        if (player != null)
        {
            player.OnDie += ShowGameOver;
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnDie -= ShowGameOver;
        }
    }

    private void Update()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        if (enemies.Length > 0)
        {
            bool allDead = true;
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                ShowGameClear();
            }
        }
    }

    // 플레이어 사망 시 호출
    private void ShowGameOver()
    {
        diePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 적 전멸 시 호출
    private void ShowGameClear()
    {
        nextPanel.SetActive(true);
        Time.timeScale = 0f;
        enabled = false; // Update 중지
    }

    // Restart 버튼 연결
    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Next 버튼 연결
    public void OnClickNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}