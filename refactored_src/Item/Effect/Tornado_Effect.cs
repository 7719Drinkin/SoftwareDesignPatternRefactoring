using UnityEngine;

[CreateAssetMenu(fileName = "Tornado effect", menuName = "Data/Item effect/Tornado")]
public class Tornado_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderPrefab;

    public override bool ExecuteEffect(Transform playerPosition) 
    {
        Instantiate(thunderPrefab, playerPosition.position, Quaternion.identity);
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(39);
        return true; // 龙卷风效果总是执行
    }
}
