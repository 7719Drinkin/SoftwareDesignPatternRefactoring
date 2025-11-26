using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Vector2 velocity;

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object -" + itemData.name;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public void PickUpItem()
    {
        if (!Inventory.instance.CanAddItemToInventory() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("背包已满");
            return;
        }

        AudioManager.instance.PlaySFX(32);

        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
