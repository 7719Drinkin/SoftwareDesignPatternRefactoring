using UnityEngine;

/// <summary>
/// 雷电打击效果 - 具体实现者（Bridge Pattern - ConcreteImplementor）
/// 直接实现 IItemEffect 接口，符合官方桥接模式定义
/// </summary>
[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]
public class ThunderStrike_Effect : ScriptableObject, IItemEffect
{
    [SerializeField] private GameObject thunderPrefab;

    public bool ExecuteEffect(Transform enemyPosition)
    {
        Instantiate(thunderPrefab, enemyPosition.position, Quaternion.identity);
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(27);
        return true; // 雷电效果总是执行
    }
}
