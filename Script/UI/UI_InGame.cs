using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI levelText; // 内圈等级文本
    [SerializeField] private Image experienceRing; // 外圈经验环
    [SerializeField] private float currencyIncreasePerSecond = 100f; // 显示用的每秒增加速度
    private int displayedCurrency; // 当前UI显示的金币数量

    [SerializeField] private Image dashImage;
    [SerializeField] private Image cloneOnDashImage;
    [SerializeField] private Image dashAttackImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image assassinateImage;
    [SerializeField] private Image multiCrystalImage;
    [SerializeField] private Image blackholeImage;

    [SerializeField] private Image weaponImage;
    [SerializeField] private Image armorImage;
    [SerializeField] private Image amuletImage;
    [SerializeField] private Image flaskImage;

    private bool dashUnlock;
    private bool cloneOnDashUnlock;
    private bool dashAttackUnlock;
    private bool parryUnlock;
    private bool assassinateUnlock;
    private bool multiCrystalUnlock;
    private bool blackholeUnlock;

    private bool weaponEquiped;
    private bool armorEquiped;
    private bool amuletEquiped;
    private bool flaskEquiped;

    private SkillManager skills;
    private Inventory inventory;

    void Start()
    {
        skills = SkillManager.instance;
        inventory = Inventory.instance;
        displayedCurrency = PlayerManager.instance.currency;
        if (currencyText != null)
            currencyText.text = displayedCurrency.ToString();

        skills.dash.OnDashUnlocked += UnlockDash;
        skills.dash.OnCloneOnDashUnlock += UnlockCloneOnDash;
        skills.dash.OnDashAttackUnlock += UnlockDashAttack;
        skills.parry.OnUnlockParry += UnlockParry;
        skills.assassinate.OnAssassinateUnlock += UnlockAsssassinate;
        skills.crystal.OnMultiCrystalUnlock += UnlockMultiCrystal;
        skills.blackhole.OnBlackholeUnlock += UnlockBlackhole;

        skills.dash.OnDashUsed += UseDash;
        PlayerManager.instance.player.dashState.OnCloneOnDashUsed += UseCloneOnDash;
        skills.dashAttack.OnDashAttackUsed += UseDashAttack;
        PlayerManager.instance.player.counterAttack.OnParryUsed += UseParry;
        skills.assassinate.OnAssassinateUsed += UseAssassinate;
        skills.crystal.OnMultiCrystalUsed += UseMultiCrystal;
        skills.blackhole.OnBlackholeUsed += UseBlackhole;

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

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = Mathf.RoundToInt(playerStats.currentHealth);
    }

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

    private void UpdateExperienceUI()
    {
        if (levelText != null)
            levelText.text = PlayerManager.instance.playerLevel.ToString();

        if (experienceRing != null)
        {
            float fill = Mathf.Clamp01(PlayerManager.instance.currentExperience / (200 * (PlayerManager.instance.playerLevel / 5 + 1)));
            experienceRing.fillAmount = fill;
        }
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateCurrencyUI();
        UpdateExperienceUI();

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

        if (weaponEquiped)
            CheckCooldownOf(weaponImage, inventory.GetEquipment(EquipmentType.Weapon).itemCooldown);
        if (armorEquiped)
            CheckCooldownOf(armorImage, inventory.GetEquipment(EquipmentType.Armor).itemCooldown);
        if (amuletEquiped)
            CheckCooldownOf(amuletImage, inventory.GetEquipment(EquipmentType.Amulet).itemCooldown);
        if (flaskEquiped)
            CheckCooldownOf(flaskImage, inventory.GetEquipment(EquipmentType.Flask).itemCooldown);
    }

    private void CheckCooldownOf(Image image, float cooldown)
    {
        if (image.fillAmount > 0)
            image.fillAmount -= 1 / cooldown * Time.deltaTime;
    }

    private void SetCooldownOf(Image image) => image.fillAmount = 1;

    private void UnlockDash()
    {
        dashUnlock = true;
        dashImage.fillAmount = 0;
    }

    private void UnlockCloneOnDash()
    {
        cloneOnDashUnlock = true;
        cloneOnDashImage.fillAmount = 0;
    }

    private void UnlockDashAttack()
    {
        dashAttackUnlock = true;
        dashAttackImage.fillAmount = 0;
    }

    private void UnlockParry()
    {
        parryUnlock = true;
        parryImage.fillAmount = 0;
    }

    private void UnlockAsssassinate()
    {
        assassinateUnlock = true;
        assassinateImage.fillAmount = 0;
    }

    private void UnlockMultiCrystal()
    {
        multiCrystalUnlock = true;
        multiCrystalImage.fillAmount = 0;
    }

    private void UnlockBlackhole()
    {
        blackholeUnlock = true;
        blackholeImage.fillAmount = 0;
    }

    private void EquipWeapon()
    {
        weaponEquiped = true;
        weaponImage.fillAmount = 0;
    }

    private void EquipArmor()
    {
        armorEquiped = true;
        armorImage.fillAmount = 0;
    }

    private void EquipAmulet()
    {
        amuletEquiped = true;
        amuletImage.fillAmount = 0;
    }

    private void EquipFlask()
    {
        flaskEquiped = true;
        flaskImage.fillAmount = 0;
    }

    private void UnequipWeapon()
    {
        weaponEquiped = false;
        weaponImage.fillAmount = 1;
    }

    private void UnequipedArmor()
    {
        armorEquiped = false;
        armorImage.fillAmount = 1;
    }

    private void UnequipedAmulet()
    {
        amuletEquiped = false;
        amuletImage.fillAmount = 1;
    }

    private void UnEquipedFlask()
    {
        flaskEquiped = false;
        flaskImage.fillAmount = 1;
    }

    private void UseDash() => SetCooldownOf(dashImage);

    private void UseCloneOnDash() => SetCooldownOf(cloneOnDashImage);

    private void UseDashAttack() => SetCooldownOf(dashAttackImage);

    private void UseParry() => SetCooldownOf(parryImage);

    private void UseAssassinate() => SetCooldownOf(assassinateImage);

    private void UseMultiCrystal() => SetCooldownOf(multiCrystalImage);

    private void UseBlackhole() => SetCooldownOf(blackholeImage);

    private void UseWeapon() => SetCooldownOf(weaponImage);

    private void UseArmor() => SetCooldownOf(armorImage);

    private void UseAmulet() => SetCooldownOf(amuletImage);

    private void UseFlask() => SetCooldownOf(flaskImage);
}
