using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";

    [SerializeField] private GameObject continueButton;

    [SerializeField] private UI_FadeOut fadeOut;

    private void Start()
    {
        if (!SaveManager.instance.HaveSaveData())
            continueButton.SetActive(false);

        AudioManager.instance.PlayBGM(0);
    }

    public void CountinueGame()
    {
        AudioManager.instance.PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveFile();
        AudioManager.instance.PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void ExitGame()
    {
        AudioManager.instance.PlaySFX(24);
    }

    IEnumerator LoadSceneWithFadeOut(float delay)
    {
        fadeOut.FadeOut();

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}
