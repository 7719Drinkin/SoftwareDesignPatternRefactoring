using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float BounceGravity;
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;

    [Header("Pierce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;

    [Header("Spin info")]
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDurantion;
    [SerializeField] private float spinGravity;
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;

    [Header("Sword info")]
    public bool sword;
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    [Header("Freeze info")]
    private bool canFreezeEnemy;
    [SerializeField] private UI_SkillTreeSlot freezeEnemyUnlockButton;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        bounceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockBounce);
        pierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockPierce);
        spinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSpin);
        freezeEnemyUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockFreezeEnemy);
        
        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }
    
    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;
        
        // 根据技能槽的解锁状态初始化技能状态
        sword = swordUnlockButton.unlocked;
        canFreezeEnemy = freezeEnemyUnlockButton.unlocked;
        
        // 根据解锁状态设置剑的类型
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
        else if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
        else if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
        else
            swordType = SwordType.Regular;
    }

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

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
            for (int i = 0; i < numberOfDots; i++)
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, player.transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

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

        newSwordScript.SetupSword(finalDir, swordGravity, player, canFreezeEnemy, freezeTimeDuration, Inventory.instance.swordUseAmulet);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < numberOfDots; i++)
            dots[i].SetActive(_isActive);
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + 0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }

    #endregion

    private void UnlockSword()
    {
        if (swordUnlockButton.CanUnlockSkillSlot() && swordUnlockButton.unlocked)
        {
            sword = true;
        }
    }

    private void UnlockBounce()
    {
        if (bounceUnlockButton.CanUnlockSkillSlot() && bounceUnlockButton.unlocked)
        {
            swordType = SwordType.Bounce;
        }
    }

    private void UnlockPierce()
    {
        if (pierceUnlockButton.CanUnlockSkillSlot() && pierceUnlockButton.unlocked)
        {
            swordType = SwordType.Pierce;
        }
    }

    private void UnlockSpin()
    {
        if (spinUnlockButton.CanUnlockSkillSlot() && spinUnlockButton.unlocked)
        {
            swordType = SwordType.Spin;
        }
    }

    private void UnlockFreezeEnemy()
    {
        if (freezeEnemyUnlockButton.CanUnlockSkillSlot() && freezeEnemyUnlockButton.unlocked)
        {
            canFreezeEnemy = true;
        }
    }
}
