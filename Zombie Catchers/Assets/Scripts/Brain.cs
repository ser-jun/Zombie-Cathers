using System.Collections;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private SpriteRenderer sp;
    public Sprite[] spritesBrain;
    private float timeBetweenChangeSprites = 1f;
    private int indexSprite = 0;
    private bool isEating = false;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
  
    }


    public void StartEating ()
    {
        if (!isEating)
        {
            isEating = true;
            StartCoroutine(BrainEatingProcess());
        }
    }
    public IEnumerator BrainEatingProcess()
    {
        while (indexSprite < spritesBrain.Length)
        {
       sp.sprite = spritesBrain[indexSprite];
            indexSprite++;
            yield return new WaitForSeconds(timeBetweenChangeSprites);
        }
        Destroy(gameObject);

    }
}
