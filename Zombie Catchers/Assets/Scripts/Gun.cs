using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float rotateShift;
  
    void Start()
    {
        
    }


    void Update()
    {
   
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.LookAt(cursorPos, Vector3.forward);
        transform.rotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z + rotateShift);
   
    }
    
}
