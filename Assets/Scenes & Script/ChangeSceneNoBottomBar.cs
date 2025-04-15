using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneNoBottomBar : MonoBehaviour
{
    public void CreateAccount()
    {
        SceneHistory.PreviousSceneName = SceneManager.GetActiveScene().name;
        LoadingScreenController.Instance?.LoadScene("CreateAccount");
    }

    public void Login()
    {
        SceneHistory.PreviousSceneName = SceneManager.GetActiveScene().name;
        LoadingScreenController.Instance?.LoadScene("Login");
    }
}
