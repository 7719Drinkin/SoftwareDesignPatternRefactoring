using UnityEngine;

public class NightBornGatherState : EnemyState
{
    private Enemy_NightBorn enemy;

    private float gatherDuration = 4f;
    private float healCooldown = 0.8f;
    private float healPercent = 0.02f;
    private float healTimer;

    public NightBornGatherState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = gatherDuration;

        enemy.ZeroVelocity();

        enemy.gatherSkill.SetActive(true);

        audioManager.PlaySFX(54);
    }

    public override void Update()
    {
        base.Update();

        if (healTimer < 0)
        {
            int healAmount = Mathf.RoundToInt(enemy.enemyStats.GetMaxHealthValue() * healPercent);
            enemy.enemyStats.IncreaseHealthBy(healAmount);

            healTimer = healCooldown;
        }

        healTimer -= Time.deltaTime;

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();

        enemy.gatherSkill.SetActive(false);
    }
}
