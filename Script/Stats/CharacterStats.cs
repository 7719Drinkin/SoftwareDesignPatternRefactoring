using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Auto,       // 自动选择（根据最高伤害）
    Fire,       // 火焰效果
    Ice,        // 寒冰效果
    Lightning   // 闪电效果
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Level details")]
    public Stat level;

    [Header("Major stats")]
    public Stat strength; // 5 point increases damage by 1 and crit.power by 1%
    public Stat agility; // 1 point increases evasion by 1% and crit.chance by 1%
    public Stat intelligence; // 1 point increases magic damage by 1 and magic resistance by 3
    public Stat vitality; // 1 point increases health by 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float ignitedCooldown = 0.3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    private int shockDamage;
    [SerializeField] private GameObject shockStrikePrefab;

    public int currentHealth;
    public bool isDead { get; private set; }

    protected ItemDrop myDropSystem;

    protected virtual void Start()
    {
        fx = GetComponent<EntityFX>();
        myDropSystem = GetComponent<ItemDrop>();

        // Level scaling: only apply to enemies, players get their bonuses from PlayerManager
        if (GetComponent<Player>() == null)
        {
            // Enemy: apply level bonuses based on enemy's preset level
            Modify(damage, 10);
            Modify(strength, 3);
            Modify(agility, 1);
            Modify(intelligence, 2);
            Modify(vitality, 30);
        }

        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (igniteDamageTimer < 0 && isIgnited)
        {
            igniteDamageTimer = ignitedCooldown;
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();
        }
    }

    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify) => StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }

    private void Modify(Stat _stat, int perLevel)
    {
        for (int i = 1; i < level.GetValue(); i++)
        {
            _stat.AddModifier(perLevel);
        }
    }

    public virtual void TakeDamage(int damage, Transform attacker, bool canDoDamage)
    {
        DecreaseHealthBy(damage);

        if (currentHealth <= 0 && !isDead)
            Die();
    }

    public void IncreaseHealthBy(int amount)
    {
        currentHealth += amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();
    }

    protected virtual void DecreaseHealthBy(int damage) => currentHealth -= damage;


    public virtual void DoDamage(CharacterStats targetStats, Transform attacker, bool canDoDamage)
    {
        if (TargetCanAvoidAttack(targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue() * 5;

        if (CanCrit())
            totalDamage = CalculateCriticalDamage(totalDamage);

        totalDamage = CheckTargetArmor(targetStats, totalDamage);

        targetStats.TakeDamage(totalDamage, attacker, canDoDamage);
    }

    #region Magical damage and ailments
    public virtual void DoMagicalDamage(CharacterStats targetStats, Transform attacker)
    {
        DoMagicalDamage(targetStats, attacker, ElementType.Auto);
    }

    public virtual void DoMagicalDamage(CharacterStats targetStats, Transform attacker, ElementType specifiedElement)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicalDamage -= targetStats.magicResistance.GetValue() + targetStats.intelligence.GetValue() * 3;
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);

        targetStats.TakeDamage(totalMagicalDamage, attacker, true);

        // 确定要应用的元素效果
        bool canApplyIgnite = false;
        bool canApplyChill = false;
        bool canApplyShock = false;

        if (specifiedElement == ElementType.Auto)
        {
            // 自动选择：根据最高伤害值选择元素效果
            int maxDamage = Mathf.Max(_fireDamage, _iceDamage, _lightningDamage);
            if (maxDamage <= 0)
                return;

            List<int> maxElements = new List<int>();
            if (_fireDamage == maxDamage) maxElements.Add(0);
            if (_iceDamage == maxDamage) maxElements.Add(1);
            if (_lightningDamage == maxDamage) maxElements.Add(2);
            int selectedElement = maxElements[Random.Range(0, maxElements.Count)];

            canApplyIgnite = (selectedElement == 0);
            canApplyChill = (selectedElement == 1);
            canApplyShock = (selectedElement == 2);
        }
        else
        {
            // 指定元素：强制使用指定的元素效果
            switch (specifiedElement)
            {
                case ElementType.Fire:
                    canApplyIgnite = (_fireDamage > 0);
                    break;
                case ElementType.Ice:
                    canApplyChill = (_iceDamage > 0);
                    break;
                case ElementType.Lightning:
                    canApplyShock = (_lightningDamage > 0);
                    break;
            }
        }

        // 应用对应的伤害效果
        if (canApplyIgnite)
            targetStats.SetupIgniteDamage(Mathf.RoundToInt(targetStats.currentHealth * Mathf.Clamp(_fireDamage * 0.001f, 0, 0.03f) + _fireDamage * 0.3f));

        if (canApplyShock)
            targetStats.SetupShockDamage(Mathf.RoundToInt(_lightningDamage * 1.2f));

        targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = 4;

            fx.IgniteFxFor(4);
        }

        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = 4;

            GetComponent<Entity>().SlowEntityBy(0.5f, 4);
            fx.ChillFxFor(4);
        }

        if (_shock && canApplyShock)
        {
            if (!isShocked)
                ApplyShock(_shock);
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                ThunderStike();
            }
        }
    }

    public void ApplyShock(bool _shock)
    {
        isShocked = _shock;
        shockedTimer = 4;

        fx.ShockFxFor(4);
    }

    public void ThunderStike()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.transform != transform)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            if (closestEnemy == null)
                closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newThunderStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newThunderStrike.GetComponent<ThunderStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockDamage(int _damage) => shockDamage = _damage;

    #endregion

    #region calculations
    private int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
    {
        if (targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(targetStats.armor.GetValue() * 0.7f);
        else
            totalDamage -= targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);

        return totalDamage;
    }

    private bool TargetCanAvoidAttack(CharacterStats targetStats)
    {
        int totalEvation = targetStats.evasion.GetValue() + targetStats.agility.GetValue();

        if (isShocked)
            totalEvation += 30;

        if (Random.Range(0, 100) < totalEvation)
            return true;

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) < totalCriticalChance)
            return true;

        return false;
    }

    private int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue() * 5) * 0.01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    #endregion

    protected virtual void Die()
    {
        isDead = true;
        myDropSystem.GenerateDrop();
    }

    public int GetMaxHealthValue() => maxHealth.GetValue() + vitality.GetValue() * 5;

    public Stat StatOfType(StatType statType)
    {
        switch (statType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.maxHealth:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicResistance:
                return magicResistance;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.lightningDamage:
                return lightningDamage;
            case StatType.level:
                return level;
        }

        return null;
    }
}
