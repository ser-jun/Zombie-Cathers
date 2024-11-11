using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List <GameObject> listOfObjects = new List <GameObject> ();
   
    public void AddObject (GameObject obj)
    {
        listOfObjects.Add(obj);
    }
   
    public void RemoveObject (GameObject obj)
    {

        listOfObjects.Add(obj);
    }
}
