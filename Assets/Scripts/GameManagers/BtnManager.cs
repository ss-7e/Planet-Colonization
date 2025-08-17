using UnityEngine;
using UnityEngine.SceneManagement;  

public class BtnManager : MonoBehaviour
{
    Scene currentScene;
    public void QuitGame()
    {
        Application.Quit();    
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        DynamicGI.UpdateEnvironment();
    }
}
