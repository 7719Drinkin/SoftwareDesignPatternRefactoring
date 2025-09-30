using UnityEngine;
using UnityEngine.UI;

public class UI_EnduranceBar : MonoBehaviour
{
    private Entity entity;
    private EnemyStats enemyStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        enemyStats = GetComponentInParent<EnemyStats>();
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        if (slider != null)
            entity.onFlipped -= FlipUI;
        entity.onFlipped += FlipUI;

        UpdateEnduranceUI();
    }

    private void UpdateEnduranceUI()
    {
        slider.maxValue = enemyStats.maxEndurance.GetValue();
        slider.value = Mathf.RoundToInt(enemyStats.currentEndurance);
    }

    private void Update() => UpdateEnduranceUI();

    private void FlipUI() => myTransform.Rotate(0, 180, 0);

    public void DestroyMe() => Destroy(gameObject);
}
