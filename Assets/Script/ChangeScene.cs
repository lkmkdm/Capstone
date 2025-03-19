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
}
