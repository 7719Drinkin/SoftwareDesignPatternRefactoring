using UnityEngine;

/// <summary>
/// 技能基类 - 所有技能的基础类
/// 提供通用的技能冷却、使用检测和敌人查找功能
/// 被各种具体技能类继承，实现统一的技能行为
/// </summary>
public class Skill : MonoBehaviour
{
    public float cooldown;                                 // 技能冷却时间
    protected float cooldownTimer;                        // 冷却计时器
    protected Player player;                              // 玩家引用

    // ========== 服务依赖 ==========
    protected IPlayerManager playerManager;
    protected IAudioManager audioManager;

    /// <summary>
    /// 初始化技能
    /// </summary>
    protected virtual void Start()
    {
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        TryEnsurePlayer();

        cooldownTimer = -100;
    }

    /// <summary>
    /// 更新冷却计时器
    /// </summary>
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 检查是否可以使用技能
    /// </summary>
    /// <returns>是否可以使用技能</returns>
    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            UseSkill();
            cooldownTimer = cooldown;
            return true;
        }

        player.fx.CreatePopUpText("技能冷却中");

        return false;
    }

    /// <summary>
    /// 使用技能（可被子类重写）
    /// </summary>
    public virtual void UseSkill() { }

    /// <summary>
    /// 查找最近的敌人
    /// </summary>
    /// <param name="_checkTransform">检测位置</param>
    /// <returns>最近的敌人Transform</returns>
    protected virtual Transform FindClosetEnemy(Transform _checkTransform)
    {
		if (_checkTransform == null)
			return null;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, 25);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

		foreach (var hit in colliders)
        {
			if (hit != null && hit.transform != null && hit.GetComponent<Enemy>() != null)
            {
				float distanceToEnemt = Vector2.Distance(_checkTransform.position, hit.transform.position);

                if (distanceToEnemt < closestDistance)
                {
                    closestDistance = distanceToEnemt;
                    closestEnemy = hit.transform;
                }
            }
        }

        return closestEnemy;
    }

    protected bool TryEnsurePlayer()
    {
        if (player != null)
            return true;

        if (playerManager == null)
            playerManager = ServiceLocator.Instance.Get<IPlayerManager>();

        if (playerManager == null || playerManager.Player == null)
            return false;

        player = playerManager.Player;
        return player != null;
    }
}
