using UnityEngine;

/// <summary>
/// 龙卷风效果 - 具体实现者（Bridge Pattern - ConcreteImplementor）
/// 直接实现 IItemEffect 接口，符合官方桥接模式定义
/// </summary>
[CreateAssetMenu(fileName = "Tornado effect", menuName = "Data/Item effect/Tornado")]
public class Tornado_Effect : ScriptableObject, IItemEffect
{
    [SerializeField] private GameObject thunderPrefab;

    public bool ExecuteEffect(Transform playerPosition) 
    {
        Instantiate(thunderPrefab, playerPosition.position, Quaternion.identity);
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(39);
        return true; // 龙卷风效果总是执行
    }
}
