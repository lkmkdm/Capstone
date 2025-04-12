using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneNoBottomBar : MonoBehaviour
{
    // 씬 이름에 따라 원하는 씬으로 전환할 수 있는 함수들

    public void CreateAccount()
    {
        SceneManager.LoadScene("CreateAccount");
    }

    public void Login()
    {
        SceneManager.LoadScene("Login");
    }

    // 필요에 따라 계속 추가 가능
}
