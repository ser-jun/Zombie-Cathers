using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform aimTransform;
    public Transform playerTransform;
    public float maxDistance = 6f;
    public float minDistance = 1.6f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cursorPos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = 0;
        aimTransform.position = cursorPos;

        Vector3 dir= cursorPos - playerTransform.position;
        if (dir.magnitude> maxDistance )
        {
            dir = dir.normalized*maxDistance;
        }
        aimTransform.position= playerTransform.position +dir;
        if(dir.magnitude< minDistance )
        {
            dir = dir.normalized*minDistance;

        }
        aimTransform.position=playerTransform.position +dir;
    }
    
}
