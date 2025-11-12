using UnityEngine;

public class ShopNPC : MonoBehaviour//, Interactable
{
    public ShopInventory shopInventory;

    public void Interact()
    {
        ShopUIManager.Instance.OpenShop(shopInventory);
    }
}
