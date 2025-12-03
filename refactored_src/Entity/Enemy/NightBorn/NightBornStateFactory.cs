using System;

/***************************************************************
 *  Refactored with: Factory Method Pattern
 *  Pattern Type: Creational
 *
 *  Document Reference:
 *  - See report section "2.2.2 Factory Method Pattern (Creational)"
 ***************************************************************/
public class NightBornStateFactory : IEnemyStateFactory<NightBornStates>
{
    public NightBornStates CreateStates(Enemy enemy, EnemyStateMachine stateMachine)
    {
        if (!(enemy is Enemy_NightBorn nightBorn))
            throw new ArgumentException("NightBornStatesFactory requires an Enemy_NightBorn instance.", nameof(enemy));

        return new NightBornStates(
            new NightBornMoveState(nightBorn, stateMachine, "Move", nightBorn),
            new NightBornBattleState(nightBorn, stateMachine, "Move", nightBorn),
            new NightBornAttackState(nightBorn, stateMachine, "Attack", nightBorn),
            new NightBornBlockedState(nightBorn, stateMachine, "Blocked", nightBorn),
            new NightBornStunnedState(nightBorn, stateMachine, "Stunned", nightBorn),
            new NightBornDeadState(nightBorn, stateMachine, "Dead", nightBorn),
            new NightBornGatherState(nightBorn, stateMachine, "Gather", nightBorn),
            new NightBornWaveState(nightBorn, stateMachine, "Wave", nightBorn)
        );
    }
}
/***************************************************************
 * End
 ***************************************************************/

