using UnityEngine;
using System.Collections;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeSpeed = 2f;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(Sprite sprite, Vector3 position, bool facingRight, float alpha = 0.5f)
    {
        transform.position = position;
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = !facingRight;
        originalColor = spriteRenderer.color;
        originalColor.a = alpha;
        spriteRenderer.color = originalColor;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (spriteRenderer.color.a > 0)
        {
            Color color = spriteRenderer.color;
            color.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = color;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
