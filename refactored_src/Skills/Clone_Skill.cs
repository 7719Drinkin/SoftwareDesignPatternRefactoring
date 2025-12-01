using System.Collections;
using UnityEngine;

/// <summary>
/// 分身技能 - 继承自Skill基类
/// 实现分身的创建、复制和水晶替代功能
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Clone_Skill : Skill
{
    [Header("Mirage info")]
#pragma warning disable CS0414
    private bool mirage;
#pragma warning restore CS0414

    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    public bool clone;

    [Header("Clone can Duplicate")]
    [SerializeField] private int chanceToDuplicate;
    private bool canDuplicateClone;

    [Header("Crysatl instead of clone")]
    public bool crystalInsteadOfClone;

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
            case "Mirage":
                mirage = true;
                break;
            case "Clone":
                clone = true;
                break;
            case "Duplicate":
                canDuplicateClone = true;
                break;
            case "CrystalInstead":
                crystalInsteadOfClone = true;
                break;
        }
    }

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            ServiceLocator.Instance.Get<ISkillManager>().Crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

		// 目标点可能在延迟期间被销毁，做空保护并回退到玩家
		var spawnTransform = _clonePosition != null ? _clonePosition : player.transform;
		var target = FindClosetEnemy(spawnTransform);
		newClone.GetComponent<Clone_Skill_Controller>().SetupClone(spawnTransform, cloneDuration, _offset, target, canDuplicateClone, chanceToDuplicate, player, clone);
    }

    public void CreateCloneOnDashStart()
    {
        if (ServiceLocator.Instance.Get<ISkillManager>().Dash.cloneOnDash)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if (ServiceLocator.Instance.Get<ISkillManager>().Dash.cloneOnDash)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (ServiceLocator.Instance.Get<ISkillManager>().Parry.fightBack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(1.2f * player.facingDir, 0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.4f);
		CreateClone(_transform != null ? _transform : (player != null ? player.transform : transform), _offset);
    }

}
