using UnityEngine;

/// <summary>
/// 剑类型枚举
/// </summary>
public enum SwordType
{
    Regular,    // 普通剑
    Bounce,     // 弹跳剑
    Pierce,     // 穿透剑
    Spin        // 旋转剑
}

/// <summary>
/// 剑技能 - 继承自Skill基类
/// 实现剑的发射、瞄准和不同类型的剑效果
/// 支持弹跳、穿透、旋转等特殊效果
/// 通过事件总线接收解锁事件，实现与解锁逻辑的解耦
/// </summary>
public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;       // 当前剑类型

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;             // 弹跳次数
    [SerializeField] private float BounceGravity;          // 弹跳剑重力

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;             // 穿透次数
    [SerializeField] private float pierceGravity;           // 穿透剑重力

    [Header("Spin info")]
    [SerializeField] private float hitCooldown;            // 旋转剑攻击冷却
    [SerializeField] private float maxTravelDistance;      // 最大飞行距离
    [SerializeField] private float spinDurantion;          // 旋转持续时间
    [SerializeField] private float spinGravity;            // 旋转剑重力

    [Header("Sword info")]
    public bool sword;                                     // 是否解锁剑技能
    [SerializeField] private GameObject swordPrefab;      // 剑预制体
    [SerializeField] private Vector2 launchForce;         // 发射力度
    [SerializeField] private float swordGravity;           // 剑重力
    [SerializeField] private float freezeTimeDuration;    // 冰冻时间持续时间

    private Vector2 finalDir;                              // 最终发射方向

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;             // 瞄准点数量
    [SerializeField] private float spaceBetweenDots;      // 瞄准点间距
    [SerializeField] private GameObject dotPrefab;         // 瞄准点预制体
    [SerializeField] private Transform dotsParent;         // 瞄准点父对象

    [Header("Freeze info")]
    private bool canFreezeEnemy;                           // 是否可以冰冻敌人

    private GameObject[] dots;                              // 瞄准点数组
    private GameEventBus eventBus;

    /// <summary>
    /// 初始化剑技能
    /// </summary>
    protected override void Start()
    {
        base.Start();

        StartCoroutine(InitializeWhenPlayerReady());

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
            case "Sword":
                sword = true;
                break;
            case "Bounce":
                swordType = SwordType.Bounce;
                SetupGravity();
                break;
            case "Pierce":
                swordType = SwordType.Pierce;
                SetupGravity();
                break;
            case "Spin":
                swordType = SwordType.Spin;
                SetupGravity();
                break;
            case "FreezeEnemy":
                canFreezeEnemy = true;
                break;
        }
    }

    private System.Collections.IEnumerator InitializeWhenPlayerReady()
    {
        while (!TryEnsurePlayer())
            yield return null;

        GenerateDots();
        SetupGravity();

        // 延迟初始化，确保保存系统已经加载完成
        yield return DelayedInitialization();
    }
    
    /// <summary>
    /// 延迟初始化协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 注意：技能状态初始化现在由解锁类通过事件总线处理
        // 解锁类会在 DelayedInitialization 中发布已解锁技能的事件
    }

    /// <summary>
    /// 设置剑的重力
    /// </summary>
    private void SetupGravity()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordGravity = BounceGravity;
                break;
            case SwordType.Pierce:
                swordGravity = pierceGravity;
                break;
            case SwordType.Spin:
                swordGravity = spinGravity;
                break;
        }
    }

    /// <summary>
    /// 更新剑技能状态
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // 鼠标右键释放时确定最终发射方向
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        // 鼠标右键按住时更新瞄准点位置
        if (Input.GetKey(KeyCode.Mouse1) && dots != null)
            for (int i = 0; i < numberOfDots; i++)
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
    }

    /// <summary>
    /// 创建剑对象
    /// </summary>
    public void CreateSword()
    {
        if (!TryEnsurePlayer())
            return;

        GameObject newSword = Instantiate(swordPrefab, player.transform.position, player.transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        // 根据剑类型设置不同的效果
        switch (swordType)
        {
            case SwordType.Bounce:
                newSwordScript.SetupBounce(true, bounceAmount);
                break;
            case SwordType.Pierce:
                newSwordScript.SetupPierce(pierceAmount);
                break;
            case SwordType.Spin:
                newSwordScript.SetupSpin(true, maxTravelDistance, spinDurantion, hitCooldown);
                break;
        }

        // 设置剑的基本属性
        newSwordScript.SetupSword(finalDir, swordGravity, player, canFreezeEnemy, freezeTimeDuration, GameFacade.Instance.AmuletSkills.SwordUseAmulet);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim region
    /// <summary>
    /// 获取瞄准方向
    /// </summary>
    /// <returns>瞄准方向向量</returns>
    public Vector2 AimDirection()
    {
        if (!TryEnsurePlayer())
            return Vector2.zero;

        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    /// <summary>
    /// 设置瞄准点的显示状态
    /// </summary>
    /// <param name="_isActive">是否激活</param>
    public void DotsActive(bool _isActive)
    {
        if (dots == null)
            return;

        for (int i = 0; i < numberOfDots; i++)
            dots[i].SetActive(_isActive);
    }

    /// <summary>
    /// 生成瞄准点
    /// </summary>
    private void GenerateDots()
    {
        if (!TryEnsurePlayer())
            return;

        dots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    /// <summary>
    /// 计算瞄准点位置
    /// </summary>
    /// <param name="t">时间参数</param>
    /// <returns>瞄准点位置</returns>
    private Vector2 DotsPosition(float t)
    {
        if (!TryEnsurePlayer())
            return Vector2.zero;

        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion

}
