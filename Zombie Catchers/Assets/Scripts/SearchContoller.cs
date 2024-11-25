using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Searcher : MonoBehaviour
{
    private float minYPosition = 100f;
    private float maxYPosition = 263f;
    private float minXPosition = -15f;
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
    void Start()
    {
        GenerateNewPosition();
        animator = GetComponent<Animator>();
        playButton = FindObjectOfType<Button>();

    }
    void Update()
    {

        if (searchTimer >=searchDuration)
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
                //SetButtonColor(Color.black);
                playButton.interactable = false;
            }
        }
    }
    //private void SetButtonColor(Color color)
    //{
    //    if (playButton != null)
    //    {
    //        ColorBlock buttonColors = playButton.colors;
    //        buttonColors.normalColor = color;
    //        playButton.colors = buttonColors;
    //    }
    //}
    private float RandomGenerationXPosition ()
    {
        return Random.Range(minXPosition, maxXPosition);
    }
    private float RandomGenerationYPosition ()
    {
        return Random.Range(minYPosition, maxYPosition);
    }

   private void GenerateNewPosition()
    {
        targetPosition= new Vector3(RandomGenerationXPosition(), RandomGenerationYPosition(), transform.position.z);
    }
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
        yield return new WaitForSeconds(3f);
        GenerateNewPosition();
        isPaused = false;   
    }
    private void StopSearching()
    {
        isPaused=true;
        isTimerRunning = false;
        playButton.interactable = true;
        //SetButtonColor(Color.white);
        animator.SetBool("isMoved", false );
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
