using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingEquipent;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;
    private UI_StatSlot[] statSlot;

    [Header("Items cooldown")]
    private float lastTimeUseWeapon;
    private float lastTimeUseArmor;
    private float lastTimeUseAmulet;
    private float lastTimeUseFlask;

    [Header("Use amulet")]
    public bool dashUseAmulet;
    public bool jumpUseAmulet;
    public bool swordUseAmulet;

    public event Action OnWeaponEquiped;
    public event Action OnArmorEquiped;
    public event Action OnAmuletEquiped;
    public event Action OnFlaskEquiped;

    public event Action OnWeaponUnequiped;
    public event Action OnArmorUnequiped;
    public event Action OnAmuletUnequiped;
    public event Action OnFlaskUnequiped;

    public event Action OnWeaponUsed;
    public event Action OnArmorUsed;
    public event Action OnAmuletUsed;
    public event Action OnFlaskUsed;

    [SerializeField] private UI_SkillTreeSlot dashUseAmuletUnlockButton;
    [SerializeField] private UI_SkillTreeSlot jumpUseAmuletUnlockButton;
    [SerializeField] private UI_SkillTreeSlot swordUseAmuletUnlockButton;

    [Header("Database")]
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartItem();

        lastTimeUseWeapon = -100;
        lastTimeUseArmor = -100;
        lastTimeUseAmulet = -100;
        lastTimeUseFlask = -100;

        dashUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDashUseAmulet);
        jumpUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockJumpUseAmulet);
        swordUseAmuletUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordUseAmulet);

        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }

    private void AddStartItem()
    {
        foreach (ItemData_Equipment item in loadedEquipment)
            EquipItem(item);

        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }

            return;
        }

        for (int i = 0; i < startingEquipent.Count; i++)
            AddItem(startingEquipent[i]);
    }

    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment itemToRemove = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            if (item.Key.equipmentType == newEquipment.equipmentType)
                itemToRemove = item.Key;

        if (itemToRemove != null)
        {
            UnequipItem(itemToRemove);
            AddItem(itemToRemove);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);

        switch (newEquipment.equipmentType)
        {
            case EquipmentType.Weapon:
                OnWeaponEquiped?.Invoke();
                lastTimeUseWeapon = -100;
                break;
            case EquipmentType.Armor:
                OnArmorEquiped?.Invoke();
                lastTimeUseArmor = -100;
                break;
            case EquipmentType.Amulet:
                OnAmuletEquiped?.Invoke();
                lastTimeUseAmulet = -100;
                break;
            case EquipmentType.Flask:
                OnFlaskEquiped?.Invoke();
                lastTimeUseFlask = -100;
                break;
        }

        UpdateSlotUI();
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();

            switch (itemToRemove.equipmentType)
            {
                case EquipmentType.Weapon:
                    OnWeaponUnequiped?.Invoke();
                    break;
                case EquipmentType.Armor:
                    OnArmorUnequiped?.Invoke();
                    break;
                case EquipmentType.Amulet:
                    OnAmuletUnequiped?.Invoke();
                    break;
                case EquipmentType.Flask:
                    OnFlaskUnequiped?.Invoke();
                    break;
            }
        }
    }

    public void UpdateSlotUI()
    {
        for (int i = 0; i < inventoryItemSlot.Length; i++)
            inventoryItemSlot[i].ClearUpSlot();
        for (int i = 0; i < stashItemSlot.Length; i++)
            stashItemSlot[i].ClearUpSlot();

        for (int i = 0; i < equipmentSlot.Length; i++)
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);

        for (int i = 0; i < inventory.Count; i++)
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        for (int i = 0; i < stash.Count; i++)
            stashItemSlot[i].UpdateSlot(stash[i]);
        for (int i = 0; i < statSlot.Length; i++)
            statSlot[i].UpdateStatValueUI();
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItemToInventory())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
            value.AddStack();
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem inventoryValue))
        {
            if (inventoryValue.stackSize <= 1)
            {
                inventory.Remove(inventoryValue);
                inventoryDictionary.Remove(_item);
            }
            else
                inventoryValue.RemoveStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);
            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();
    }

    public bool CanAddItemToInventory()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
            return false;

        return true;
    }

    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    AudioManager.instance.PlaySFX(29);
                    return false;
                }
                else
                {
                    materialsToRemove.Add(_requiredMaterials[i]);
                }
            }
            else
            {
                AudioManager.instance.PlaySFX(29);
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)
            for (int j = 0; j < materialsToRemove[i].stackSize; j++)
                RemoveItem(materialsToRemove[i].data);
        AddItem(_itemToCraft);

        AudioManager.instance.PlaySFX(28);

        return true;
    }

    public List<InventoryItem> GetEquipmentList() => equipment;

    public ItemData_Equipment GetEquipment(EquipmentType type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            if (item.Key.equipmentType == type)
                equipedItem = item.Key;

        return equipedItem;
    }

    public bool CanUseWeapon()
    {
        ItemData_Equipment currentWeapon = GetEquipment(EquipmentType.Weapon);

        if (currentWeapon == null)
            return false;

        return Time.time > lastTimeUseWeapon + currentWeapon.itemCooldown;
    }

    public void ConsumeWeaponCooldown()
    {
        ItemData_Equipment currentWeapon = GetEquipment(EquipmentType.Weapon);
        if (currentWeapon != null)
        {
            lastTimeUseWeapon = Time.time;
            OnWeaponUsed?.Invoke();
        }
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);

        if ((currentArmor == null))
            return false;

        bool canUseArmor = Time.time > lastTimeUseArmor + currentArmor.itemCooldown;
        if (canUseArmor)
        {
            lastTimeUseArmor = Time.time;
            OnArmorUsed?.Invoke();
        }

        return canUseArmor;
    }

    public bool CanUseAmulet()
    {
        ItemData_Equipment currentAmulet = GetEquipment(EquipmentType.Amulet);

        if ((currentAmulet == null))
            return false;

        bool canUseAmulet = Time.time > lastTimeUseAmulet + currentAmulet.itemCooldown;
        if (canUseAmulet)
        {
            lastTimeUseAmulet = Time.time;
            OnAmuletUsed?.Invoke();
        }

        return canUseAmulet;
    }

    public bool CanUseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);

        if ((currentFlask == null))
            return false;

        bool canUseFlask = Time.time > lastTimeUseFlask + currentFlask.itemCooldown;
        if (canUseFlask)
        {
            lastTimeUseFlask = Time.time;
            OnFlaskUsed?.Invoke();
            AudioManager.instance.PlaySFX(38);
        }

        return canUseFlask;
    }

    public void UnlockDashUseAmulet()
    {
        if (dashUseAmuletUnlockButton.CanUnlockSkillSlot() && dashUseAmuletUnlockButton.unlocked)
        {
            dashUseAmulet = true;
        }
    }

    public void UnlockJumpUseAmulet()
    {
        if (jumpUseAmuletUnlockButton.CanUnlockSkillSlot() && jumpUseAmuletUnlockButton.unlocked)
        {
            jumpUseAmulet = true;
        }
    }

    public void UnlockSwordUseAmulet()
    {
        if (swordUseAmuletUnlockButton.CanUnlockSkillSlot() && swordUseAmuletUnlockButton.unlocked)
        {
            swordUseAmulet = true;
        }
    }

    public void LoadData(GameData data)
    {
        foreach (KeyValuePair<string, int> pair in data.inventory)
        {
            foreach (var item in GetItemDatabase())
            {
                if (item != null && item.itemId == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        foreach (string loadedItemId in data.equipmentId)
        {
            foreach (var item in GetItemDatabase())
            {
                if (item != null && item.itemId == loadedItemId)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.inventory.Clear();

        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
            data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);

        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
            data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);

        // Ensure equipment list reflects current state instead of accumulating across saves
        data.equipmentId.Clear();
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
            data.equipmentId.Add(pair.Key.itemId);
    }

    private List<ItemData> GetItemDatabase()
    {
        List<ItemData> itemDatabase = new List<ItemData>();

        // 查找指定文件夹中的所有资源
        string[] assetNames = AssetDatabase.FindAssets("", new[] { "Assets/Data/Items" });

        foreach (string SOName in assetNames)
        {
            // GUID转路径
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            // 加载资源
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDatabase.Add(itemData);
        }

        return itemDatabase;
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;

        // 根据技能槽的解锁状态初始化护身符使用状态
        dashUseAmulet = dashUseAmuletUnlockButton.unlocked;
        jumpUseAmulet = jumpUseAmuletUnlockButton.unlocked;
        swordUseAmulet = swordUseAmuletUnlockButton.unlocked;
    }
}
