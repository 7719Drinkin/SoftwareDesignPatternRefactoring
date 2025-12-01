using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NightBornTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tilemapBlock;

    [SerializeField] private Enemy_NightBorn nightBorn;
    [SerializeField] private float battleStartDelay = 1.5f;
    private bool triggered;
    private IAudioManager audioManager;

    private void Awake()
    {
        audioManager = ServiceLocator.Instance.Get<IAudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (triggered)
            return;

        if (collider.GetComponent<Player>() != null)
        {
            triggered = true;

            if (nightBorn != null)
                StartCoroutine(EnterBattleAfterDelay());

            StartCoroutine(SetBlockActive());
        }
    }

    private IEnumerator EnterBattleAfterDelay()
    {
        if (battleStartDelay > 0f)
            yield return new WaitForSeconds(battleStartDelay);

        if (nightBorn != null)
            nightBorn.stateMachine.ChangeState(nightBorn.battleState);

        audioManager.PlaySFX(55);
        audioManager.PlayBGM(2);
    }

    private IEnumerator SetBlockActive()
    {
        yield return new WaitForSeconds(3);

        tilemapBlock.SetActive(true);
    }
}
