using UnityEngine;

public class Enemy_NightBorn : Enemy
{
    #region States
    public NightBornMoveState moveState { get; private set; }
    public NightBornBattleState battleState { get; private set; }
    public NightBornAttackState attackState { get; private set; }
    public NightBornBlockedState blockedState { get; private set; }
    public NightBornStunnedState stunnedState { get; private set; }
    public NightBornDeadState deadState { get; private set; }
    public NightBornGatherState gatherState { get; private set; }
    public NightBornWaveState waveState { get; private set; }

    #endregion

    private NightBornStates nightBornStates;

    public float assassinateCooldown;
    [Header("Shared Skill Cooldown")]
    public float sharedSkillCooldown = 10f;  // gather、wave、魔法阵共享冷却时间

    private float sharedSkillTimer;

    public GameObject gatherSkill;
    [SerializeField] private GameObject magicCirclePrefab;
    public GameObject fireWavePrefab;
    public GameObject iceWavePrefab;
    public GameObject lightningWavePrefab;

    public bool canAssassinate { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();

        nightBornStates = new NightBornStateFactory().CreateStates(this, stateMachine);

        moveState = nightBornStates.Move;
        battleState = nightBornStates.Battle;
        attackState = nightBornStates.Attack;
        blockedState = nightBornStates.Blocked;
        stunnedState = nightBornStates.Stunned;
        deadState = nightBornStates.Dead;
        gatherState = nightBornStates.Gather;
        waveState = nightBornStates.Wave;
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(nightBornStates.InitialState);

        sharedSkillTimer = sharedSkillCooldown;

        gatherSkill.SetActive(false);
        CloseCounterAttackWindow();
        if (discoverImage != null)
            discoverImage.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (stateMachine != null && stateMachine.currentState != null)
        {
            if (enemyStats != null && enemyStats.currentEndurance <= 0 && stateMachine.currentState != stunnedState)
                stateMachine.ChangeState(stunnedState);
        }

        // 共享冷却时间：冷却结束时随机触发 gather、wave 或魔法阵
        if (sharedSkillTimer < 0 && stateMachine.currentState == battleState && !canAssassinate && !isStunned)
        {
            Transform playerTf = playerManager != null ? playerManager.Player.transform : null;
            if (playerTf != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTf.position);
                
                // 构建可用的技能列表
                System.Collections.Generic.List<int> availableSkills = new System.Collections.Generic.List<int>();
                
               
                availableSkills.Add(0);
                availableSkills.Add(1);
                availableSkills.Add(2);
                
                // 随机选择一个可用技能
                if (availableSkills.Count > 0)
                {
                    int selectedSkill = availableSkills[Random.Range(0, availableSkills.Count)];
                    
                    switch (selectedSkill)
                    {
                        case 0: // Gather
                            stateMachine.ChangeState(gatherState);
                            break;
                        case 1: // Wave
                            stateMachine.ChangeState(waveState);
                            break;
                        case 2: // Magic Circle
                            Vector2 circlePos = new Vector2(playerTf.position.x, playerTf.position.y - 1.2f);
                            Instantiate(magicCirclePrefab, circlePos, transform.rotation);
                            break;
                    }
                    
                    sharedSkillTimer = sharedSkillCooldown;
                }
            }
        }

        sharedSkillTimer -= Time.deltaTime;

        if (isDead && stateMachine.currentState != deadState)
            SelfDestroy();
    }

    /// <summary>
    /// 传送到玩家身后 offsetX 位置；若该位置下方无地面，则不传送
    /// </summary>
    /// <param name="offsetX">相对玩家背后水平偏移（正值）</param>
    /// <returns>是否成功传送</returns>
    public bool TryAssassinateBehindPlayer(float offsetX)
    {
        // 敌人被冻结时不能传送
        if (isStunned)
            return false;

        if (playerManager == null || playerManager.Player == null)
            return false;

        Transform playerTf = playerManager.Player.transform;
        Player playerComp = playerTf.GetComponent<Player>();
        if (playerComp == null)
            return false;

        int behindDir = -playerComp.facingDir;
        float targetX = playerTf.position.x + behindDir * Mathf.Abs(offsetX);
        float targetY = playerTf.position.y;

        // 先做一次地面检测，若失败则直接返回 false
        Vector2 probeStart = new Vector2(targetX, targetY);
        bool hasGround = Physics2D.Raycast(probeStart, Vector2.down, groundCheckDistance, whatIsGround);
        if (!hasGround)
            return false;

        StartCoroutine(AssassinateAfterDelay(targetX, targetY, 0.25f));
        return true;
    }

    private System.Collections.IEnumerator AssassinateAfterDelay(float targetX, float targetY, float delaySeconds)
    {
        audioManager.PlaySFX(49);
        canAssassinate = true;
        yield return new WaitForSeconds(delaySeconds);
        transform.position = new Vector2(targetX, targetY);
        stateMachine.ChangeState(attackState);
        canAssassinate = false;
    }

    public override bool EnemyCanBeBlocked()
    {
        if (base.EnemyCanBeBlocked())
        {
            StartCoroutine(BlockKnockback(playerManager.Player.transform));

            Vector2 spawnPosition = new Vector2(transform.position.x + (0.5f * facingDir), transform.position.y + 1.2f);
            Instantiate(sparkPrefab, spawnPosition, transform.rotation);

            if (enemyStats != null)
                enemyStats.currentEndurance -= enemyStats.blockedCost;

            if (enemyStats.currentEndurance > 0)
                stateMachine.ChangeState(blockedState);

            return true;
        }

        return false;
    }

    public override void DamageEffect(Transform attackerTransform, bool canFlash, bool isKnocked)
    {
        base.DamageEffect(attackerTransform, canFlash, isKnocked);

        if (stateMachine != null && stateMachine.currentState == gatherState && canFlash)
        {
            sharedSkillTimer = sharedSkillCooldown;  // 重置共享冷却时间
            stateMachine.ChangeState(battleState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}
