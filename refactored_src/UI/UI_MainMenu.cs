using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";

    [SerializeField] private GameObject continueButton;

    [SerializeField] private UI_FadeOut fadeOut;

    // ========== 使用 Facade Pattern 简化服务访问 ==========
    private GameFacade game => GameFacade.Instance;

    private void Start()
    {
        if (!game.Save.HaveSaveData())
            continueButton.SetActive(false);

        game.PlayBGM(0);
    }

    public void CountinueGame()
    {
        game.PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void NewGame()
    {
        game.Save.DeleteSaveFile();
        game.PlaySFX(24);
        StartCoroutine(LoadSceneWithFadeOut(2));
    }

    public void ExitGame()
    {
        game.PlaySFX(24);
    }

    IEnumerator LoadSceneWithFadeOut(float delay)
    {
        fadeOut.FadeOut();

        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}
