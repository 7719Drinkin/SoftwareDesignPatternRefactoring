using System.Collections;
using TMPro;
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

    [Header("Ailment particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField] private GameObject criticalHitFXPrefab;

    [Header("Pop up")]
    [SerializeField] private GameObject popUpTextPrefab;

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

        shockFx.Play();

        InvokeRepeating("ShockColorFx", 0f, .3f);
        Invoke("CancelShockColorFx", _seconds);
    }

    public void IgniteFxFor(float _seconds)
    {
        CancelInvoke("IgniteColorFx");
        CancelInvoke("CancelIgniteColorFx");

        igniteFx.Play();

        InvokeRepeating("IgniteColorFx", 0f, .3f);
        Invoke("CancelIgniteColorFx", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        CancelInvoke("ChillColorFx");
        CancelInvoke("CancelChillColorFx");

        chillFx.Play();

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
        shockFx.Stop();
    }

    private void CancelIgniteColorFx()
    {
        CancelInvoke("IgniteColorFx");
        igniteColorState = false;
        sr.color = Color.white;
        igniteFx.Stop();
    }

    private void CancelChillColorFx()
    {
        CancelInvoke("ChillColorFx");
        chillColorState = false;
        sr.color = Color.white;
        chillFx.Stop();
    }

    // 强制重置所有颜色状态的方法
    public void ForceResetAllColors()
    {
        CancelStunBlink();
        CancelShockColorFx();
        CancelIgniteColorFx();
        CancelChillColorFx();
        
        sr.color = Color.white;
    }
    #endregion

    public void CreateHitFX(Transform target, bool critical)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);
        Vector3 hitFXRotation = new Vector3(0, 0, zRotation);

        GameObject hitFXPrefabitFX = hitFXPrefab;
        if (critical)
        {
            hitFXPrefabitFX = criticalHitFXPrefab;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (target.GetComponent<Entity>().facingDir == 1)
                yRotation = 180;

            hitFXRotation = new Vector3(0, yRotation, zRotation);
        }

        GameObject newHitFX = Instantiate(hitFXPrefabitFX, target.position + new Vector3(xPosition, yPosition), Quaternion.identity);
        newHitFX.transform.Rotate(hitFXRotation);

        Destroy(newHitFX, .5f);
    }

    public void CreatePopUpText(string text, bool canCrit = false)
    {
        if (popUpTextPrefab == null)
            return;

        float randomx = Random.Range(-0.5f, 0.5f);
        float randomy = Random.Range(0.5f, 1.2f);

        Vector3 positionOffset = new Vector3(randomx, randomy, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        var tmp = newText.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = text;
            if (canCrit)
            {
                tmp.color = Color.red;
                tmp.fontSize *= 1.5f;
            }
        }
    }
}