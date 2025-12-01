using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {

    }

    public override void Enter()
    {
        base.Enter();

        GameObject.Find("Canvas").GetComponent<UI>().DieFadeOut();

        // 防止快速退出游戏，没有保存数据，这里手动保存一次
        if (saveManager != null)
            saveManager.SaveGame();
        // 延迟保存，等待物品物理运动完成
        player.StartCoroutine(DelayedSave());

        audioManager.PlaySFX(8);
    }

    private System.Collections.IEnumerator DelayedSave()
    {
        // 等待物品物理运动完成
        yield return new WaitForSeconds(1f);
        
        if (saveManager != null)
            saveManager.SaveGame();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.ZeroVelocity();
    }
}
