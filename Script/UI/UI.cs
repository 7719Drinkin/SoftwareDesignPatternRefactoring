using System.Collections;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartGameButton;

    private bool isOpen;

    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_SkillToolTip skillToolTip;
    public UI_CraftWindow craftWindow;

    private UI_FadeOut startFadeIn;
    private bool isFadeIn;

    void Start()
    {
        itemToolTip = UI_ItemToolTip.instance;
        statToolTip = UI_StatToolTip.instance;
        skillToolTip = UI_SkillToolTip.instance;
        startFadeIn = fadeIn.GetComponent<UI_FadeOut>();

        SwitchTo(inGameUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isOpen)
            {
                AudioManager.instance.PlaySFX(23);
                SwitchTo(characterUI);

                if (GameManager.instance != null)
                    GameManager.instance.PauseGame(true);
            }
            else
            {
                SwitchTo(inGameUI);
                if (GameManager.instance != null)
                    GameManager.instance.PauseGame(false);
                AudioManager.instance.PlaySFX(23);
            }

            isOpen = !isOpen;
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        // 检查并关闭所有tooltip
        if (itemToolTip != null && itemToolTip.gameObject.activeSelf)
            itemToolTip.HideToolTip();

        if (statToolTip != null && statToolTip.gameObject.activeSelf)
            statToolTip.HideStatToolTip();

        if (skillToolTip != null && skillToolTip.gameObject.activeSelf)
            skillToolTip.HideSkillToolTip();

        // 先关闭除 fadeIn 外的其它子物体
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child != fadeIn) // 跳过淡入层
                child.SetActive(false);
        }

        // 打开目标菜单
        if (_menu != null)
            _menu.SetActive(true);

        // 最后触发一次淡入（并确保它是激活的）
        if (startFadeIn != null && !isFadeIn)
        {
            if (!fadeIn.activeSelf) fadeIn.SetActive(true);
            startFadeIn.FadeIn();
            isFadeIn = true;
        }
    }

    public void DieFadeOut()
    {
        if (startFadeIn != null)
        {
            startFadeIn.FadeOut();
            isFadeIn = false;

            StartCoroutine(ShowDieText());
        }
    }

    private IEnumerator ShowDieText()
    {
        yield return new WaitForSeconds(2);

        endText.SetActive(true);

        yield return new WaitForSeconds(1);

        restartGameButton.SetActive(true);
    }

    public void RestartGame() => GameManager.instance.ReStartScene();

    public void PlayCLickSFX() => AudioManager.instance.PlaySFX(25);

    public void PlayButtonSFX() => AudioManager.instance.PlaySFX(24);
}
