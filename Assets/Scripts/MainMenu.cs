using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Scenes/GameScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
