using TMPro;
using UnityEngine;

public class UI_CraftToolTip : MonoBehaviour
{
    public static UI_CraftToolTip instance;

    [SerializeField] private TextMeshProUGUI materialName;

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

    public void ShowCraftToolTip(string _materialName)
    {
        materialName.text = _materialName;

        gameObject.SetActive(true);
    }

    public void HideCraftToolTip() => gameObject.SetActive(false);
}
