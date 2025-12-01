using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水晶技能 - 继承自Skill基类
/// 实现水晶的创建、传送和多重水晶效果
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    public bool crystal;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    private bool createMirage;

    [Header("Explosive crystal")]
    private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private float moveSpeed;
    private bool canMoveToEnemy;

    [Header("Multi crystal")]
    [SerializeField] private int amountOfStacks;
    public float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    public bool canUseMultiStacks;

    private GameEventBus eventBus;

    protected override void Start()
    {
        base.Start();

        // ========== 订阅技能解锁事件（Observer Pattern） ==========
        eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Subscribe<SkillUnlockedEvent>(OnSkillUnlocked);
        // ===========================================================
    }

    private void OnDestroy()
    {
        // 取消订阅
        eventBus?.Unsubscribe<SkillUnlockedEvent>(OnSkillUnlocked);
    }

    /// <summary>
    /// 处理技能解锁事件 - 从解锁类接收解锁通知
    /// </summary>
    private void OnSkillUnlocked(SkillUnlockedEvent evt)
    {
        switch (evt.SkillName)
        {
            case "Crystal":
                crystal = true;
                break;
            case "Mirage":
                createMirage = true;
                break;
            case "Explode":
                canExplode = true;
                break;
            case "Move":
                canMoveToEnemy = true;
                break;
            case "MultiCrystal":
                canUseMultiStacks = true;
                break;
        }
    }

    public override void UseSkill()
    {
        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
            CreateCrystal();
        else
        {
            if (canMoveToEnemy)
                return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            audioManager.PlaySFX(30);

            if (createMirage)
            {
                ServiceLocator.Instance.Get<ISkillManager>().Clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>()?.FinishCrystal();
            }
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();
        audioManager.PlaySFX(31);
        currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(currentCrystal.transform), player);
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                audioManager.PlaySFX(31);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(newCrystal.transform), player);

                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefillCrystal();

                    // ========== 发布到事件总线（Observer Pattern） ==========
                    var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
                    eventBus?.Publish(new SkillUsedEvent
                    {
                        SkillName = "Crystal",
                        Cooldown = multiStackCooldown
                    });
                }

                return true;
            }
        }

        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - crystalLeft.Count;

        for (int i = 0; i < amountToAdd; i++)
            crystalLeft.Add(crystalPrefab);
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefillCrystal();

        // ========== 发布到事件总线（Observer Pattern） ==========
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Publish(new SkillUsedEvent
        {
            SkillName = "Crystal",
            Cooldown = multiStackCooldown
        });
    }

}
