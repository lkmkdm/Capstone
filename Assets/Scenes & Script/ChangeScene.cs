using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // BottomBar 씬이 로드되지 않았다면 로드
        if (!SceneManager.GetSceneByName("BottomBar").isLoaded)
        {
            SceneManager.LoadScene("BottomBar", LoadSceneMode.Additive);
        }

        // 새로운 씬을 로드하되 BottomBar는 유지
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void GoToMainScreen()
    {
        LoadScene("MainScreen");
    }

    // 각 버튼에 연결된 씬 로드 함수들
    public void GameList()
    {
        LoadScene("GameList");
    }

    public void Game_1()
    {
        LoadScene("Game_1");
    }

    public void GoToAdhdTest()
    {
        LoadScene("AdhdTest");
    }

    public void GoToServiceContent()
    {
        LoadScene("ServiceContent");
    }

    public void GoToLogin()
    {
        LoadScene("Login");
    }
}
