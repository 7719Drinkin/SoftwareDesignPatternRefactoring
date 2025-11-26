using System.Collections;
using UnityEngine;
using Cinemachine;

public class CinemachineShaker : MonoBehaviour
{
    public static CinemachineShaker instance { get; private set; }

    [Header("Defaults")]
    public float defaultAmplitude = 1.2f;
    public float defaultFrequency = 1.0f;
    public float defaultDuration = 0.12f;

    [Header("Noise Profile")]
    public NoiseSettings noiseProfile;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void Shake(float amplitude, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitude, defaultFrequency, duration));
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(amplitude, frequency, duration));
    }

    private IEnumerator ShakeCoroutine(float amplitude, float frequency, float duration)
    {
        CinemachineBrain brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : null;
        if (brain == null)
            yield break;

        ICinemachineCamera activeCam = brain.ActiveVirtualCamera;
        if (activeCam == null)
            yield break;

        GameObject vcamGO = activeCam.VirtualCameraGameObject;
        CinemachineVirtualCamera vcam = vcamGO.GetComponent<CinemachineVirtualCamera>();
        if (vcam == null)
            yield break;

        CinemachineBasicMultiChannelPerlin perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (perlin == null)
        {
            // Try add one if missing
            perlin = vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = 0f;
            perlin.m_FrequencyGain = 0f;

        }

        // 若没有噪声配置，赋默认配置（需在Inspector指定），否则不会抖动
        if (perlin.m_NoiseProfile == null)
        {
            if (noiseProfile != null)
            {
                perlin.m_NoiseProfile = noiseProfile;
            }
        }

        float originalAmp = perlin.m_AmplitudeGain;
        float originalFreq = perlin.m_FrequencyGain;

        // 设置幅度和频率
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;


        float timer = 0f;
        while (timer < duration)
        {
            // 使用不受时间缩放影响的时间增量
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        perlin.m_AmplitudeGain = originalAmp;
        perlin.m_FrequencyGain = originalFreq;

        shakeCoroutine = null;
    }
}
