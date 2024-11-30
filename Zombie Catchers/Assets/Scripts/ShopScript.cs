using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Button shopButton;
    public Button closeShopPanel;
    public GameObject shopPanel;
    public GameObject garpunPref;
    private List<GameObject> listWeapon;
    void Start()
    {
        shopButton.onClick.AddListener(()=>shopPanel.SetActive(true));
        closeShopPanel.onClick.AddListener(()=>shopPanel.SetActive(false));
         listWeapon = Player.Instance.weapons;
    }

    void Update()
    {
        
    }
    private void BuyWeapon(GameObject weapon) //Добавить использование UpdateCointCoins через синглтон в SaveManager 
    {
        listWeapon.Add(weapon);
        GameData data = SaveManager.Instance.LoadData();
        data.coins -= 2500;
        SaveManager.Instance.SaveData(data);    
    }
}
