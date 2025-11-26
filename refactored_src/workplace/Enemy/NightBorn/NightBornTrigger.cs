using System.Collections;
using UnityEngine;

public class NightBornTrigger : MonoBehaviour
{
    [SerializeField] private Enemy_NightBorn nightBorn;
    [SerializeField] private float battleStartDelay = 1.5f;
    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (triggered)
            return;

        if (collider.GetComponent<Player>() != null)
        {
            triggered = true;

            if (nightBorn != null)
                StartCoroutine(EnterBattleAfterDelay());
        }
    }

    private IEnumerator EnterBattleAfterDelay()
    {
        if (battleStartDelay > 0f)
            yield return new WaitForSeconds(battleStartDelay);

        if (nightBorn != null)
            nightBorn.stateMachine.ChangeState(nightBorn.battleState);

        AudioManager.instance.PlaySFX(55);
        AudioManager.instance.PlayBGM(2);
    }
}
