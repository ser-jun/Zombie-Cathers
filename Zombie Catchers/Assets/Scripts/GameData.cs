using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int zombieKillCount;
    public int coins;


    public List<int> upgradeWeapons = new List<int>() { 0,0,0};
    public List<string> price = new List<string>() { null, null, null };

}
