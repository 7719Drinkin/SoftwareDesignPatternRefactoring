using System;
using UnityEngine;

/// <summary>
/// 黑洞技能 - 继承自Skill基类
/// 实现黑洞的创建、成长和收缩功能
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Blackhole_Skill : Skill
{
    [SerializeField] private int amountOfAttacks;
    [SerializeField] private float cloneAttackCooldown;
    [SerializeField] private float blackholeDuration;
    [Space]
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    public bool blackhole;

    private Blackhole_Skill_Controller currentBlackhole;
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
        if (evt.SkillName == "Blackhole")
        {
            blackhole = true;
        }
    }

    public void CreateBlackhole()
    {
        GameObject newBlackHole = Instantiate(blackholePrefab, player.transform.position, Quaternion.identity);

        currentBlackhole = newBlackHole.GetComponent<Blackhole_Skill_Controller>();

        currentBlackhole.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, amountOfAttacks, cloneAttackCooldown, blackholeDuration);
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole)
            return false;

        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;

            cooldownTimer = cooldown;

            // ========== 发布到事件总线（Observer Pattern） ==========
            var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
            eventBus?.Publish(new SkillUsedEvent
            {
                SkillName = "Blackhole",
                Cooldown = cooldown
            });

            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

}
