using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseFailManager : MonoBehaviour
{
    public static PauseFailManager instance;
    public CanvasGroup pauseMenu;
    public CanvasGroup failMenu;
    public float fadeDuration = 0.3f;

    private bool isPaused = false;
    private bool isFailed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        pauseMenu.alpha = 0f;
        failMenu.alpha = 0f;
        pauseMenu.gameObject.SetActive(false);
        failMenu.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isFailed)
        {
            if (isPaused)
                StartCoroutine(FadeOutUI(pauseMenu, () => { ResumeGame(); }));
            else
                StartCoroutine(FadeInUI(pauseMenu, () => { PauseGame(); }));
        }
    }


    IEnumerator FadeInUI(CanvasGroup cg, System.Action onShown = null)
    {
        cg.gameObject.SetActive(true);
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 1f;
        onShown?.Invoke();
    }

    IEnumerator FadeOutUI(CanvasGroup cg, System.Action onHidden = null)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        cg.alpha = 0f;
        cg.gameObject.SetActive(false);
        onHidden?.Invoke();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void TriggerFail()
    {
        if (!isFailed)
        {
            isFailed = true;
            StartCoroutine(FadeInUI(failMenu, () =>
            {
                Time.timeScale = 0f;
            }));
        }
    }

    public void OnResumeButton()
    {
        StartCoroutine(FadeOutUI(pauseMenu, () => { ResumeGame(); }));
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        DynamicGI.UpdateEnvironment();
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
