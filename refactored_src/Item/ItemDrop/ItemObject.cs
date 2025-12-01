using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Vector2 velocity;

    private IAudioManager audioManager;
    private IInventory inventory;

    private void Awake()
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        inventory = ServiceLocator.Instance.Get<IInventory>();
    }

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
        if (!inventory.CanAddItemToInventory() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            ServiceLocator.Instance.Get<IPlayerManager>().Player.fx.CreatePopUpText("背包已满");
            return;
        }

        audioManager.PlaySFX(32);

        inventory.AddItem(itemData);
        Destroy(gameObject);
    }
}
