using UnityEngine;

[CreateAssetMenu(fileName = "Fire effect", menuName = "Data/Item effect/Fire Effect")]
public class Fire_Effect : ItemEffect
{
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject shinePrefab;

    public override bool ExecuteEffect(Transform respondPosition)
    {
        Player player = PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;

        if (!thirdAttack)
            return false; // 没有执行效果，不消耗冷却

        GameObject newFire = Instantiate(firePrefab, respondPosition.position, player.transform.rotation);

        Vector3 shineSpawnPosition = respondPosition.position + new Vector3(player.facingDir * 2.5f, 0);
        GameObject newShine1 = Instantiate(shinePrefab, shineSpawnPosition, player.transform.rotation);
        GameObject newShine2 = Instantiate(shinePrefab, shineSpawnPosition, player.transform.rotation);
        GameObject newShine3 = Instantiate(shinePrefab, shineSpawnPosition, player.transform.rotation);

        Vector2 velocity1 = new Vector2(player.facingDir * 25, 0);
        Vector2 velocity2 = new Vector2(player.facingDir * 25, 3);
        Vector2 velocity3 = new Vector2(player.facingDir * 25, -3);

        newShine1.GetComponent<Shine_Controller>().Setup(velocity1);
        newShine2.GetComponent<Shine_Controller>().Setup(velocity2);
        newShine3.GetComponent<Shine_Controller>().Setup(velocity3);

        return true; // 成功执行了效果
    }
}
