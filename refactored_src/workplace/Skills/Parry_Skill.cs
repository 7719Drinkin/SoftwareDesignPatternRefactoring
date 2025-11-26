using System;
using UnityEngine;
using UnityEngine.UI;

public class Parry_Skill : Skill
{
    [Header("Parry")]
    public bool parry;
    [SerializeField] private UI_SkillTreeSlot parryUnlockButton;

    [Header("Fight back")]
    public bool fightBack;
    [SerializeField] private UI_SkillTreeSlot fightBackUnlockButton;

    public event Action OnUnlockParry;

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        fightBackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFightBack);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        parry = parryUnlockButton.unlocked;
        fightBack = fightBackUnlockButton.unlocked;

        if (parry)
            OnUnlockParry?.Invoke();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.CanUnlockSkillSlot() && parryUnlockButton.unlocked)
        {
            parry = true;
            OnUnlockParry?.Invoke();
        }
    }

    private void UnlockFightBack()
    {
        if (fightBackUnlockButton.CanUnlockSkillSlot() && fightBackUnlockButton.unlocked)
        {
            fightBack = true;
        }
    }
}
