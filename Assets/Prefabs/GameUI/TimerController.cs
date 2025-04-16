using TMPro;
using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour
{
    public TMP_Text timeText;
    public float gameDuration = 60f; // 전체 게임 시간 (초)

    private float remainingTime;
    private bool isRunning = true;

    void Start()
    {
        remainingTime = gameDuration;
        StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        while (isRunning && remainingTime > 0f)
        {
            if (Time.timeScale > 0f) // 일시정지 중이 아닐 때만 시간 감소
            {
                remainingTime -= Time.deltaTime;
                timeText.text = $"남은 시간: {remainingTime:F1}초";
            }
            yield return null;
        }

        if (remainingTime <= 0f)
        {
            timeText.text = "남은 시간: 0.0초";
            GameOver();
        }
    }

    void GameOver()
    {
        isRunning = false;
        // 여기에 결과 저장, UI 전환, 버튼 비활성화 등 추가 가능
    }

    public void StopTimer() // 외부에서 수동으로 멈추고 싶을 때 사용
    {
        isRunning = false;
    }
}
