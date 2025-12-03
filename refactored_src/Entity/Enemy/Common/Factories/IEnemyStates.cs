/***************************************************************
 *  Refactored with: Factory Method Pattern
 *  Pattern Type: Creational
 *
 *  Document Reference:
 *  - See report section "2.2.2 Factory Method Pattern (Creational)"
 ***************************************************************/
public interface IEnemyStates
{
    /// <summary>
    /// 返回敌人应当进入的初始状态
    /// </summary>
    EnemyState InitialState { get; }
}
/***************************************************************
 * End
 ***************************************************************/

