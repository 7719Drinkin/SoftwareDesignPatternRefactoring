using System;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash Info")]
    public float dashSpeed;
    public float dashDuration;

    [HideInInspector]
    public float dashDir;

    [Header("Dash")]
    public bool dash;
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;

    [Header("Clone on dash")]
    public bool cloneOnDash;
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;

    [Header("Dash attack")]
    public bool dashAttack;
    [SerializeField] private UI_SkillTreeSlot dashAttackUnlockButton;

    public event Action OnDashUnlocked;
    public event Action OnCloneOnDashUnlock;
    public event Action OnDashAttackUnlock;

    public event Action OnDashUsed;

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        dashAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        dash = dashUnlockButton.unlocked;
        cloneOnDash = cloneOnDashUnlockButton.unlocked;
        dashAttack = dashAttackUnlockButton.unlocked;

        if (dash)
            OnDashUnlocked?.Invoke();
        if (cloneOnDash)
            OnCloneOnDashUnlock?.Invoke();
        if (dashAttack)
            OnDashAttackUnlock?.Invoke();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        OnDashUsed?.Invoke();
    }

    private void UnlockDash()
    {
        if (dashUnlockButton.CanUnlockSkillSlot() && dashUnlockButton.unlocked)
        {
            dash = true;
            OnDashUnlocked?.Invoke();
        }
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.CanUnlockSkillSlot() && cloneOnDashUnlockButton.unlocked)
        {
            cloneOnDash = true;
            OnCloneOnDashUnlock?.Invoke();
        }
    }

    private void UnlockCloneAttack()
    {
        if (dashAttackUnlockButton.CanUnlockSkillSlot() && dashAttackUnlockButton.unlocked)
        {
            dashAttack = true;
            OnDashAttackUnlock?.Invoke();
        }
    }
}
