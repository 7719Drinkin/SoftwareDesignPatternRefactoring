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

    private SkillManager skills;                          // 技能管理器引用
    private Inventory inventory;                           // 物品栏引用

    /// <summary>
    /// 初始化UI系统
    /// </summary>
    void Start()
    {
        skills = SkillManager.instance;
        inventory = Inventory.instance;
        displayedCurrency = PlayerManager.instance.currency;
        if (currencyText != null)
            currencyText.text = displayedCurrency.ToString();

        // 绑定技能解锁事件
        skills.dash.OnDashUnlocked += UnlockDash;
        skills.dash.OnCloneOnDashUnlock += UnlockCloneOnDash;
        skills.dash.OnDashAttackUnlock += UnlockDashAttack;
        skills.parry.OnUnlockParry += UnlockParry;
        skills.assassinate.OnAssassinateUnlock += UnlockAsssassinate;
        skills.crystal.OnMultiCrystalUnlock += UnlockMultiCrystal;
        skills.blackhole.OnBlackholeUnlock += UnlockBlackhole;

        // 绑定技能使用事件
        skills.dash.OnDashUsed += UseDash;
        PlayerManager.instance.player.dashState.OnCloneOnDashUsed += UseCloneOnDash;
        skills.dashAttack.OnDashAttackUsed += UseDashAttack;
        PlayerManager.instance.player.counterAttack.OnParryUsed += UseParry;
        skills.assassinate.OnAssassinateUsed += UseAssassinate;
        skills.crystal.OnMultiCrystalUsed += UseMultiCrystal;
        skills.blackhole.OnBlackholeUsed += UseBlackhole;

        // 绑定装备事件
        inventory.OnWeaponEquiped += EquipWeapon;
        inventory.OnWeaponUnequiped += UnequipWeapon;
        inventory.OnWeaponUsed += UseWeapon;

        inventory.OnArmorEquiped += EquipArmor;
        inventory.OnArmorUnequiped += UnequipedArmor;
        inventory.OnArmorUsed += UseArmor;

        inventory.OnAmuletEquiped += EquipAmulet;
        inventory.OnAmuletUnequiped += UnequipedAmulet;
        inventory.OnAmuletUsed += UseAmulet;

        inventory.OnFlaskEquiped += EquipFlask;
        inventory.OnFlaskUnequiped += UnEquipedFlask;
        inventory.OnFlaskUsed += UseFlask;

        AudioManager.instance.PlayBGM(1);
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

        int target = PlayerManager.instance.currency;
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
            levelText.text = PlayerManager.instance.playerLevel.ToString();

        // 更新经验环填充度
        if (experienceRing != null)
        {
            // 调试信息
            if (PlayerManager.instance == null)
            {
                return;
            }

            int currentExp = PlayerManager.instance.currentExperience;
            int currentLevel = PlayerManager.instance.playerLevel;
            
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
        UpdateHealthUI();
        UpdateCurrencyUI();
        UpdateExperienceUI();

        // 更新技能冷却显示
        if (dashUnlock)
            CheckCooldownOf(dashImage, skills.dash.cooldown);
        if (cloneOnDashUnlock)
            CheckCooldownOf(cloneOnDashImage, skills.clone.cooldown);
        if (dashAttackUnlock)
            CheckCooldownOf(dashAttackImage, skills.dashAttack.cooldown);
        if (parryUnlock)
            CheckCooldownOf(parryImage, skills.parry.cooldown);
        if (assassinateUnlock)
            CheckCooldownOf(assassinateImage, skills.assassinate.cooldown);
        if (multiCrystalUnlock)
            CheckCooldownOf(multiCrystalImage, skills.crystal.multiStackCooldown);
        if (blackholeUnlock)
            CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);

        // 更新装备冷却显示
        if (weaponEquiped)
            CheckCooldownOf(weaponImage, inventory.GetEquipment(EquipmentType.Weapon).itemCooldown);
        if (armorEquiped)
            CheckCooldownOf(armorImage, inventory.GetEquipment(EquipmentType.Armor).itemCooldown);
        if (amuletEquiped)
            CheckCooldownOf(amuletImage, inventory.GetEquipment(EquipmentType.Amulet).itemCooldown);
        if (flaskEquiped)
            CheckCooldownOf(flaskImage, inventory.GetEquipment(EquipmentType.Flask).itemCooldown);
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
