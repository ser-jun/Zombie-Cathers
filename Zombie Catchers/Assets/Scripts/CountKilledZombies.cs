using UnityEngine;
using UnityEngine.UI;

public class CountKilledZombies : MonoBehaviour
{
    public static CountKilledZombies Instance;
    public int zombieKillCount = 0;
    public Text killCountText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

    }
    public void IncrementKillCount()
    {
        zombieKillCount++;
        UpdateKillCount();
    }
    private void UpdateKillCount()
    {
        if(killCountText != null) 
        {
        killCountText.text= "Kill count:" + zombieKillCount;
        }
    }
}
