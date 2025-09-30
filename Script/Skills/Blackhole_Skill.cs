using System;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private UI_SkillTreeSlot blackholeUnlockButton;
    public bool blackhole;

    private Blackhole_Skill_Controller currentBlackhole;

    public event Action OnBlackholeUnlock;
    public event Action OnBlackholeUsed;

    protected override void Start()
    {
        base.Start();

        blackholeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        blackhole = blackholeUnlockButton.unlocked;

        if (blackhole)
            OnBlackholeUnlock?.Invoke();
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
            OnBlackholeUsed?.Invoke();

            return true;
        }

        return false;
    }

    public float GetBlackholeRadius()
    {
        return maxSize / 2;
    }

    private void UnlockBlackhole()
    {
        if (blackholeUnlockButton.CanUnlockSkillSlot() && blackholeUnlockButton.unlocked)
        {
            blackhole = true;
            OnBlackholeUnlock?.Invoke();
        }
    }
}
