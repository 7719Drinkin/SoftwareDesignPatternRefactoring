using System.Collections;
using UnityEngine;
using Cinemachine;

public class CombatFeedback : MonoBehaviour
{
    public static CombatFeedback instance { get; private set; }

    [Header("Screen Flash (UI)")]
    public CanvasGroup flashCanvasGroup; // 全屏UI(白色Image)所在的CanvasGroup
    public float flashMaxAlpha = 0.6f;

    private Coroutine zoomCoroutine;
    private Coroutine chromaCoroutine;
    
    // Zoom state
    private float zoomEndTime;
    private float currentZoomDelta;
    private float currentZoomInDuration;
    private float currentZoomOutDuration;
    
    // Flash state  
    private float flashEndTime;
    private float currentFlashIntensity;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Hit-stop state
    private Coroutine hitStopCoroutine;
    private float hitStopEndTime;

    public void DoHitStop(float duration)
    {
        // Extend or start a single hit-stop window
        float now = Time.realtimeSinceStartup;
        float requestedEnd = now + Mathf.Max(0f, duration);

        if (hitStopCoroutine == null)
        {
            hitStopEndTime = requestedEnd;
            hitStopCoroutine = StartCoroutine(HitStopController());
        }
        else
        {
            // Extend the end time if a new request lasts longer
            if (requestedEnd > hitStopEndTime)
                hitStopEndTime = requestedEnd;
        }
    }

    public void DoZoom(float delta, float inDuration, float outDuration)
    {
        float now = Time.realtimeSinceStartup;
        float requestedEnd = now + inDuration + outDuration;
        
        // 保存当前参数
        currentZoomDelta = delta;
        currentZoomInDuration = inDuration;
        currentZoomOutDuration = outDuration;
        
        if (zoomCoroutine == null)
        {
            zoomEndTime = requestedEnd;
            zoomCoroutine = StartCoroutine(ZoomController());
        }
        else
        {
            // 延长结束时间
            if (requestedEnd > zoomEndTime)
                zoomEndTime = requestedEnd;
        }
    }

    public void DoChromaticFlash(float intensity, float duration)
    {
        if (flashCanvasGroup == null)
            return;
            
        float now = Time.realtimeSinceStartup;
        float requestedEnd = now + duration;
        
        // 保存当前参数
        currentFlashIntensity = intensity;
        
        if (chromaCoroutine == null)
        {
            flashEndTime = requestedEnd;
            chromaCoroutine = StartCoroutine(FlashController());
        }
        else
        {
            // 延长结束时间
            if (requestedEnd > flashEndTime)
                flashEndTime = requestedEnd;
        }
    }

    private IEnumerator HitStopController()
    {
        // Force pause
        Time.timeScale = 0f;

        while (Time.realtimeSinceStartup < hitStopEndTime)
            yield return null;

        // Restore to normal gameplay
        Time.timeScale = 1f;
        hitStopCoroutine = null;
    }

    private IEnumerator ZoomController()
    {
        yield return StartCoroutine(ZoomCoroutine(currentZoomDelta, currentZoomInDuration, currentZoomOutDuration));
        
        // 等待到结束时间
        while (Time.realtimeSinceStartup < zoomEndTime)
            yield return null;
            
        zoomCoroutine = null;
    }

    private IEnumerator FlashController()
    {
        yield return StartCoroutine(ChromaticFlashCoroutine(currentFlashIntensity, flashEndTime - Time.realtimeSinceStartup));
        
        // 等待到结束时间
        while (Time.realtimeSinceStartup < flashEndTime)
            yield return null;
            
        chromaCoroutine = null;
    }

    // 缩放控制协程：实现摄像机的缩放动画效果（带弹性回弹）
    // delta: 缩放变化量（正数放大视野，负数缩小视野）
    // inDuration: 缩放到目标值所需时间
    // outDuration: 回弹到原始值所需时间
    private IEnumerator ZoomCoroutine(float delta, float inDuration, float outDuration)
    {
        CinemachineBrain brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : null;
        if (brain == null || brain.ActiveVirtualCamera == null)
            yield break;

        var vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (vcam == null)
            yield break;

        // 检查摄像机是否使用正交投影
        bool isOrtho = vcam.m_Lens.Orthographic;
        float t = 0f;

        if (!isOrtho)
        {
            float originalFov = vcam.m_Lens.FieldOfView; // 保存原始视野值
            float targetFov = Mathf.Max(1f, originalFov + delta); // 计算目标视野值，确保不小于1度
            while (t < inDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = inDuration <= 0f ? 1f : Mathf.Clamp01(t / inDuration);
                vcam.m_Lens.FieldOfView = Mathf.Lerp(originalFov, targetFov, a);
                yield return null;
            }
            // 回弹
            t = 0f;
            while (t < outDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = outDuration <= 0f ? 1f : Mathf.Clamp01(t / outDuration);
                vcam.m_Lens.FieldOfView = Mathf.Lerp(targetFov, originalFov, a);
                yield return null;
            }
            vcam.m_Lens.FieldOfView = originalFov;
        }
        else
        {
            float originalOrtho = vcam.m_Lens.OrthographicSize;
            float targetOrtho = Mathf.Max(0.01f, originalOrtho + delta); // delta 为负缩小视野
            while (t < inDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = inDuration <= 0f ? 1f : Mathf.Clamp01(t / inDuration);
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(originalOrtho, targetOrtho, a);
                yield return null;
            }
            // 回弹
            t = 0f;
            while (t < outDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = outDuration <= 0f ? 1f : Mathf.Clamp01(t / outDuration);
                vcam.m_Lens.OrthographicSize = Mathf.Lerp(targetOrtho, originalOrtho, a);
                yield return null;
            }
            vcam.m_Lens.OrthographicSize = originalOrtho;
        }

        zoomCoroutine = null;
    }

    private IEnumerator ChromaticFlashCoroutine(float targetIntensity, float duration)
    {
        // 使用全屏UI白色Image的alpha进行闪烁，targetIntensity映射到alpha（0~1）
        float originalAlpha = flashCanvasGroup.alpha;
        float targetAlpha = Mathf.Clamp01(targetIntensity) * flashMaxAlpha;
        float half = Mathf.Max(0f, duration * 0.5f);
        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float a = half <= 0f ? 1f : Mathf.Clamp01(t / half);
            flashCanvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, a);
            yield return null;
        }
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float a = half <= 0f ? 1f : Mathf.Clamp01(t / half);
            flashCanvasGroup.alpha = Mathf.Lerp(targetAlpha, originalAlpha, a);
            yield return null;
        }
        flashCanvasGroup.alpha = originalAlpha;
        chromaCoroutine = null;
    }
}
