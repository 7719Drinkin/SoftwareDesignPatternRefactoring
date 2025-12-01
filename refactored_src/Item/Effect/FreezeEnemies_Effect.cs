using UnityEngine;

/// <summary>
/// 冰冻敌人效果 - 具体实现者（Bridge Pattern - ConcreteImplementor）
/// 直接实现 IItemEffect 接口
/// </summary>
[CreateAssetMenu(fileName = "Freeze enemies effect", menuName = "Data/Item effect/Freeze enemies")]
public class FreezeEnemies_Effect : ScriptableObject, IItemEffect
{
    [SerializeField] private float duration;

    public bool ExecuteEffect(Transform position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position.position, 2);

        foreach (var hit in colliders)
        {
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }
        
        return true; 
    }
}