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

    public float assassinateCooldown;
    public float gatherCooldown;
    public float magicCircleCooldown;
    public float waveCooldown;

    private float gatherTimer;
    private float magicCircleTimer;
    private float waveTimer;

    public GameObject gatherSkill;
    [SerializeField] private GameObject magicCirclePrefab;
    public GameObject fireWavePrefab;
    public GameObject iceWavePrefab;
    public GameObject lightningWavePrefab;

    private bool canAssassinate = false;

    protected override void Awake()
    {
        base.Awake();

        moveState = new NightBornMoveState(this, stateMachine, "Move", this);
        battleState = new NightBornBattleState(this, stateMachine, "Move", this);
        attackState = new NightBornAttackState(this, stateMachine, "Attack", this);
        blockedState = new NightBornBlockedState(this, stateMachine, "Blocked", this);
        stunnedState = new NightBornStunnedState(this, stateMachine, "Stunned", this);
        deadState = new NightBornDeadState(this, stateMachine, "Dead", this);
        gatherState = new NightBornGatherState(this, stateMachine, "Gather", this);
        waveState = new NightBornWaveState(this, stateMachine, "Wave", this);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(moveState);

        gatherTimer = gatherCooldown;
        magicCircleTimer = magicCircleCooldown;
        waveTimer = waveCooldown;

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

        if (gatherTimer < 0 && stateMachine.currentState == battleState && !canAssassinate)
        {
            stateMachine.ChangeState(gatherState);
            gatherTimer = gatherCooldown;
        }

        if (magicCircleTimer < 0 && stateMachine.currentState == battleState)
        {
            Transform playerTf = PlayerManager.instance != null ? PlayerManager.instance.player.transform : null;
            Vector2 circlePos = new Vector2(playerTf.position.x, playerTf.position.y - 1.2f);
            Instantiate(magicCirclePrefab, circlePos, transform.rotation);
            magicCircleTimer = magicCircleCooldown;
        }

        if (waveTimer < 0 && stateMachine.currentState == battleState && !canAssassinate
            && Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 20 
            && Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) > 10)
        {
            stateMachine.ChangeState(waveState);
            waveTimer = waveCooldown;
        }

        gatherTimer -= Time.deltaTime;
        magicCircleTimer -= Time.deltaTime;
        waveTimer -= Time.deltaTime;

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
        if (PlayerManager.instance == null || PlayerManager.instance.player == null)
            return false;

        Transform playerTf = PlayerManager.instance.player.transform;
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

        StartCoroutine(AssassinateAfterDelay(targetX, targetY, 1f));
        return true;
    }

    private System.Collections.IEnumerator AssassinateAfterDelay(float targetX, float targetY, float delaySeconds)
    {
        AudioManager.instance.PlaySFX(49);
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
            StartCoroutine(BlockKnockback(PlayerManager.instance.player.transform));

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
            gatherTimer = gatherCooldown;
            stateMachine.ChangeState(battleState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }
}
