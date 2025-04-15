using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonClickSoundInstaller : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Button[] allButtons = FindObjectsOfType<Button>(true); // 비활성 포함
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayClickSound();
                }
            });
        }
    }
}
