using UnityEngine;
using System.Collections;

public class ExperienceObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int experienceAmount;
    [SerializeField] private Vector2 velocity;

    // ========== 服务依赖 ==========
    private IPlayerManager playerManager;

    [Header("Color Flash Settings")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color[] flashColors;
    [SerializeField] private float flashInterval = 0.3f; // 颜色切换间隔
    [SerializeField] private bool randomizeColors = true;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        // 获取SpriteRenderer组件
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        // 保存原始颜色
        originalColor = sr.color;

        // 开始闪烁效果
        StartColorFlash();

        // 通过ServiceLocator获取依赖
        playerManager = ServiceLocator.Instance.Get<IPlayerManager>();
    }

    public void SetupObject(int _experienceAmount, Vector2 _velocity)
    {
        experienceAmount = _experienceAmount;
        rb.velocity = _velocity;
    }

    public int GetExperienceAmount()
    {
        return experienceAmount;
    }

    private void StartColorFlash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(ColorFlashCoroutine());
    }

    private IEnumerator ColorFlashCoroutine()
    {
        while (true)
        {
            // 随机选择颜色
            Color flashColor = randomizeColors ?
                flashColors[Random.Range(0, flashColors.Length)] :
                flashColors[Random.Range(0, flashColors.Length)];

            // 直接设置为随机颜色
            sr.color = flashColor;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public void PickUpItem()
    {
        // 停止闪烁效果
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        rb.velocity = new Vector2(0, 12);

        playerManager.AddExperience(experienceAmount);

        StartCoroutine(DestroyMe());
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(0.6f);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 确保协程被正确停止
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }
}
