using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AnalyzingManager : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public float duration = 7f; // 7초 동안 분석 진행

    private float timer = 0f;

    void Start()
    {
        // 7초 뒤 씬 전환
        Invoke("LoadUserInfoScene", duration);
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            int percent = Mathf.Clamp(Mathf.RoundToInt((timer / duration) * 100), 0, 100);
            loadingText.text = $"분석 중입니다...{percent}%";
        }
    }

    void LoadUserInfoScene()
    {
        SceneManager.LoadScene("UserInfo");
    }
}
