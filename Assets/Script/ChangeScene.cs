using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GameList()
    {
        SceneManager.LoadScene("GameList");
    }
    
    public void Game_1()
    {
        SceneManager.LoadScene("Game_1");
    }

    // Scene -> AdhdTest Scene
    public void GoToAdhdTest()
    {
        SceneManager.LoadScene("AdhdTest");
    }

    // Scene -> ServiceContent Scene
    public void GoToServiceContent()
    {
        SceneManager.LoadScene("ServiceContent");
    }

    // Scene -> GoogleSignIn Scene
    public void GoToGoogleSignIn()
    {
        SceneManager.LoadScene("GoogleSignIn");
    }

}
