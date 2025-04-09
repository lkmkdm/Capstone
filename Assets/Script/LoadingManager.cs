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
    public Color fillColor = new Color(0, 1, 1, 1); // û�ϻ�

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

        // Firebase ����� ���� �������� (�ε� �� ���� ����)
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
            Debug.Log("���� �����: " + user.Email);

        float elapsed = 0f;
        float progress = 0f;

        while ((elapsed < fakeDuration) || (willLoadScene && operation.progress < 0.9f))
        {
            elapsed += Time.deltaTime;

            // ���� �ε� ���൵
            float realProgress = willLoadScene ? Mathf.Clamp01(operation.progress / 0.9f) : 1f;

            // �����̴��� ������ ��¥+��¥ ���� ��
            progress = Mathf.Clamp01(elapsed / fakeDuration);
            progress = Mathf.Min(progress, realProgress); // �� �� ���� �� ����

            loadingBar.value = progress;
            loadingText.text = $"�ε� ��... {progress * 100:F0}%";
            fillImage.color = Color.Lerp(new Color(0, 1, 1, 0.3f), fillColor, progress);

            yield return null;
        }

        if (willLoadScene)
            operation.allowSceneActivation = true;
        else
            loadingText.text = "�ε� �Ϸ�!";
    }

}

// ���������� ���� �� �̸��� �����ϴ� Ŭ����
public static class LoadingInfo
{
    public static string nextSceneName = ""; // ��� ������ �� ��ȯ ����
}
/* ȣ�� ���
void GoToResultScene()
{
    LoadingInfo.nextSceneName = "ResultScene"; // ���� ���� �̸� ����
    SceneManager.LoadScene("LoadingScene");    // �ε� ������ ���� �̵�
}
*/
