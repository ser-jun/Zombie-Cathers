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
    private AudioSource audioSource;
    public AudioClip endGameSound;
    private bool isPlayed=false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                if(!audioSource.isPlaying && !isPlayed)
                {
                    Cursor.visible = true;
                    audioSource.PlayOneShot(endGameSound);
                    isPlayed = true;
                }
                endGamePanel.SetActive(true);
            }

            if (textInGamePanel != null)
            {
                textInGamePanel.text = $"Поймано зомби: {zombieKillCount}";
            }
        }
    }

}
