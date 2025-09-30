using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    // 状态标志
    private bool shockColorState = false;
    private bool igniteColorState = false;
    private bool chillColorState = false;
    private bool stunColorState = false;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    #region Transparency
    public void MakeTransprent(bool _transparent)
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }
    #endregion

    #region Flash Effects
    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;

        sr.color = Color.white;
        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        stunColorState = !stunColorState;
        sr.color = stunColorState ? Color.red : Color.white;
    }
    #endregion

    #region Ailment Effects
    public void ShockFxFor(float _seconds)
    {
        CancelInvoke("ShockColorFx");
        CancelInvoke("CancelShockColorFx");
        InvokeRepeating("ShockColorFx", 0f, .3f);
        Invoke("CancelShockColorFx", _seconds);
    }

    public void IgniteFxFor(float _seconds)
    {
        CancelInvoke("IgniteColorFx");
        CancelInvoke("CancelIgniteColorFx");
        InvokeRepeating("IgniteColorFx", 0f, .3f);
        Invoke("CancelIgniteColorFx", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        CancelInvoke("ChillColorFx");
        CancelInvoke("CancelChillColorFx");
        InvokeRepeating("ChillColorFx", 0f, .3f);
        Invoke("CancelChillColorFx", _seconds);
    }

    private void ShockColorFx()
    {
        // 如果正在眩晕，直接退出
        if (IsInvoking("RedColorBlink"))
            return;
            
        shockColorState = !shockColorState;
        sr.color = shockColorState ? shockColor[0] : shockColor[1];
    }

    private void IgniteColorFx()
    {
        // 如果正在眩晕，直接退出
        if (IsInvoking("RedColorBlink"))
            return;
            
        igniteColorState = !igniteColorState;
        sr.color = igniteColorState ? igniteColor[0] : igniteColor[1];
    }

    private void ChillColorFx()
    {
        // 如果正在眩晕，直接退出
        if (IsInvoking("RedColorBlink"))
            return;
            
        chillColorState = !chillColorState;
        sr.color = chillColorState ? chillColor[0] : chillColor[1];
    }
    #endregion

    #region Cancel Methods
    public void CancelStunBlink()
    {
        CancelInvoke("RedColorBlink");
        stunColorState = false;
        sr.color = Color.white;
    }

    private void CancelShockColorFx()
    {
        CancelInvoke("ShockColorFx");
        shockColorState = false;
        sr.color = Color.white; 
    }

    private void CancelIgniteColorFx()
    {
        CancelInvoke("IgniteColorFx");
        igniteColorState = false;
        sr.color = Color.white; 
    }

    private void CancelChillColorFx()
    {
        CancelInvoke("ChillColorFx");
        chillColorState = false;
        sr.color = Color.white;
    }

    // 强制重置所有颜色状态的方法
    public void ForceResetAllColors()
    {
        CancelInvoke("RedColorBlink");
        CancelInvoke("IgniteColorFx");
        CancelInvoke("ChillColorFx");
        CancelInvoke("ShockColorFx");
        
        stunColorState = false;
        shockColorState = false;
        igniteColorState = false;
        chillColorState = false;
        
        sr.color = Color.white;
    }
    #endregion
}