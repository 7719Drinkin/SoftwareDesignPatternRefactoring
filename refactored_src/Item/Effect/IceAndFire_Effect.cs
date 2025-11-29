using UnityEngine;

[CreateAssetMenu(fileName = "Ice and fire effect", menuName = "Data/Item effect/Ice and fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override bool ExecuteEffect(Transform respondPosition)
    {
        Player player = ServiceLocator.Instance.Get<IPlayerManager>().Player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (!thirdAttack)
            return false; // 没有执行效果，不消耗冷却

        GameObject newIceAndFire = Instantiate(iceAndFirePrefab, respondPosition.position, player.transform.rotation);

        newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

        Destroy(newIceAndFire, 5);

        return true; // 成功执行了效果
    }
}
