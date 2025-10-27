using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;
    public GameObject shopPanel;
    public Transform itemContainer;
    public GameObject itemUIPrefab;
    private ShopInventory currentInventory;

    void Awake()
    {
        Instance = this;
        shopPanel.SetActive(false);
    }

    public void OpenShop(ShopInventory inventory)
    {
        currentInventory = inventory;
        shopPanel.SetActive(true);
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);

        foreach (ShopItem item in currentInventory.items)
        {
            GameObject obj = Instantiate(itemUIPrefab, itemContainer);
            obj.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            obj.transform.Find("Price").GetComponent<TMP_Text>().text = item.price.ToString();
            obj.transform.Find("Icon").GetComponent<Image>().sprite = item.itemIcon;

            Button buyButton = obj.transform.Find("BuyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    public void BuyItem(ShopItem item)
    {
        if (GameManager.Instance.coins >= item.price)
        {
            GameManager.Instance.SpendCoins(item.price);
            Debug.Log($"Bought {item.itemName}");
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }
}
