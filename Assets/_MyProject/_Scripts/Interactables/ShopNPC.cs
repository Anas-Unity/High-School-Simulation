using UnityEngine;

public class ShopNPC : MonoBehaviour, IInteractable
{
    public ShopInventory shopInventory;

    public void Interact()
    {
        ShopUIManager.Instance.OpenShop(shopInventory);
    }
}
