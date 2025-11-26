using UnityEngine;
using TMPro;
using System.Collections;

public class UI_PopUpText : MonoBehaviour
{
    private TextMeshProUGUI myText;

    [SerializeField] private float upSpeed;
    [SerializeField] private float colorDisappearanceSpeed;

    [SerializeField] private float lifeTime;
    private float textTimer;

    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
        textTimer = lifeTime;
    }

    void Update()
    {
        // 用未缩放时间，暂停时也能播放
        float dt = Time.unscaledDeltaTime;
        
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), upSpeed * dt);

        textTimer -= dt;

        if (textTimer < 0)
        {
            float alpha = myText.color.a - colorDisappearanceSpeed * dt;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);
            StartCoroutine(DestroyMe());
        }
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (this != null) 
            Destroy(gameObject);
    }
}
