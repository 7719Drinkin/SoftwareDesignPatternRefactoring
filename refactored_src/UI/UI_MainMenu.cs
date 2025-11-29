using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";

    [SerializeField] private GameObject continueButton;

    [SerializeField] private UI_FadeOut fadeOut;

    private ISaveManagerService saveManager;

    private void Start()
    {
        saveManager = ServiceLocator.Instance.Get<ISaveManagerService>();
        
        if (saveManager != null && !saveManager.HaveSaveData())
            continueButton.SetActive(false);

        ServiceLocator.Instance.Get<IAudioManager>().PlayBGM(0);
    }

    public void CountinueGame()
    {
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void NewGame()
    {
        if (saveManager != null)
            saveManager.DeleteSaveFile();
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void ExitGame()
    {
        ServiceLocator.Instance.Get<IAudioManager>().PlaySFX(24);
    }

    IEnumerator LoadSceneWithFadeOut(float delay)
    {
        fadeOut.FadeOut();

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}
