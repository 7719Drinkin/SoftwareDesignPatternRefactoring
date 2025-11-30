using System;

public class SlimeStates : IEnemyStates
{
    public SlimeMoveState Move { get; }
    public SlimeAttackState Attack { get; }
    public SlimeDeadState Dead { get; }
    public SlimeBattleState Battle { get; }

    public SlimeStates(
        SlimeMoveState move,
        SlimeAttackState attack,
        SlimeDeadState dead,
        SlimeBattleState battle)
    {
        Move = move ?? throw new ArgumentNullException(nameof(move));
        Attack = attack ?? throw new ArgumentNullException(nameof(attack));
        Dead = dead ?? throw new ArgumentNullException(nameof(dead));
        Battle = battle ?? throw new ArgumentNullException(nameof(battle));
    }

    public EnemyState InitialState => Move;
}

