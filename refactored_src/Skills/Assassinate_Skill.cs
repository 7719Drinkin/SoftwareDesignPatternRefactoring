using System;
using UnityEngine;

/// <summary>
/// 暗杀技能 - 继承自Skill基类
/// 实现玩家的暗杀移动和敌人冰冻功能
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Assassinate_Skill : Skill
{
    [Header("Assassinate")]
    public bool assassinate;

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
        if (evt.SkillName == "Assassinate")
        {
            assassinate = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();

        MoveToEnemy();

        // ========== 发布到事件总线（Observer Pattern） ==========
        var eventBus = ServiceLocator.Instance.Get<GameEventBus>();
        eventBus?.Publish(new SkillUsedEvent
        {
            SkillName = "Assassinate",
            Cooldown = cooldown
        });
    }

    private void MoveToEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(player.transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }

        if (closestEnemy != null)
        {
            int enemyFacingDir = closestEnemy.GetComponentInParent<Enemy>().facingDir;

            player.transform.position = new Vector2(closestEnemy.position.x - 5 * enemyFacingDir, closestEnemy.position.y);

            if (player.facingDir != enemyFacingDir)
                player.Flip();

            closestEnemy.GetComponentInParent<Enemy>().FreezeTimeFor(1.5f);
        }
    }

}
