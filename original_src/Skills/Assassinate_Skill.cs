using System;
using UnityEngine;
using UnityEngine.UI;

public class Assassinate_Skill : Skill
{
    [Header("Assassinate")]
    public bool assassinate;
    [SerializeReference] private UI_SkillTreeSlot assassinateUnlockButton;

    public event Action OnAssassinateUnlock;
    public event Action OnAssassinateUsed;

    protected override void Start()
    {
        base.Start();

        assassinateUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAssassinate);

        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;

        // 根据技能槽的解锁状态初始化技能状态
        assassinate = assassinateUnlockButton.unlocked;

        // 同步触发事件，驱动 UI 立即更新
        if (assassinate)
            OnAssassinateUnlock?.Invoke();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        MoveToEnemy();

        OnAssassinateUsed?.Invoke();
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

    private void UnlockAssassinate()
    {
        if (assassinateUnlockButton.CanUnlockSkillSlot() && assassinateUnlockButton.unlocked)
        {
            assassinate = true;
            OnAssassinateUnlock?.Invoke();
        }
    }
}
