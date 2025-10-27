using UnityEngine;

[CreateAssetMenu(fileName = "NewShopInventory", menuName = "Shop/Inventory")]
public class ShopInventory : ScriptableObject
{
    public ShopItem[] items;
}
