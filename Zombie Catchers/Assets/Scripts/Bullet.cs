using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 aimPos;
    public float destroyDis = 0.1f;
    void Start()
    {
        
    }


    void Update()
    {
        if (Vector3.Distance(transform.position, aimPos) <= destroyDis)
        {
            Destroy(gameObject);
        }
        
    }
}
