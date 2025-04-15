using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private void Start()
    {
        // 이 버튼이 눌리면 GoBack 실행
        GetComponent<Button>().onClick.AddListener(GoBack);
    }

    public void GoBack()
    {
        if (!string.IsNullOrEmpty(SceneHistory.PreviousSceneName))
        {
            SceneManager.LoadScene(SceneHistory.PreviousSceneName);
        }
        else
        {
            Debug.LogWarning("이전 씬 정보가 없습니다!");
        }
    }
}
