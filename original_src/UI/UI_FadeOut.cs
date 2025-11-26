using UnityEngine;

public class UI_FadeOut : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void FadeOut()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        // 使用受Time.timeScale影响的更新（正常时间）
        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.SetTrigger("fadeOut");
    }

    public void FadeIn()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        // 使用不受Time.timeScale影响的更新（不受暂停影响）
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        anim.SetTrigger("fadeIn");
    }
}
