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
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(false);
        }

        if (endGameButton != null)
        {

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

            killCountText.text = $"{zombieKillCount} / {tileGeneration.countZombies}";
 
        StopGame();
    }

    private void StopGame()
    {

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
