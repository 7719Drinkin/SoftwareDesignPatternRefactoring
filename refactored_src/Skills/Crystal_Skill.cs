using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    [SerializeField] private UI_SkillTreeSlot crystalUnlockButton;
    public bool crystal;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private UI_SkillTreeSlot mirageUnlockButton;
    private bool createMirage;

    [Header("Explosive crystal")]
    [SerializeField] private UI_SkillTreeSlot explaodeUnlockButton;
    private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private UI_SkillTreeSlot moveUnlockButton;
    private bool canMoveToEnemy;

    [Header("Multi crystal")]
    [SerializeField] private int amountOfStacks;
    public float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    [SerializeField] private UI_SkillTreeSlot multiUnlockButton;
    public bool canUseMultiStacks;

    protected override void Start()
    {
        base.Start();

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        mirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirage);
        explaodeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplode);
        moveUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMove);
        multiUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMulti);

        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;

        // 根据技能槽的解锁状态初始化技能状态
        crystal = crystalUnlockButton.unlocked;
        createMirage = mirageUnlockButton.unlocked;
        canExplode = explaodeUnlockButton.unlocked;
        canMoveToEnemy = moveUnlockButton.unlocked;
        canUseMultiStacks = multiUnlockButton.unlocked;

        // 同步触发事件，驱动 UI 立即更新（从存档加载时）
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        if (canUseMultiStacks)
            eventBus?.Publish(new SkillUnlockedEvent { SkillName = "MultiCrystal" });
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

    private void UnlockCrystal()
    {
        if (crystalUnlockButton.CanUnlockSkillSlot() && crystalUnlockButton.unlocked)
        {
            crystal = true;
        }
    }

    private void UnlockMirage()
    {
        if (mirageUnlockButton.CanUnlockSkillSlot() && mirageUnlockButton.unlocked)
        {
            createMirage = true;
        }
    }

    private void UnlockExplode()
    {
        if (explaodeUnlockButton.CanUnlockSkillSlot() && explaodeUnlockButton.unlocked)
        {
            canExplode = true;
        }
    }

    private void UnlockMove()
    {
        if (moveUnlockButton.CanUnlockSkillSlot() && moveUnlockButton.unlocked)
        {
            canMoveToEnemy = true;
        }
    }

    private void UnlockMulti()
    {
        if (multiUnlockButton.CanUnlockSkillSlot() && multiUnlockButton.unlocked)
        {
            canUseMultiStacks = true;
            
            // ========== 发布技能解锁事件到事件总线（Observer Pattern） ==========
            var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
            eventBus?.Publish(new SkillUnlockedEvent
            {
                SkillName = "MultiCrystal"
            });
        }
    }
}
