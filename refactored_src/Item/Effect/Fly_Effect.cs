using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Fly effect", menuName = "Data/Item effect/Fly")]
public class Fly_Effect : ItemEffect
{
    [Header("速度提升设置")]
    [SerializeField] private float speedMultiplier = 1.3f; // 速度倍数
    [SerializeField] private float jumpForce = 1.2f; // 跳跃力度倍数
    [SerializeField] private float duration = 3f; // 持续时间（秒）

    public override bool ExecuteEffect(Transform position)
    {
        if (PlayerManager.instance.player != null)
        {
            PlayerManager.instance.player.StartCoroutine(SpeedBoostEffect());
            AudioManager.instance.PlaySFX(41);
            return true; // 速度提升效果执行成功
        }

        return false; // 玩家不存在时不执行效果
    }

    private IEnumerator SpeedBoostEffect()
    {
        Player player = PlayerManager.instance.player;
        float originalSpeed = player.moveSpeed;
        float originalJumpForce = player.jumpForce;

        // 应用速度提升
        player.moveSpeed *= speedMultiplier;
        player.jumpForce *= jumpForce;

        // 等待持续时间
        yield return new WaitForSeconds(duration);

        // 恢复原始速度
        player.moveSpeed = originalSpeed;
        player.jumpForce = originalJumpForce;
    }
}
