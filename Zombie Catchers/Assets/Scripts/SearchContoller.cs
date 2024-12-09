using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Searcher : MonoBehaviour
{
    private float minYPosition = 100f;
    private float maxYPosition = 263f;
    private float minXPosition = 0f;
    private float maxXPosition = 640f;

    private float moveSpeed = 15f;
    private Vector3 targetPosition;
    private bool isPaused;
    private Animator animator;
    [SerializeField] private float searchDuration = 80f;
    private float searchTimer = 0f;
    public Text textTimer;
    public Button playButton;
    private bool isTimerRunning = true;
    public GameObject panel;
    public Button noButton;
    public Button yesButton;
    public Canvas canvasTimer;
    public Text countCoinsText;
    private AudioSource audioSource;
    public AudioClip searchSound;
    public AudioClip clickSound;

    void Start()
    {
        GenerateNewPosition();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        noButton.onClick.AddListener(ClosePanel);
        yesButton.onClick.AddListener(ClickYesButton);
        GameObject saveManagerObject = new GameObject("SaveManager");
        saveManagerObject.AddComponent<SaveManager>();

        UpdateCoinUI();
    }
    void Update()
    {

        if (searchTimer >= searchDuration)
        {
            StopSearching();
            return;
        }
        if (!isPaused)
        {
            searchTimer += Time.deltaTime;
            UpdateTimer();
            Move();
            if (isTimerRunning)
            {

                playButton.interactable = false;
            }
        }
       
    }
    public void UpdateCoinUI()
    {
        GameData data = SaveManager.Instance.LoadData();
        countCoinsText.text = data.coins.ToString();
    }
    private void ClickYesButton()
    {
        StopSearching();
        audioSource.PlayOneShot(clickSound);
        panel.SetActive(false);
        GameData data = SaveManager.Instance.LoadData();
        data.coins -= 1000; 
        SaveManager.Instance.SaveData(data);
        UpdateCoinUI();

    }
    private void ClosePanel()
    {
        audioSource.PlayOneShot(clickSound);
        panel.SetActive(false);
    }
    private void OnMouseDown()
    {
        panel.SetActive(true);
    }
    #region generatePosition
    private float RandomGenerationXPosition()
    {
        return Random.Range(minXPosition, maxXPosition);
    }
    private float RandomGenerationYPosition()
    {
        return Random.Range(minYPosition, maxYPosition);
    }

    private void GenerateNewPosition()
    {
        targetPosition = new Vector3(RandomGenerationXPosition(), RandomGenerationYPosition(), transform.position.z);
    }
    #endregion
    private void Move()
    {

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < 1f)
        {
            StartCoroutine(SerchProcess());
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            animator.SetBool("isMoved", true);
        }
    }
    private IEnumerator SerchProcess()
    {
        isPaused = true;
        animator.SetBool("isMoved", false);

        float duration = 3f; 
        float soundInterval = 0.3f; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            audioSource.PlayOneShot(searchSound); 
            yield return new WaitForSeconds(soundInterval);
            elapsedTime += soundInterval;
        }

        GenerateNewPosition();
        isPaused = false;
    }

    private void StopSearching()
    {
        isPaused = true;
        isTimerRunning = false;
        playButton.interactable = true;
        animator.SetBool("isMoved", false);
        canvasTimer.gameObject.SetActive(false);
    }
    private void UpdateTimer()
    {

        float remainingTime = searchDuration - searchTimer;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        if (minutes > 0)
        {
            textTimer.text = $"{minutes}:{seconds:D2}";
        }
        else
        {
            textTimer.text = $"{seconds} ";
        }

    }
}
