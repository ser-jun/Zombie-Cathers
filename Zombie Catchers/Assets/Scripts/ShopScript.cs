using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Button shopButton;
    public Button closeShopPanel;
    public GameObject shopPanel;
    void Start()
    {
        shopButton.onClick.AddListener(()=>shopPanel.SetActive(true));
        closeShopPanel.onClick.AddListener(()=>shopPanel.SetActive(false));
    }

    void Update()
    {
        
    }
}
