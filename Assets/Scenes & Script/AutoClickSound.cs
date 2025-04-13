using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoClickSound : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClickSound();
            }
        });
    }
}
