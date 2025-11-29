using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;
    private int bgmIndex;

    public bool PlayBgm 
    { 
        get => playBgm; 
        set => playBgm = value; 
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBGM();
    }

    public void PlaySFX(int index)
    {
        if (sfx == null || index < 0 || index >= sfx.Length)
            return;

        if (sfx[index] != null)
            sfx[index].Play();
    }

    public void StopSFX(int index)
    {
        if (sfx == null || index < 0 || index >= sfx.Length)
            return;

        if (sfx[index] != null)
            sfx[index].Stop();
    }

    public void StopAllSFX()
    {
        if (sfx == null)
            return;

        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i] != null)
                sfx[i].Stop();
        }
    }

    public void PlaySFXLoop(int index)
    {
        if (sfx == null || index < 0 || index >= sfx.Length)
            return;

        if (sfx[index] != null)
        {
            sfx[index].loop = true;

            if (!sfx[index].isPlaying)
                sfx[index].Play();
        }
    }

    public void StopAllLoopSFX()
    {
        if (sfx == null)
            return;

        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i] != null && sfx[i].isPlaying && sfx[i].loop)
                sfx[i].Stop();
        }
    }

    public void PlayBGM(int index)
    {
        bgmIndex = index;

        StopAllBGM();

        if (bgm != null && index >= 0 && index < bgm.Length && bgm[index] != null)
        {
            bgm[index].loop = true;
            bgm[index].Play();
        }
    }

    public void PLayBGMContinue()
    {
        StopAllBGM();

        if (bgm != null && bgmIndex >= 0 && bgmIndex < bgm.Length && bgm[bgmIndex] != null)
        {
            bgm[bgmIndex].loop = true;
            bgm[bgmIndex].Play();
        }
    }

    public void StopAllBGM()
    {
        if (bgm == null)
            return;

        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i] != null)
                bgm[i].Stop();
        }
    }
}
