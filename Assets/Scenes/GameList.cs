using UnityEngine;
using UnityEngine.SceneManagement;

public class GameList : MonoBehaviour
{
    public void ReturnToGameList()
    {
        SceneManager.LoadScene("GameList");
    }
}
