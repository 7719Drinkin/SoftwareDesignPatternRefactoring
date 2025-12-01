using UnityEngine;
using TMPro;
using System.Collections;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float upSpeed;
    [SerializeField] private float colorDisappearanceSpeed;

    [SerializeField] private float lifeTime;
    private float textTimer;

    void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 1), upSpeed * Time.deltaTime);

        textTimer -= Time.deltaTime;

        if (textTimer < 0)
        {
            float alpha = myText.color.a - colorDisappearanceSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);
            StartCoroutine(DestroyMe());
        }
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
