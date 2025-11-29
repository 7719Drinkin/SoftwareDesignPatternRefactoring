using UnityEngine;
using System.Collections;

public class NightBornWaveState : EnemyState
{
    private Enemy_NightBorn enemy;

    private float waveStateDuration = 5f;
    private float waveSpawnCoolDown = 1f;
    private float waveSpawnTimer;

    private float defaultGravityScale;

    public NightBornWaveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_NightBorn _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravityScale = enemy.rb.gravityScale;
        enemy.rb.gravityScale = 0;

        enemy.ZeroVelocity();

        audioManager.PlaySFX(51);

        stateTimer = Mathf.Infinity;
        waveSpawnTimer = Mathf.Infinity;
        enemy.StartCoroutine(WaveSpawn());
    }

    public IEnumerator WaveSpawn()
    {
        yield return new WaitForSeconds(waveSpawnCoolDown);

        stateTimer = waveStateDuration;
        waveSpawnTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.battleState);

        if (waveSpawnTimer < 0)
        {
            waveSpawnTimer = waveSpawnCoolDown;
            
            // 朝向玩家
            Transform playerTf = ServiceLocator.Instance.Get<IPlayerManager>().Player.transform;
            if (playerTf.position.x > enemy.transform.position.x && enemy.facingDir == -1)
                enemy.Flip();
            else if (playerTf.position.x < enemy.transform.position.x && enemy.facingDir == 1)
                enemy.Flip();
            
            Vector3 spawnOffset = new Vector3(enemy.facingDir * 1.5f, 0, 0);
            Vector3 spawnPosition = enemy.transform.position + spawnOffset;
            
            int elementIndex = Random.Range(0, 3);
            GameObject wavePrefab = null;
            switch (elementIndex)
            {
                case 0:
                    wavePrefab = enemy.fireWavePrefab;
                    break;
                case 1:
                    wavePrefab = enemy.iceWavePrefab;
                    break;
                case 2:
                    wavePrefab = enemy.lightningWavePrefab;
                    break;
            }
           
            if (wavePrefab != null)
                UnityEngine.Object.Instantiate(wavePrefab, spawnPosition, enemy.transform.rotation);
            audioManager.PlaySFX(50);
        }

        waveSpawnTimer -= Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        enemy.rb.gravityScale = defaultGravityScale;
    }
}
