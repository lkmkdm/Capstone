using UnityEngine;
using UnityEngine.SceneManagement;

public class BarLoader : MonoBehaviour
{
    void Awake()
    {
        // BottomBar 씬이 이미 로드되지 않았다면 추가 로드
        if (!SceneManager.GetSceneByName("BottomBar").isLoaded)
        {
            SceneManager.LoadScene("BottomBar", LoadSceneMode.Additive);
        }
    }

    public void LoadScene(string sceneName)
    {
        // 씬을 전환할 때 기존 씬은 Single 모드로 전환
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        // 씬 변경 후 BottomBar 씬이 로드되지 않으면 추가로 로드
        if (!SceneManager.GetSceneByName("BottomBar").isLoaded)
        {
            SceneManager.LoadScene("BottomBar", LoadSceneMode.Additive);
        }
    }
}
