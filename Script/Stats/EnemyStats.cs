using UnityEngine;
using System.Collections;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private Player player;

    [Header("Endurance stats")]
    public Stat maxEndurance;
    public int currentEndurance;
    public int enduranceRestore;
    public int blockCost;
    public int blockedCost;

    public float restoreCooldown;
    private float cooldownTimer;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        player = PlayerManager.instance.player.GetComponent<Player>();

        currentEndurance = maxEndurance.GetValue();
    }

    protected override void Update()
    {
        base.Update();

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0 && currentEndurance < maxEndurance.GetValue() && !enemy.isStunned)
        {
            cooldownTimer = restoreCooldown;

            currentEndurance += enduranceRestore;

            if (currentEndurance > maxEndurance.GetValue())
                currentEndurance = maxEndurance.GetValue();
        }
    }

    public override void TakeDamage(int _damage, Transform _attacker, bool _canDoDamage)
    {
        if (_attacker.GetComponent<Player>() != null)
        {
            if (player.facingDir == enemy.facingDir || enemy.isStunned || _canDoDamage)
            {
                _canDoDamage = true;
                AudioManager.instance.PlaySFX(4);
            }
            else
            {
                AudioManager.instance.PlaySFX(6);
            }
        }
        else
        {
            float directionToAttackerX = _attacker.position.x - transform.position.x;
            if (directionToAttackerX * enemy.facingDir <= 0 || enemy.isStunned || _canDoDamage)
            {
                _canDoDamage = true;

                if (_attacker.GetComponent<Clone_Skill_Controller>() != null || _attacker.GetComponent<Sword_Skill_Controller>() != null)
                    AudioManager.instance.PlaySFX(4);
            }
            else
            {
                AudioManager.instance.PlaySFX(6);
            }
        }

        if (_canDoDamage)
            base.TakeDamage(_damage, _attacker, _canDoDamage);
        else
            currentEndurance -= blockCost;

        enemy.DamageEffect(_attacker, _canDoDamage, _canDoDamage);

        // 玩家被敌人格挡：恢复相机抖动与短暂停顿
        if (_attacker.GetComponent<Player>() != null && !_canDoDamage)
        {
            if (CombatFeedback.instance != null)
                CombatFeedback.instance.DoHitStop(0.12f);
            if (CinemachineShaker.instance != null)
                CinemachineShaker.instance.Shake(0.8f, 1.4f, 0.12f);
        }
    }

    protected override void Die()
    {
        base.Die();

        // 生成经验掉落
        if (enemy.experienceDrop != null)
        {
            enemy.experienceDrop.SetExperienceAmount(enemy.experencePoints);
            enemy.experienceDrop.GenerateExperienceDrop();
        }

        enemy.Die();
    }
}
