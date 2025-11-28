using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderPrefab;

    public override bool ExecuteEffect(Transform enemyPosition)
    {
        Instantiate(thunderPrefab, enemyPosition.position, Quaternion.identity);
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(27);
        return true; // 雷电效果总是执行
    }
}
