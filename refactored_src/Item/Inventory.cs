using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品栏系统 - 管理玩家的装备、背包和仓库
/// 实现物品的装备、使用、合成和存档功能
/// 支持装备效果、冷却时间和技能解锁
/// </summary>
public class Inventory : MonoBehaviour, ISaveManager, IInventory
{
    private IAudioManager audioManager;

    public List<ItemData> startingEquipent;

    public List<InventoryItem> equipment;                  // 已装备物品列表
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary; // 装备字典

    public List<InventoryItem> inventory;                  // 背包物品列表
    public Dictionary<ItemData, InventoryItem> inventoryDictionary; // 背包字典

    public List<InventoryItem> stash;                     // 仓库物品列表
    public Dictionary<ItemData, InventoryItem> stashDictionary; // 仓库字典

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent; // 背包槽位父对象
    [SerializeField] private Transform stashSlotParent;     // 仓库槽位父对象
    [SerializeField] private Transform equipmentSlotParent; // 装备槽位父对象
    [SerializeField] private Transform statSlotParent;      // 属性槽位父对象

    private UI_ItemSlot[] inventoryItemSlot;               // 背包槽位数组
    private UI_ItemSlot[] stashItemSlot;                   // 仓库槽位数组
    private UI_EquipmentSlot[] equipmentSlot;               // 装备槽位数组
    private UI_StatSlot[] statSlot;                         // 属性槽位数组


    [Header("Database")]
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private GameEventBus eventBus;

    private void Start()
    {
        // 获取服务依赖
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 初始化字典和列表
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        // 获取UI组件
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartItem();
    }

    /// <summary>
    /// 添加初始物品
    /// </summary>
    private void AddStartItem()
    {
        // 装备加载的装备
        foreach (ItemData_Equipment item in loadedEquipment)
            EquipItem(item);

        // 如果有加载的物品，则添加到背包
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

        // 否则添加初始装备
        for (int i = 0; i < startingEquipent.Count; i++)
            AddItem(startingEquipent[i]);
    }

    /// <summary>
    /// 装备物品
    /// </summary>
    /// <param name="_item">要装备的物品</param>
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment itemToRemove = null;

        // 查找同类型装备
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            if (item.Key.equipmentType == newEquipment.equipmentType)
                itemToRemove = item.Key;

        // 如果有同类型装备，先卸装
        if (itemToRemove != null)
        {
            UnequipItem(itemToRemove);
            AddItem(itemToRemove);
        }

        // 装备新物品
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);

        // ========== 发布装备变更事件到事件总线（Observer Pattern） ==========
        eventBus?.Publish(new EquipmentChangedEvent
        {
            EquipmentType = newEquipment.equipmentType,
            Equipment = newEquipment,
            IsEquipped = true
        });

        UpdateSlotUI();
    }

    /// <summary>
    /// 卸装物品
    /// </summary>
    /// <param name="itemToRemove">要卸装的物品</param>
    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();

            // ========== 发布装备变更事件到事件总线（Observer Pattern） ==========
            eventBus?.Publish(new EquipmentChangedEvent
            {
                EquipmentType = itemToRemove.equipmentType,
                Equipment = itemToRemove,
                IsEquipped = false
            });
        }
    }

    /// <summary>
    /// 更新槽位UI显示
    /// </summary>
    public void UpdateSlotUI()
    {
        // 清空所有槽位
        for (int i = 0; i < inventoryItemSlot.Length; i++)
            inventoryItemSlot[i].ClearUpSlot();
        for (int i = 0; i < stashItemSlot.Length; i++)
            stashItemSlot[i].ClearUpSlot();

        // 更新装备槽位
        for (int i = 0; i < equipmentSlot.Length; i++)
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
                if (item.Key.equipmentType == equipmentSlot[i].slotType)
                    equipmentSlot[i].UpdateSlot(item.Value);

        // 更新背包和仓库槽位
        for (int i = 0; i < inventory.Count; i++)
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        for (int i = 0; i < stash.Count; i++)
            stashItemSlot[i].UpdateSlot(stash[i]);
        for (int i = 0; i < statSlot.Length; i++)
            statSlot[i].UpdateStatValueUI();
    }

    /// <summary>
    /// 添加物品到物品栏
    /// </summary>
    /// <param name="_item">要添加的物品</param>
    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItemToInventory())
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);

        UpdateSlotUI();
    }

    /// <summary>
    /// 添加物品到仓库
    /// </summary>
    /// <param name="_item">要添加的物品</param>
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

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="_item">要添加的物品</param>
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

    /// <summary>
    /// 移除物品
    /// </summary>
    /// <param name="_item">要移除的物品</param>
    public void RemoveItem(ItemData _item)
    {
        // 从背包中移除
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

        // 从仓库中移除
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

    /// <summary>
    /// 检查背包是否有空间
    /// </summary>
    /// <returns>是否有空间</returns>
    public bool CanAddItemToInventory()
    {
        if (inventory.Count >= inventoryItemSlot.Length)
            return false;

        return true;
    }

    /// <summary>
    /// 检查是否可以合成物品
    /// </summary>
    /// <param name="_itemToCraft">要合成的物品</param>
    /// <param name="_requiredMaterials">所需材料列表</param>
    /// <returns>是否可以合成</returns>
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        // 检查是否有足够的材料
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    audioManager.PlaySFX(29); // 合成失败音效
                    return false;
                }
                else
                {
                    materialsToRemove.Add(_requiredMaterials[i]);
                }
            }
            else
            {
                audioManager.PlaySFX(29); // 合成失败音效
                return false;
            }
        }

        // 消耗材料并添加合成物品
        for (int i = 0; i < materialsToRemove.Count; i++)
            for (int j = 0; j < materialsToRemove[i].stackSize; j++)
                RemoveItem(materialsToRemove[i].data);
        AddItem(_itemToCraft);

        audioManager.PlaySFX(28); // 合成成功音效

        return true;
    }

    /// <summary>
    /// 获取装备列表
    /// </summary>
    /// <returns>装备列表</returns>
    public List<InventoryItem> GetEquipmentList() => equipment;

    /// <summary>
    /// 根据类型获取装备
    /// </summary>
    /// <param name="type">装备类型</param>
    /// <returns>装备数据</returns>
    public ItemData_Equipment GetEquipment(EquipmentType type)
    {
        ItemData_Equipment equipedItem = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            if (item.Key.equipmentType == type)
                equipedItem = item.Key;

        return equipedItem;
    }

    /// <summary>
    /// 加载存档数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void LoadData(GameData data)
    {
        // 加载背包和仓库物品
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

        // 加载装备
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

    /// <summary>
    /// 保存存档数据
    /// </summary>
    /// <param name="data">游戏数据</param>
    public void SaveData(ref GameData data)
    {
        data.inventory.Clear();

        // 保存背包物品
        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
            data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);

        // 保存仓库物品
        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
            data.inventory.Add(pair.Key.itemId, pair.Value.stackSize);

        // 确保装备列表反映当前状态而不是在多次保存中累积
        data.equipmentId.Clear();
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
            data.equipmentId.Add(pair.Key.itemId);
    }

    /// <summary>
    /// 获取物品数据库
    /// </summary>
    /// <returns>物品数据列表</returns>
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
}
