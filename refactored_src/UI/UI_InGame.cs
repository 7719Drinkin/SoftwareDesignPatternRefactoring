using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏内UI管理器 - 管理所有游戏内UI元素的显示和更新
/// 包括生命值、经验值、货币、技能冷却和装备状态等
/// </summary>
public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;     // 玩家属性引用
    private IPlayerManager playerManager;                  // 玩家管理器引用
    [SerializeField] private Slider slider;                // 生命值滑块
    [SerializeField] private TextMeshProUGUI healthText;   // 生命值文本
    [SerializeField] private TextMeshProUGUI currencyText;  // 货币文本
    [SerializeField] private TextMeshProUGUI levelText;     // 内圈等级文本
    [SerializeField] private Image experienceRing;         // 外圈经验环
    [SerializeField] private float currencyIncreasePerSecond = 300f; // 显示用的每秒增加速度
    private int displayedCurrency;                         // 当前UI显示的金币数量

    // 技能图标
    [SerializeField] private Image dashImage;              // 冲刺技能图标
    [SerializeField] private Image cloneOnDashImage;      // 冲刺分身技能图标
    [SerializeField] private Image dashAttackImage;       // 冲刺攻击技能图标
    [SerializeField] private Image parryImage;             // 格挡技能图标
    [SerializeField] private Image assassinateImage;       // 暗杀技能图标
    [SerializeField] private Image multiCrystalImage;      // 多重水晶技能图标
    [SerializeField] private Image blackholeImage;         // 黑洞技能图标

    // 装备图标
    [SerializeField] private Image weaponImage;            // 武器图标
    [SerializeField] private Image armorImage;              // 护甲图标
    [SerializeField] private Image amuletImage;            // 护身符图标
    [SerializeField] private Image flaskImage;             // 药水图标

    // 技能解锁状态
    private bool dashUnlock;                              // 冲刺技能是否解锁
    private bool cloneOnDashUnlock;                       // 冲刺分身技能是否解锁
    private bool dashAttackUnlock;                        // 冲刺攻击技能是否解锁
    private bool parryUnlock;                             // 格挡技能是否解锁
    private bool assassinateUnlock;                       // 暗杀技能是否解锁
    private bool multiCrystalUnlock;                      // 多重水晶技能是否解锁
    private bool blackholeUnlock;                         // 黑洞技能是否解锁

    // 装备状态
    private bool weaponEquiped;                           // 武器是否装备
    private bool armorEquiped;                            // 护甲是否装备
    private bool amuletEquiped;                           // 护身符是否装备
    private bool flaskEquiped;                            // 药水是否装备

    private ISkillManager skills;                         // 技能管理器引用
    private IInventory inventory;                         // 物品栏引用
    private GameEventBus eventBus;                        // 事件总线引用

    /// <summary>
    /// 初始化UI系统
    /// </summary>
    void Start()
    {
        // 初始化服务依赖
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        skills = ServiceLocator.Instance.Get<ISkillManager>();
        inventory = ServiceLocator.Instance.Get<IInventory>();
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();

        // 初始化货币显示
        if (playerManager != null)
        {
            displayedCurrency = playerManager.Currency;
            if (currencyText != null)
                currencyText.text = displayedCurrency.ToString();
        }

        // ========== 订阅事件总线（Observer Pattern） ==========
        SubscribeToEvents();

        // 播放背景音乐
        var audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        if (audioManager != null)
            audioManager.PlayBGM(1);
    }

    /// <summary>
    /// 订阅游戏事件 - UI 作为 Observer（观察者）
    /// 替代原来的直接事件订阅，解耦 UI 与技能/装备系统
    /// </summary>
    private void SubscribeToEvents()
    {
        if (eventBus == null)
            return;

        // ========== 订阅技能使用事件（替代 skills.Dash.OnDashUsed 等） ==========
        eventBus.Subscribe<SkillUsedEvent>(OnSkillUsedViaEventBus);

        // ========== 订阅技能解锁事件（替代 skills.Dash.OnDashUnlocked 等） ==========
        eventBus.Subscribe<SkillUnlockedEvent>(OnSkillUnlockedViaEventBus);

        // ========== 订阅装备变更事件（替代 inventory.OnWeaponEquiped 等） ==========
        eventBus.Subscribe<EquipmentChangedEvent>(OnEquipmentChangedViaEventBus);

        // ========== 订阅装备使用事件（替代 inventory.OnWeaponUsed 等） ==========
        eventBus.Subscribe<EquipmentUsedEvent>(OnEquipmentUsedViaEventBus);
    }

    /// <summary>
    /// 取消订阅事件（防止内存泄漏）
    /// </summary>
    private void OnDestroy()
    {
        if (eventBus != null)
        {
            eventBus.Unsubscribe<SkillUsedEvent>(OnSkillUsedViaEventBus);
            eventBus.Unsubscribe<SkillUnlockedEvent>(OnSkillUnlockedViaEventBus);
            eventBus.Unsubscribe<EquipmentChangedEvent>(OnEquipmentChangedViaEventBus);
            eventBus.Unsubscribe<EquipmentUsedEvent>(OnEquipmentUsedViaEventBus);
        }
    }

    /// <summary>
    /// 通过事件总线处理技能使用 - Observer 响应（替代直接订阅）
    /// </summary>
    private void OnSkillUsedViaEventBus(SkillUsedEvent evt)
    {
        // 根据技能名称调用对应的UI更新方法
        switch (evt.SkillName)
        {
            case "Dash":
                UseDash();
                break;
            case "Crystal":
                UseMultiCrystal();
                break;
            case "Assassinate":
                UseAssassinate();
                break;
            case "DashAttack":
                UseDashAttack();
                break;
            case "Blackhole":
                UseBlackhole();
                break;
            case "CloneOnDash":
                UseCloneOnDash();
                break;
            case "Parry":
                UseParry();
                break;
        }
    }

    /// <summary>
    /// 通过事件总线处理技能解锁 - Observer 响应（替代直接订阅）
    /// </summary>
    private void OnSkillUnlockedViaEventBus(SkillUnlockedEvent evt)
    {
        // 根据技能名称调用对应的UI解锁方法
        switch (evt.SkillName)
        {
            case "Dash":
                UnlockDash();
                break;
            case "CloneOnDash":
                UnlockCloneOnDash();
                break;
            case "DashAttack":
                UnlockDashAttack();
                break;
            case "Parry":
                UnlockParry();
                break;
            case "Assassinate":
                UnlockAsssassinate();
                break;
            case "MultiCrystal":
                UnlockMultiCrystal();
                break;
            case "Blackhole":
                UnlockBlackhole();
                break;
        }
    }

    /// <summary>
    /// 通过事件总线处理装备变更 - Observer 响应（替代直接订阅）
    /// </summary>
    private void OnEquipmentChangedViaEventBus(EquipmentChangedEvent evt)
    {
        if (evt.IsEquipped)
        {
            // 装备事件
            switch (evt.EquipmentType)
            {
                case EquipmentType.Weapon:
                    EquipWeapon();
                    break;
                case EquipmentType.Armor:
                    EquipArmor();
                    break;
                case EquipmentType.Amulet:
                    EquipAmulet();
                    break;
                case EquipmentType.Flask:
                    EquipFlask();
                    break;
            }
        }
        else
        {
            // 卸装事件
            switch (evt.EquipmentType)
            {
                case EquipmentType.Weapon:
                    UnequipWeapon();
                    break;
                case EquipmentType.Armor:
                    UnequipedArmor();
                    break;
                case EquipmentType.Amulet:
                    UnequipedAmulet();
                    break;
                case EquipmentType.Flask:
                    UnEquipedFlask();
                    break;
            }
        }
    }

    /// <summary>
    /// 通过事件总线处理装备使用 - Observer 响应（替代直接订阅）
    /// </summary>
    private void OnEquipmentUsedViaEventBus(EquipmentUsedEvent evt)
    {
        // 调用对应的装备使用UI更新
        switch (evt.EquipmentType)
        {
            case EquipmentType.Weapon:
                UseWeapon();
                break;
            case EquipmentType.Armor:
                UseArmor();
                break;
            case EquipmentType.Amulet:
                UseAmulet();
                break;
            case EquipmentType.Flask:
                UseFlask();
                break;
        }
    }

    /// <summary>
    /// 更新生命值UI
    /// </summary>
    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = Mathf.RoundToInt(playerStats.currentHealth);

        if (healthText != null)
        {
            int currentHealth = Mathf.Clamp(playerStats.currentHealth, 0, playerStats.GetMaxHealthValue());
            string text = $"{currentHealth}/{playerStats.GetMaxHealthValue()}";
            healthText.text = text;
        }
    }

    /// <summary>
    /// 更新货币UI - 使用平滑动画效果
    /// </summary>
    private void UpdateCurrencyUI()
    {
        if (currencyText == null)
            return;

        int target = playerManager.Currency;
        if (displayedCurrency == target)
            return;

        int delta = Mathf.CeilToInt(currencyIncreasePerSecond * Time.deltaTime);
        if (delta <= 0) delta = 1;

        if (displayedCurrency < target)
            displayedCurrency = Mathf.Min(displayedCurrency + delta, target);
        else
            displayedCurrency = Mathf.Max(displayedCurrency - delta, target);

        currencyText.text = displayedCurrency.ToString();
    }

    /// <summary>
    /// 更新经验值UI - 核心经验值显示逻辑
    /// </summary>
    private void UpdateExperienceUI()
    {
        // 更新等级文本显示
        if (levelText != null)
            levelText.text = playerManager.PlayerLevel.ToString();

        // 更新经验环填充度
        if (experienceRing != null)
        {
            // 调试信息
            if (playerManager == null)
            {
                return;
            }

            int currentExp = playerManager.CurrentExperience;
            int currentLevel = playerManager.PlayerLevel;

            float requiredExp = 200 * (currentLevel / 5 + 1);
            float fill = Mathf.Clamp01(currentExp / requiredExp);

            experienceRing.fillAmount = fill;
        }
    }

    /// <summary>
    /// 每帧更新UI状态
    /// </summary>
    private void Update()
    {
        // 延迟初始化
        if (playerManager == null)
            playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        if (skills == null)
            skills = ServiceLocator.Instance.Get<ISkillManager>();
        if (inventory == null)
            inventory = ServiceLocator.Instance.Get<IInventory>();

        UpdateHealthUI();
        UpdateCurrencyUI();
        UpdateExperienceUI();

        // 更新技能冷却显示
        if (skills != null)
        {
            if (dashUnlock)
                CheckCooldownOf(dashImage, skills.Dash.cooldown);
            if (cloneOnDashUnlock)
                CheckCooldownOf(cloneOnDashImage, skills.Clone.cooldown);
            if (dashAttackUnlock)
                CheckCooldownOf(dashAttackImage, skills.DashAttack.cooldown);
            if (parryUnlock)
                CheckCooldownOf(parryImage, skills.Parry.cooldown);
            if (assassinateUnlock)
                CheckCooldownOf(assassinateImage, skills.Assassinate.cooldown);
            if (multiCrystalUnlock)
                CheckCooldownOf(multiCrystalImage, skills.Crystal.multiStackCooldown);
            if (blackholeUnlock)
                CheckCooldownOf(blackholeImage, skills.Blackhole.cooldown);
        }

        // 更新装备冷却显示
        if (inventory != null)
        {
            if (weaponEquiped)
                CheckCooldownOf(weaponImage, inventory.GetEquipment(EquipmentType.Weapon).itemCooldown);
            if (armorEquiped)
                CheckCooldownOf(armorImage, inventory.GetEquipment(EquipmentType.Armor).itemCooldown);
            if (amuletEquiped)
                CheckCooldownOf(amuletImage, inventory.GetEquipment(EquipmentType.Amulet).itemCooldown);
            if (flaskEquiped)
                CheckCooldownOf(flaskImage, inventory.GetEquipment(EquipmentType.Flask).itemCooldown);
        }
    }

    /// <summary>
    /// 检查并更新技能/装备的冷却时间显示
    /// </summary>
    /// <param name="image">要更新的图标</param>
    /// <param name="cooldown">冷却时间</param>
    private void CheckCooldownOf(Image image, float cooldown)
    {
        if (image.fillAmount > 0)
            image.fillAmount -= 1 / cooldown * Time.deltaTime;
    }

    /// <summary>
    /// 设置技能/装备的冷却状态
    /// </summary>
    /// <param name="image">要设置的图标</param>
    private void SetCooldownOf(Image image) => image.fillAmount = 1;

    #region 技能解锁事件处理
    /// <summary>
    /// 解锁冲刺技能
    /// </summary>
    private void UnlockDash()
    {
        dashUnlock = true;
        dashImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁冲刺分身技能
    /// </summary>
    private void UnlockCloneOnDash()
    {
        cloneOnDashUnlock = true;
        cloneOnDashImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁冲刺攻击技能
    /// </summary>
    private void UnlockDashAttack()
    {
        dashAttackUnlock = true;
        dashAttackImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁格挡技能
    /// </summary>
    private void UnlockParry()
    {
        parryUnlock = true;
        parryImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁暗杀技能
    /// </summary>
    private void UnlockAsssassinate()
    {
        assassinateUnlock = true;
        assassinateImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁多重水晶技能
    /// </summary>
    private void UnlockMultiCrystal()
    {
        multiCrystalUnlock = true;
        multiCrystalImage.fillAmount = 0;
    }

    /// <summary>
    /// 解锁黑洞技能
    /// </summary>
    private void UnlockBlackhole()
    {
        blackholeUnlock = true;
        blackholeImage.fillAmount = 0;
    }
    #endregion

    #region 装备事件处理
    /// <summary>
    /// 装备武器
    /// </summary>
    private void EquipWeapon()
    {
        weaponEquiped = true;
        weaponImage.fillAmount = 0;
    }

    /// <summary>
    /// 装备护甲
    /// </summary>
    private void EquipArmor()
    {
        armorEquiped = true;
        armorImage.fillAmount = 0;
    }

    /// <summary>
    /// 装备护身符
    /// </summary>
    private void EquipAmulet()
    {
        amuletEquiped = true;
        amuletImage.fillAmount = 0;
    }

    /// <summary>
    /// 装备药水
    /// </summary>
    private void EquipFlask()
    {
        flaskEquiped = true;
        flaskImage.fillAmount = 0;
    }

    /// <summary>
    /// 卸下武器
    /// </summary>
    private void UnequipWeapon()
    {
        weaponEquiped = false;
        weaponImage.fillAmount = 1;
    }

    /// <summary>
    /// 卸下护甲
    /// </summary>
    private void UnequipedArmor()
    {
        armorEquiped = false;
        armorImage.fillAmount = 1;
    }

    /// <summary>
    /// 卸下护身符
    /// </summary>
    private void UnequipedAmulet()
    {
        amuletEquiped = false;
        amuletImage.fillAmount = 1;
    }

    /// <summary>
    /// 卸下药水
    /// </summary>
    private void UnEquipedFlask()
    {
        flaskEquiped = false;
        flaskImage.fillAmount = 1;
    }
    #endregion

    #region 技能使用事件处理
    /// <summary>
    /// 使用冲刺技能
    /// </summary>
    private void UseDash() => SetCooldownOf(dashImage);

    /// <summary>
    /// 使用冲刺分身技能
    /// </summary>
    private void UseCloneOnDash() => SetCooldownOf(cloneOnDashImage);

    /// <summary>
    /// 使用冲刺攻击技能
    /// </summary>
    private void UseDashAttack() => SetCooldownOf(dashAttackImage);

    /// <summary>
    /// 使用格挡技能
    /// </summary>
    private void UseParry() => SetCooldownOf(parryImage);

    /// <summary>
    /// 使用暗杀技能
    /// </summary>
    private void UseAssassinate() => SetCooldownOf(assassinateImage);

    /// <summary>
    /// 使用多重水晶技能
    /// </summary>
    private void UseMultiCrystal() => SetCooldownOf(multiCrystalImage);

    /// <summary>
    /// 使用黑洞技能
    /// </summary>
    private void UseBlackhole() => SetCooldownOf(blackholeImage);

    /// <summary>
    /// 使用武器
    /// </summary>
    private void UseWeapon() => SetCooldownOf(weaponImage);

    /// <summary>
    /// 使用护甲
    /// </summary>
    private void UseArmor() => SetCooldownOf(armorImage);

    /// <summary>
    /// 使用护身符
    /// </summary>
    private void UseAmulet() => SetCooldownOf(amuletImage);

    /// <summary>
    /// 使用药水
    /// </summary>
    private void UseFlask() => SetCooldownOf(flaskImage);
    #endregion
}
