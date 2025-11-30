using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    // ========== 服务依赖 ==========
    private IPlayerManager playerManager;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool canPlayerDisappear = true;

    private int amountOfAttacks = 5;
    private float cloneAttackCooldown = 0.3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createHotKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (ServiceLocator.Instance.Get<ISkillManager>().Clone.crystalInsteadOfClone)
            canPlayerDisappear = false;
    }

    private void Update()
    {
        // 延迟初始化依赖
        if (playerManager == null)
            playerManager = ServiceLocator.Instance.Get<IPlayerManager>();

		CleanTargets();

        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

			if (targets.Count > 0)
                ReleaseCloneAttack();
            else
            {
                DestroyHotKeys();
                FinishBlackholeAbility();
            }
        }

		// 若已进入克隆攻击阶段，但所有目标在过程中死亡/销毁，则立即结束黑洞
		if (cloneAttackReleased && targets.Count == 0 && amountOfAttacks > 0)
		{
			DestroyHotKeys();
			Invoke("FinishBlackholeAbility", 1);
			return;
		}

        if (Input.GetKeyDown(KeyCode.V))
            ReleaseCloneAttack();

        CloneAttackLogic();

        if (canGrow && !canShrink)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        if (targets.Count < 0)
            return;

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;
        
        if (canPlayerDisappear)
        {
            canPlayerDisappear = false;
            playerManager.Player.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

			CleanTargets();
			if (targets.Count <= 0)
                return;
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset = Random.Range(0, 100) > 50 ? 2 : -2;

            if (ServiceLocator.Instance.Get<ISkillManager>().Clone.crystalInsteadOfClone)
            {
                ServiceLocator.Instance.Get<ISkillManager>().Crystal.CreateCrystal();
                ServiceLocator.Instance.Get<ISkillManager>().Crystal.CurrentCrystalChooseRandomTarget();
            }
			else
            {
				if (targets[randomIndex] != null)
					ServiceLocator.Instance.Get<ISkillManager>().Clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }

            amountOfAttacks--;

            if (amountOfAttacks <= 0)
                Invoke("FinishBlackholeAbility", 1);
        }
    }

	private void CleanTargets()
	{
		targets.RemoveAll(t => t == null || (t.GetComponent<Enemy>() != null && t.GetComponent<Enemy>().isDead));
	}

    private void FinishBlackholeAbility()
    {
        playerCanExitState = true;
        cloneAttackReleased = false;
        canShrink = true;
    }

    private void DestroyHotKeys()
    {
        if (createHotKey.Count <= 0)
            return;

        for (int i = 0; i < createHotKey.Count; i++)
            Destroy(createHotKey[i]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

            playerManager.Player.stats.DoMagicalDamage(collision.GetComponent<CharacterStats>(), transform);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null && !collision.GetComponent<Enemy>().isDead)
            collision.GetComponent<Enemy>().FreezeTime(true);
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        collision.GetComponent<Enemy>()?.FreezeTime(false);

        if (collision.GetComponent<Enemy>() != null)
            playerManager.Player.stats.DoMagicalDamage(collision.GetComponent<CharacterStats>(), transform);
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0 || !canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}