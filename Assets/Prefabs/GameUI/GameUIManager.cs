using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject pausePanel;

    public void OnClickPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
    }

    public void OnClickResume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }

    public void OnClickHome()
    {
        Time.timeScale = 0.5f; // 씬 이동 전 시간 정상화
        LoadingInfo.nextSceneName = "MainScreen"; // 메인 홈 씬 이름
        SceneManager.LoadScene("Loading"); // 로딩 씬으로 이동
    }
}
