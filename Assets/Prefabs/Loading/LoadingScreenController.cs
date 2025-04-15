using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingPanel;    // 전체 로딩 패널 (항상 켜져 있음)
    public GameObject visualGroup;     // 내부 UI만 보여주고 숨김 처리

    private static LoadingScreenController instance;
    public static LoadingScreenController Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // ✅ 로딩창 UI 보여줌 (DotContainer는 항상 돌아가고 있음)
        if (visualGroup != null)
            visualGroup.SetActive(true);

        // ✅ UI가 렌더될 시간 확보 (점 애니메이션도 바로 시작)
        yield return new WaitForSeconds(0.3f);

        // ✅ 씬 로딩 시작
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
            yield return null;

        // ✅ 씬 로딩 완료되면 즉시 로딩창 UI 닫기
        if (visualGroup != null)
            visualGroup.SetActive(false);
    }
}
