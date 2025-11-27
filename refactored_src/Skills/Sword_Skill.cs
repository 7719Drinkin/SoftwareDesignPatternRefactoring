using UnityEngine;
using UnityEngine.UI;

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
/// </summary>
public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;       // 当前剑类型

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;             // 弹跳次数
    [SerializeField] private float BounceGravity;          // 弹跳剑重力
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton; // 弹跳剑解锁按钮

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;             // 穿透次数
    [SerializeField] private float pierceGravity;           // 穿透剑重力
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton; // 穿透剑解锁按钮

    [Header("Spin info")]
    [SerializeField] private float hitCooldown;            // 旋转剑攻击冷却
    [SerializeField] private float maxTravelDistance;      // 最大飞行距离
    [SerializeField] private float spinDurantion;          // 旋转持续时间
    [SerializeField] private float spinGravity;            // 旋转剑重力
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton; // 旋转剑解锁按钮

    [Header("Sword info")]
    public bool sword;                                     // 是否解锁剑技能
    [SerializeField] private GameObject swordPrefab;      // 剑预制体
    [SerializeField] private Vector2 launchForce;         // 发射力度
    [SerializeField] private float swordGravity;           // 剑重力
    [SerializeField] private float freezeTimeDuration;    // 冰冻时间持续时间
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton; // 剑技能解锁按钮

    private Vector2 finalDir;                              // 最终发射方向

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;             // 瞄准点数量
    [SerializeField] private float spaceBetweenDots;      // 瞄准点间距
    [SerializeField] private GameObject dotPrefab;         // 瞄准点预制体
    [SerializeField] private Transform dotsParent;         // 瞄准点父对象

    [Header("Freeze info")]
    private bool canFreezeEnemy;                           // 是否可以冰冻敌人
    [SerializeField] private UI_SkillTreeSlot freezeEnemyUnlockButton; // 冰冻敌人解锁按钮

    private GameObject[] dots;                              // 瞄准点数组

    /// <summary>
    /// 初始化剑技能
    /// </summary>
    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();

        // 绑定技能解锁按钮事件
        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        freezeEnemyUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFreezeEnemy);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    /// <summary>
    /// 延迟初始化协程
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        sword = swordUnlockButton.unlocked;
        canFreezeEnemy = freezeEnemyUnlockButton.unlocked;
        
        // 根据解锁状态设置剑的类型（优先级：旋转 > 穿透 > 弹跳 > 普通）
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
        else if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
        else if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
        else
            swordType = SwordType.Regular;
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
        if (Input.GetKey(KeyCode.Mouse1))
            for (int i = 0; i < numberOfDots; i++)
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
    }

    /// <summary>
    /// 创建剑对象
    /// </summary>
    public void CreateSword()
    {
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
        newSwordScript.SetupSword(finalDir, swordGravity, player, canFreezeEnemy, freezeTimeDuration, Inventory.instance.swordUseAmulet);

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
        for (int i = 0; i < numberOfDots; i++)
            dots[i].SetActive(_isActive);
    }

    /// <summary>
    /// 生成瞄准点
    /// </summary>
    private void GenerateDots()
    {
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
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion

    /// <summary>
    /// 解锁剑技能
    /// </summary>
    private void UnlockSword()
    {
        if (swordUnlockButton.CanUnlockSkillSlot() && swordUnlockButton.unlocked)
        {
            sword = true;
        }
    }

    /// <summary>
    /// 解锁弹跳剑
    /// </summary>
    private void UnlockBounce()
    {
        if (bounceUnlockButton.CanUnlockSkillSlot() && bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Bounce;
        }
    }

    /// <summary>
    /// 解锁穿透剑
    /// </summary>
    private void UnlockPierce()
    {
        if (pierceUnlockButton.CanUnlockSkillSlot() && pierceUnlockButton.unlocked)
        {
            swordType = SwordType.Pierce;
        }
    }

    /// <summary>
    /// 解锁旋转剑
    /// </summary>
    private void UnlockSpin()
    {
        if (spinUnlockButton.CanUnlockSkillSlot() && spinUnlockButton.unlocked)
        {
            swordType = SwordType.Spin;
        }
    }

    /// <summary>
    /// 解锁冰冻敌人效果
    /// </summary>
    private void UnlockFreezeEnemy()
    {
        if (freezeEnemyUnlockButton.CanUnlockSkillSlot() && freezeEnemyUnlockButton.unlocked)
        {
            canFreezeEnemy = true;
        }
    }
}
