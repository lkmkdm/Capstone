using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AnalyzingManager : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float duration = 7f; // 7�� ���� �м� ����

    private float timer = 0f;

    void Start()
    {
        // 7�� �� �� ��ȯ
        Invoke("LoadUserInfoScene", duration);
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            int percent = Mathf.Clamp(Mathf.RoundToInt((timer / duration) * 100), 0, 100);
            loadingText.text = $"�м� ���Դϴ�...{percent}%";
        }
    }

    void LoadUserInfoScene()
    {
        SceneManager.LoadScene("UserInfo");
    }
}
