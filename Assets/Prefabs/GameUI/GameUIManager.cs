using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject pausePanel;

    public void OnClickPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // ���� �Ͻ�����
    }

    public void OnClickResume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // ���� �簳
    }

    public void OnClickHome()
    {
        Time.timeScale = 0.5f; // �� �̵� �� �ð� ����ȭ
        LoadingInfo.nextSceneName = "MainScreen"; // ���� Ȩ �� �̸�
        SceneManager.LoadScene("Loading"); // �ε� ������ �̵�
    }
}
