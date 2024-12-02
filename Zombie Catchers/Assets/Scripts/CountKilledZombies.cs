using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountKilledZombies : MonoBehaviour
{
    public static CountKilledZombies Instance;
    public int zombieKillCount = 0;
    public Text killCountText;
    public TileGeneration tileGeneration;

    public GameObject endGamePanel;
    public Button endGameButton;
    public Text textInGamePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
        
    }

    private void Start()
    {
        // Скрываем панель завершения игры при старте
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        // Назначаем слушатель кнопки завершения
        if (endGameButton != null)
        {
            endGameButton.onClick.RemoveAllListeners(); // Убираем старые слушатели
            endGameButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Map");
                if (endGamePanel != null) endGamePanel.SetActive(false);
            });
        }
    }

    public void IncrementKillCount()
    {
        zombieKillCount++;
        UpdateKillCount();
    }

    public void UpdateKillCount()
    {
        // Обновляем текст счётчика
        if (killCountText != null && tileGeneration != null)
        {
            killCountText.text = $"{zombieKillCount} / {tileGeneration.countZombies}";
        }
        StopGame();
    }

    private void StopGame()
    {
        // Проверяем, завершена ли игра
        if (tileGeneration != null && zombieKillCount == tileGeneration.countZombies)
        {
            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true);
            }

            if (textInGamePanel != null)
            {
                textInGamePanel.text = $"Поймано зомби: {zombieKillCount}";
            }
        }
    }

}
