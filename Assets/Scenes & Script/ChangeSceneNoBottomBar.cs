using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneNoBottomBar : MonoBehaviour
{
    public void CreateAccount()
    {
        // 현재 씬 이름을 저장
        SceneHistory.PreviousSceneName = SceneManager.GetActiveScene().name;

        // 씬 전환
        SceneManager.LoadScene("CreateAccount");
    }

    public void Login()
    {
        // 마찬가지로 현재 씬 이름을 저장
        SceneHistory.PreviousSceneName = SceneManager.GetActiveScene().name;

        // 씬 전환
        SceneManager.LoadScene("Login");
    }

    // 필요에 따라 계속 추가 가능
}