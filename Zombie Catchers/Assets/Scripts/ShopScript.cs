using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Button shopButton;
    public Button closeShopPanel;

    public Button buyGarpun;
    public Button buyDartGunButton;

    public Button gunShopPanelButton;
    public Button closeGunShopPanel;
    public GameObject shopGunPanel;
    public Searcher searcher;

    public GameObject shopPanel;


    public GameObject upgradeShopPanel;
    public Button upgradeShopButtonOpen;
    public Button closeUpgradePanel;
    public Color color;

    public Button upgradeGunButton;
    public List<GameObject> miniPanelIndicatorGun = new List<GameObject>();
    public Text textPriceGun;

    public Button upgradeGarpunButton;
    public List<GameObject> miniPanelIndicatorGarpun = new List<GameObject>();
    public Text textPriceGarpun;

    public GameObject additionalShopPanel;
    public Button closeAdditionalPanelButton;
    public Button openAdditionalPanelButton;
    public Button buyCap;
    public Button chooseCapButton;
    private AudioSource audioSource;
    public AudioClip buySound;
    public AudioClip menuSound;

    public GameObject exitGamePanel;
    public Button continueButton;
    public Button exitGameButton;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  
        GameData data = SaveManager.Instance.LoadData();
        shopButton.onClick.AddListener(() => 
        {
            shopPanel.SetActive(true);
            audioSource.PlayOneShot(menuSound);
        });
        closeShopPanel.onClick.AddListener(() => 
        {
            shopPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });

        gunShopPanelButton.onClick.AddListener(() =>
        {
            shopGunPanel.SetActive(true);
            shopPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });
        closeGunShopPanel.onClick.AddListener(() => 
        {
            shopGunPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });
        buyGarpun.onClick.AddListener(() => BuyItem("upgradeWeapons", 1, 2000, buyGarpun));
        buyDartGunButton.onClick.AddListener(() => BuyItem("upgradeWeapons", 2, 3500, buyDartGunButton));

        upgradeShopButtonOpen.onClick.AddListener(() =>
        {
            upgradeShopPanel.SetActive(true);
            shopPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });
        closeUpgradePanel.onClick.AddListener(() => 
        {
            upgradeShopPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });


        openAdditionalPanelButton.onClick.AddListener(() =>
         {
             additionalShopPanel.SetActive(true);
             shopPanel.SetActive(false);
             audioSource.PlayOneShot(menuSound);
         });
        closeAdditionalPanelButton.onClick.AddListener(() => 
        {
            additionalShopPanel.SetActive(false);
            audioSource.PlayOneShot(menuSound);
        });
        buyCap.onClick.AddListener(() => BuyItem("caps",0, 2000, buyCap));
        chooseCapButton.onClick.AddListener(() =>SellCap());
  

        upgradeGunButton.onClick.AddListener(() => UpgradeWeapon(0, upgradeGunButton, miniPanelIndicatorGun, textPriceGun));
        ColorSave(0, miniPanelIndicatorGun);
        CheckButtonState(0, upgradeGunButton);
        SavePrice(0, textPriceGun);

        upgradeGarpunButton.onClick.AddListener(() => UpgradeWeapon(1, upgradeGarpunButton, miniPanelIndicatorGarpun, textPriceGarpun));
        ColorSave(1, miniPanelIndicatorGarpun);
        CheckButtonState(1, upgradeGarpunButton);
        SavePrice(1, textPriceGarpun);
    
    }
    private void Update()
    {
        CheckButtonState(1, upgradeGarpunButton);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           
            exitGamePanel.SetActive(true);
            exitGameButton.onClick.AddListener(() => { Application.Quit(); });
            continueButton.onClick.AddListener(() => {
                exitGamePanel.SetActive(false);
                Time.timeScale = 1f;
            });

        }
    }   

    
   private void SellCap()
    {
        GameData data = SaveManager.Instance.LoadData();
        if (data.caps[0]==1)
        {
            audioSource.PlayOneShot(buySound);
            data.caps[0] -= 1;
        data.coins += 2000;
        }
        else { chooseCapButton.interactable = false; }
        SaveManager.Instance.SaveData(data);
        searcher.UpdateCoinUI();
    }
    private void BuyItem(string nameList, int index, int price, Button button)
    {
        GameData data = SaveManager.Instance.LoadData();
        List<int> itemList = nameList switch
        {
            "upgradeWeapons" => data.upgradeWeapons,
            "caps" => data.caps,
            _=>null
        };

        if (itemList[index] == 1 || data.coins < price)
        {
            button.interactable = false;
        }
        else
        {
            audioSource.PlayOneShot(buySound);
            data.coins -= price;
            itemList[index] = 1;
            SaveManager.Instance.SaveData(data);
            searcher.UpdateCoinUI();
        }
    }


    private void ColorSave(int index, List<GameObject> indicator)
    {
        GameData data = SaveManager.Instance.LoadData();
        if (data.upgradeWeapons[index] >= 2)
        {
            Image indicatorLevel = indicator[0].GetComponent<Image>();
            indicatorLevel.color = color;
        }
        if (data.upgradeWeapons[index] >= 3)
        {
            Image indicatorLevel = indicator[1].GetComponent<Image>();
            indicatorLevel.color = color;
        }

    }

    private void UpgradeWeapon(int weaponIndex, Button button, List<GameObject> indicator, Text priceText)
    {
        GameData data = SaveManager.Instance.LoadData();

        int upgradeCost = 0;
        switch (data.upgradeWeapons[weaponIndex])
        {
            case 1:
                upgradeCost = int.Parse(priceText.text);

                break;
            case 2:
                upgradeCost = int.Parse(priceText.text);
                break;
        }

        if (data.coins >= upgradeCost && data.upgradeWeapons[weaponIndex] > 0 && data.upgradeWeapons[weaponIndex] < 3)
        {
            audioSource.PlayOneShot(buySound);
            data.coins -= upgradeCost;
            data.upgradeWeapons[weaponIndex] += 1;
            switch (data.upgradeWeapons[weaponIndex])
            {
                case 2:
                    ChangeColorIndicator(0, indicator);
                    priceText.text = (upgradeCost * 2).ToString();

                    break;
                case 3:
                    ChangeColorIndicator(1, indicator);
                    button.interactable = false;
                    priceText.text = "MAX";

                    break;
            }
            data.price[weaponIndex] = priceText.text;
            SaveManager.Instance.SaveData(data);
            searcher.UpdateCoinUI();
        }
        else
        {
            button.interactable = false;
        }
    }
    private void SavePrice(int index, Text textPrice)
    {
        GameData data = SaveManager.Instance.LoadData();
        textPrice.text = data.price[index].ToString();
    }

    private void CheckButtonState(int weaponIndex, Button button)
    {
        GameData data = SaveManager.Instance.LoadData();

        if (data.upgradeWeapons[weaponIndex] == 0 || data.upgradeWeapons[weaponIndex] == 3)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }


    private void ChangeColorIndicator(int index, List<GameObject> indicator)
    {
        Image indicatorLevel = indicator[index].GetComponent<Image>();
        indicatorLevel.color = color;
    }

}
