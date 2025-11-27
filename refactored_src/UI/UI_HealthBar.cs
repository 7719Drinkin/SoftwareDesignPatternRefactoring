using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();
        myTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();

        if (slider != null)
            entity.onFlipped -= FlipUI;
        entity.onFlipped += FlipUI;

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = Mathf.RoundToInt(myStats.currentHealth);
    }

    private void Update() => UpdateHealthUI();

    private void FlipUI() => myTransform.Rotate(0, 180, 0);

    public void DestroyMe() => Destroy(gameObject);
}
