public class NightBornDeadState : EnemyState
{
    private Enemy_NightBorn enemy;

    public NightBornDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.FreezeTime(false);

        enemy.isDead = true;
        enemy.CloseCounterAttackWindow();
        enemy.GetComponentInChildren<UI_HealthBar>().DestroyMe();
        enemy.GetComponentInChildren<UI_EnduranceBar>().DestroyMe();

        AudioManager.instance.PlaySFX(52);
        AudioManager.instance.PlayBGM(1);
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
            enemy.SelfDestroy();

        enemy.fx.ForceResetAllColors();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
