using TMPro;
using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour
{
    public TMP_Text timeText;
    public float gameDuration = 60f; // ��ü ���� �ð� (��)

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
            if (Time.timeScale > 0f) // �Ͻ����� ���� �ƴ� ���� �ð� ����
            {
                remainingTime -= Time.deltaTime;
                timeText.text = $"���� �ð�: {remainingTime:F1}��";
            }
            yield return null;
        }

        if (remainingTime <= 0f)
        {
            timeText.text = "���� �ð�: 0.0��";
            GameOver();
        }
    }

    void GameOver()
    {
        isRunning = false;
        // ���⿡ ��� ����, UI ��ȯ, ��ư ��Ȱ��ȭ �� �߰� ����
    }

    public void StopTimer() // �ܺο��� �������� ���߰� ���� �� ���
    {
        isRunning = false;
    }
}
