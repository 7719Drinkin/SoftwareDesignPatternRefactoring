using UnityEngine;
using TMPro;

public class UI_TextFade : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("Fade Settings")]
    [SerializeField] [Min(0f)] private float cycleDurationSeconds = 2f; // 一次完整 0→255→0 的总时长
    [SerializeField] [Range(0, 255)] private int minAlpha = 0;
    [SerializeField] [Range(0, 255)] private int maxAlpha = 255;
    [SerializeField] private bool useUnscaledTime = true; // 不受 Time.timeScale 影响
    [SerializeField] private bool playOnEnable = true;

    private bool isPlaying;

    private void Reset()
    {
        targetText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (playOnEnable)
            StartFade();
    }

    private void OnDisable()
    {
        isPlaying = false;
    }

    public void StartFade()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
        isPlaying = true;
    }

    public void StopFade()
    {
        isPlaying = false;
    }

    private void Update()
    {
        if (!isPlaying || targetText == null || cycleDurationSeconds <= 0f)
            return;

        float time = useUnscaledTime ? Time.unscaledTime : Time.time;

        // 取余数，让时间在[0,1]之间循环
        float t = Mathf.PingPong(time / (cycleDurationSeconds * 2f), 1f);

        float aMin = Mathf.Clamp01(minAlpha / 255f);
        float aMax = Mathf.Clamp01(maxAlpha / 255f);
        float a = Mathf.Lerp(aMin, aMax, t);

        Color c = targetText.color;
        c.a = a;
        targetText.color = c;
    }
}


