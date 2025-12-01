using System.Collections;
using UnityEngine;

// ========== Composite Pattern (Client) ==========
// 目的：使用组合模式统一管理 UI 元素
// 作用：通过组合模式简化 Tooltip 和 UI 元素组的操作
// ===============================================
public class UI : MonoBehaviour
{
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject fadeIn;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartGameButton;

    [SerializeField] private GameObject popUpTextPrefab;

    private bool isOpen;
    
    // ========== 使用 Facade Pattern 简化服务访问 ==========
    private GameFacade game => GameFacade.Instance;

    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_SkillToolTip skillToolTip;
    public UI_CraftToolTip craftToolTip;
    public UI_CraftWindow craftWindow;

    // ========== Composite Pattern ==========
    private UIElementGroup tooltipGroup;        // Tooltip 组合组
    private UIElementGroup deathUIGroup;        // 死亡界面 UI 元素组
    private IUIComponent endTextComponent;       // 死亡文本组件引用
    private IUIComponent restartButtonComponent; // 重启按钮组件引用
    private IUIComponent fadeComponent;         // 淡入淡出组件引用
    // =======================================

    void Start()
    {
        itemToolTip = UI_ItemToolTip.instance;
        statToolTip = UI_StatToolTip.instance;
        skillToolTip = UI_SkillToolTip.instance;
        craftToolTip = UI_CraftToolTip.instance;

        // ========== 初始化组合模式 ==========
        InitializeCompositeGroups();
        // ===================================

        // ========== 使用组合模式统一隐藏死亡界面 UI ==========
        // 确保游戏开始时死亡界面被隐藏（场景重新加载时）
        deathUIGroup?.Hide();
        // =====================================================

        SwitchTo(inGameUI);
    }

    /// <summary>
    /// 初始化组合模式组
    /// </summary>
    private void InitializeCompositeGroups()
    {
        // 创建 Tooltip 组合组
        tooltipGroup = new UIElementGroup();
        if (itemToolTip != null)
            tooltipGroup.Add(new TooltipComponent(itemToolTip));
        if (statToolTip != null)
            tooltipGroup.Add(new TooltipComponent(statToolTip));
        if (skillToolTip != null)
            tooltipGroup.Add(new TooltipComponent(skillToolTip));
        if (craftToolTip != null)
            tooltipGroup.Add(new TooltipComponent(craftToolTip));

        // 创建死亡界面 UI 元素组
        deathUIGroup = new UIElementGroup();
        if (endText != null)
        {
            endTextComponent = new UIElementComponent(endText);
            deathUIGroup.Add(endTextComponent);
        }
        if (restartGameButton != null)
        {
            restartButtonComponent = new UIElementComponent(restartGameButton);
            deathUIGroup.Add(restartButtonComponent);
        }

        // 创建淡入淡出组件
        if (fadeIn != null)
        {
            fadeComponent = new FadeComponent(fadeIn);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isOpen)
            {
                game.PlaySFX(23);
                SwitchTo(characterUI);
                game.PauseGame(true);
            }
            else
            {
                SwitchTo(inGameUI);
                game.PauseGame(false);
                game.PlaySFX(23);
            }

            isOpen = !isOpen;
        }
    }

    public void SwitchTo(GameObject _menu)
    {
        // ========== 使用组合模式统一关闭所有 Tooltip ==========
        tooltipGroup?.Hide();
        // =====================================================

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

        // ========== 使用组合模式触发淡入 ==========
        fadeComponent?.Show();
        // =======================================
    }

    public void DieFadeOut()
    {
        // ========== 使用组合模式触发淡出 ==========
        fadeComponent?.Hide();

        StartCoroutine(ShowDieText());
        // =======================================
    }

    private IEnumerator ShowDieText()
    {
        yield return new WaitForSeconds(2);

        // ========== 使用组合模式按顺序显示死亡界面 UI ==========
        // 通过组合对象按索引访问组件（更符合组合模式）
        if (deathUIGroup != null && deathUIGroup.Count > 0)
        {
            // 显示第一个组件（endText）
            deathUIGroup.GetComponent(0)?.Show();

            yield return new WaitForSeconds(1);

            // 显示第二个组件（restartGameButton）
            if (deathUIGroup.Count > 1)
                deathUIGroup.GetComponent(1)?.Show();
        }
        // =====================================================
    }

    public void CreateUI_PopUpText(string text)
    {
        if (popUpTextPrefab == null)
            return;

        // 找到Canvas中心位置
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Transform parent = parentCanvas != null ? parentCanvas.transform : transform;

        GameObject newText = Instantiate(popUpTextPrefab, parent);

        // 放到画布中心
        RectTransform rect = newText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        // 设置文本
        var tmp = newText.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = text;
    }

    public void RestartGame() => game.RestartScene();

    public void PlayCLickSFX() => game.PlaySFX(25);

    public void PlayButtonSFX() => game.PlaySFX(24);
}
