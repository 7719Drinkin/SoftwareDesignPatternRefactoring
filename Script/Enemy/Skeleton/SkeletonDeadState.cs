public class SkeletonDeadState : EnemyState
{
    private Enemy_Skeleton enemy;

    public SkeletonDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(12);

        enemy.isDead = true;
        enemy.CloseCounterAttackWindow();
        enemy.GetComponentInChildren<UI_HealthBar>().DestroyMe();
        enemy.GetComponentInChildren<UI_EnduranceBar>().DestroyMe();
        enemy.PlayerGetCurrency();
    }

    public override void Update()
    {
        base.Update();

        enemy.rb.velocity = new UnityEngine.Vector2(0, -10);

        if (triggerCalled)
            enemy.SelfDestroy();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
