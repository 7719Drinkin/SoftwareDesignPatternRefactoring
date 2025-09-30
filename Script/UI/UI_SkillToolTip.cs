using TMPro;
using UnityEngine;

public class UI_SkillToolTip : MonoBehaviour
{
    public static UI_SkillToolTip instance { get; private set; }

    [SerializeField] private TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillName;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        gameObject.SetActive(false);
    }

    public void ShowSkillToolTip(string _skillDescription, string _skillName)
    {
        skillDescription.text = _skillDescription;
        skillName.text = _skillName;
        gameObject.SetActive(true);
    }

    public void HideSkillToolTip() => gameObject.SetActive(false);
}
