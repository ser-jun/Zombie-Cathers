using UnityEngine;
using UnityEngine.UI;

public class CoinController : MonoBehaviour
{
    private float attractionalRadius = 3f;
    private float attractionalSpeed = 5f;
    private Transform playerTransform;
    private Player player;
    public Text coinsCount;
    private AudioSource audioSource;
    public AudioClip tookCoin;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        coinsCount = GameObject.Find("CoinsCount").GetComponent<Text>();
        player = FindObjectOfType<Player>();
        UpdateCoinsCountUI(coinsCount);

    }

    void Update()
    {
        playerTransform = player.transform;
        AttractionalToPlayer();
    }
    private void AttractionalToPlayer()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance < attractionalRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractionalSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           // audioSource.PlayOneShot(tookCoin);
            PlaySound(tookCoin);
            Destroy(gameObject);
            GameData data = SaveManager.Instance.LoadData();
            data.coins += Random.Range(40, 100);
            SaveManager.Instance.SaveData(data);
            UpdateCoinsCountUI(coinsCount);
        }
    }
    private void PlaySound(AudioClip clip)
    {
        GameObject soundObject = new GameObject("CoinSound");
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        Destroy(soundObject, clip.length); 
    }


    public void UpdateCoinsCountUI(Text coinsCountInText)
    {

        GameData data = SaveManager.Instance.LoadData();
        coinsCountInText.text = data.coins.ToString();
    }

}
