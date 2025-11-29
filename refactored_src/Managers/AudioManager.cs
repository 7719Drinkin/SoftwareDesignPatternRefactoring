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
        if (index < sfx.Length)
            sfx[index].Play();
    }

    public void StopSFX(int index)
    {
        if (index < sfx.Length)
            sfx[index].Stop();
    }

    public void StopAllSFX()
    {
        for (int i = 0; i < sfx.Length; i++)
            sfx[i].Stop();
    }

    public void PlaySFXLoop(int index)
    {
        if (index < sfx.Length)
        {
            sfx[index].loop = true;

            if (!sfx[index].isPlaying)
                sfx[index].Play();
        }
    }

    public void StopAllLoopSFX()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i].isPlaying && sfx[i].loop)
                sfx[i].Stop();
        }
    }

    public void PlayBGM(int index)
    {
        bgmIndex = index;

        StopAllBGM();

        if (index < bgm.Length)
        {
            bgm[index].loop = true;
            bgm[index].Play();
        }
    }

    public void PLayBGMContinue()
    {
        StopAllBGM();

        bgm[bgmIndex].loop = true;
        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
            bgm[i].Stop();
    }
}
