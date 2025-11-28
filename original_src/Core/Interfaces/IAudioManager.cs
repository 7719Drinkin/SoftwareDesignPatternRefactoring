/// <summary>
/// 音频管理器接口 - 定义所有音频相关操作
/// </summary>
public interface IAudioManager
{
    void PlaySFX(int index);
    void StopSFX(int index);
    void StopAllSFX();
    void PlaySFXLoop(int index);
    void StopAllLoopSFX();
    void PlayBGM(int index);
    void PLayBGMContinue();
    void StopAllBGM();
    bool PlayBgm { get; set; }
}
