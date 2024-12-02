using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Button shopButton;
    public Button closeShopPanel;

    public Button buyGarpun;

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
    void Start()
    {
        shopButton.onClick.AddListener(() => shopPanel.SetActive(true));
        closeShopPanel.onClick.AddListener(() => shopPanel.SetActive(false));

        gunShopPanelButton.onClick.AddListener(() =>
        {
            shopGunPanel.SetActive(true);
            shopPanel.SetActive(false);
        });
        closeGunShopPanel.onClick.AddListener(() => shopGunPanel.SetActive(false));

        upgradeShopButtonOpen.onClick.AddListener(() =>
        {
            upgradeShopPanel.SetActive(true);
            shopPanel.SetActive(false);
        });
        closeUpgradePanel.onClick.AddListener(() => upgradeShopPanel.SetActive(false));

        buyGarpun.onClick.AddListener(() => BuyWeapon(1, 1500, buyGarpun));

        openAdditionalPanelButton.onClick.AddListener(() =>
         {
             additionalShopPanel.SetActive(true);
             shopPanel.SetActive(false);
         });
        closeAdditionalPanelButton.onClick.AddListener(() => additionalShopPanel.SetActive(false));



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
    }

    private void BuyWeapon(int index, int price, Button button)
    {
        GameData data = SaveManager.Instance.LoadData();
        if (data.upgradeWeapons[index] == 1 || data.coins < price)
        {
            button.interactable = false;
        }
        else
        {
            data.coins -= price;
            data.upgradeWeapons[index] = 1;
            SaveManager.Instance.SaveData(data);
            searcher.UpdateCoinUI();
        }
    }
    private void BuyCaps(int index, int price, Button button) //Объеденить два метода сделат универсальный
    {
        GameData data = SaveManager.Instance.LoadData();
        if (data.caps[index] == 1 || data.coins < price)
        {
            button.interactable = false;
        }
        else
        {
            data.coins -= price;
            data.caps[index] = 1;
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
