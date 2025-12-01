/***************************************************************
 *  Refactored with: Factory Method Pattern
 *  Pattern Type: Creational
 *
 *  Document Reference:
 *  - See report section "2.2.2 Factory Method Pattern (Creational)"
 ***************************************************************/
public interface IEnemyStateFactory<out TStates> where TStates : IEnemyStates
{
    /// <summary>
    /// 创建并返回敌人所有需要的状态集合
    /// </summary>
    /// <param name="enemy">状态所属的敌人实例</param>
    /// <param name="stateMachine">敌人的状态机</param>
    /// <returns>状态集合</returns>
    TStates CreateStates(Enemy enemy, EnemyStateMachine stateMachine);
}
/***************************************************************
 * End
 ***************************************************************/

