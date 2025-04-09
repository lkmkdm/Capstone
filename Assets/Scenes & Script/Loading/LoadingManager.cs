using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Firebase.Auth;

public class LoadingManager : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;
    public float fakeLoadingTime = 2f;
    public Color fillColor = new Color(0, 1, 1, 1); // 청록색

    private Image fillImage;

    void Start()
    {
        fillImage = loadingBar.fillRect.GetComponent<Image>();
        StartCoroutine(LoadSceneAsync(LoadingInfo.nextSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        float fakeDuration = fakeLoadingTime;
        bool willLoadScene = !string.IsNullOrEmpty(sceneName);

        AsyncOperation operation = null;
        if (willLoadScene)
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
        }

        // Firebase 사용자 정보 가져오기 (로딩 중 병렬 실행)
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
            Debug.Log("현재 사용자: " + user.Email);

        float elapsed = 0f;
        float progress = 0f;

        while ((elapsed < fakeDuration) || (willLoadScene && operation.progress < 0.9f))
        {
            elapsed += Time.deltaTime;

            // 실제 로딩 진행도
            float realProgress = willLoadScene ? Mathf.Clamp01(operation.progress / 0.9f) : 1f;

            // 슬라이더에 적용할 가짜+진짜 섞인 값
            progress = Mathf.Clamp01(elapsed / fakeDuration);
            progress = Mathf.Min(progress, realProgress); // 둘 중 느린 쪽 기준

            loadingBar.value = progress;
            loadingText.text = $"로딩 중... {progress * 100:F0}%";
            fillImage.color = Color.Lerp(new Color(0, 1, 1, 0.3f), fillColor, progress);

            yield return null;
        }

        if (willLoadScene)
            operation.allowSceneActivation = true;
        else
            loadingText.text = "로딩 완료!";
    }

}

// 전역적으로 다음 씬 이름을 저장하는 클래스
public static class LoadingInfo
{
    public static string nextSceneName = ""; // 비어 있으면 씬 전환 없음
}
/* 호출 방법
void GoToResultScene()
{
    LoadingInfo.nextSceneName = "ResultScene"; // 다음 씬의 이름 설정
    SceneManager.LoadScene("LoadingScene");    // 로딩 씬으로 먼저 이동
}
*/
