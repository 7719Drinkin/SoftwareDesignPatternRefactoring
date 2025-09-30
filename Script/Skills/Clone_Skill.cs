using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Mirage info")]
    [SerializeField] private UI_SkillTreeSlot mirageUnlockButton;
#pragma warning disable CS0414
    private bool mirage;
#pragma warning restore CS0414

    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private UI_SkillTreeSlot cloneUnlockButton;
    public bool clone;

    [Header("Clone can Duplicate")]
    [SerializeField] private int chanceToDuplicate;
    [SerializeField] private UI_SkillTreeSlot duplicateUnlockButton;
    private bool canDuplicateClone;

    [Header("Crysatl instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInsteadUnlockButton;
    public bool crystalInsteadOfClone;

    protected override void Start()
    {
        base.Start();

        mirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirage);
        cloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockClone);
        duplicateUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDuplicate);
        crystalInsteadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);

        // 延迟初始化，确保保存系统已经加载完成
        StartCoroutine(DelayedInitialization());
    }

    private System.Collections.IEnumerator DelayedInitialization()
    {
        // 等待一帧，确保所有保存数据都已加载
        yield return null;

        // 根据技能槽的解锁状态初始化技能状态
        mirage = mirageUnlockButton.unlocked;
        clone = cloneUnlockButton.unlocked;
        canDuplicateClone = duplicateUnlockButton.unlocked;
        crystalInsteadOfClone = crystalInsteadUnlockButton.unlocked;
    }

    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, _offset, FindClosetEnemy(_clonePosition), canDuplicateClone, chanceToDuplicate, player, clone);
    }

    public void CreateCloneOnDashStart()
    {
        if (SkillManager.instance.dash.cloneOnDash)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnDashOver()
    {
        if (SkillManager.instance.dash.cloneOnDash)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (SkillManager.instance.parry.fightBack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(1.2f * player.facingDir, 0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(_transform, _offset);
    }

    private void UnlockMirage()
    {
        if (mirageUnlockButton.CanUnlockSkillSlot() && mirageUnlockButton.unlocked)
        {
            mirage = true;
        }
    }

    private void UnlockClone()
    {
        if (cloneUnlockButton.CanUnlockSkillSlot() && cloneUnlockButton.unlocked)
        {
            clone = true;
        }
    }

    private void UnlockDuplicate()
    {
        if (duplicateUnlockButton.CanUnlockSkillSlot() && duplicateUnlockButton.unlocked)
        {
            canDuplicateClone = true;
        }
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInsteadUnlockButton.CanUnlockSkillSlot() && crystalInsteadUnlockButton.unlocked)
        {
            crystalInsteadOfClone = true;
        }
    }
}
