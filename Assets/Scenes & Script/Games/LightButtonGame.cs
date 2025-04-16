using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightButtonGame : MonoBehaviour
{
    public Button[] buttons; // 9개의 TMP 버튼
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public GameObject resultPanel;  // 결과 패널 (UI)
    public TextMeshProUGUI finalScoreText;  // 결과 패널의 최종 점수 텍스트

    private List<int> lightSequence = new List<int>(); // 불빛 시퀀스
    private int inputIndex = 0;
    private int score = 0;
    private float timeLeft = 60f;  // 타이머를 60초로 설정
    private bool acceptingInput = false;

    void Start()
    {
        scoreText.text = "Score: 0";
        resultPanel.SetActive(false);  // 게임 시작 시 결과 패널 숨기기
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;  // 타이머 감소
            timerText.text = "Time: " + timeLeft.ToString("F1");  // 타이머 출력

            // 사용자가 입력을 받지 않고 있을 때만 불빛 시퀀스를 실행
            if (!acceptingInput)
            {
                // 불빛 시퀀스를 새로 시작
                yield return StartCoroutine(ShowLights());
            }

            yield return null;
        }

        // 타이머가 끝난 후 결과 패널을 활성화하고 최종 점수 표시
        ShowResultPanel();
    }

    IEnumerator ShowLights()
    {
        lightSequence.Clear();  // 새 라운드를 위해 시퀀스를 초기화

        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 6; i++)  // 6개의 불빛 시퀀스
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, buttons.Length);  // 버튼 중 하나 랜덤 선택
            } while (usedIndexes.Contains(randomIndex));  // 중복된 버튼을 피하기 위해

            usedIndexes.Add(randomIndex);
            lightSequence.Add(randomIndex);  // 불빛 시퀀스에 추가

            // 버튼에 불빛 효과를 켜고 끄는 부분
            buttons[randomIndex].GetComponent<ButtonLightEffect>().TurnOnLight();
            yield return new WaitForSeconds(0.5f);
            buttons[randomIndex].GetComponent<ButtonLightEffect>().TurnOffLight();
            yield return new WaitForSeconds(0.2f);
        }

        // 버튼에 클릭 이벤트 연결
        acceptingInput = true;  // 버튼 클릭을 받을 수 있도록 설정
        inputIndex = 0;  // 라운드 시작 시 입력 인덱스는 0으로 초기화

        // 버튼 클릭 이벤트 설정
        for (int i = 0; i < buttons.Length; i++)
        {
            int btnIndex = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnButtonPressed(btnIndex));
        }
    }

    void OnButtonPressed(int index)
    {
        if (!acceptingInput || timeLeft <= 0f) return;  // 게임이 끝났거나 입력을 받을 수 없으면 아무 동작도 하지 않음

        if (index == lightSequence[inputIndex])  // 올바른 버튼을 눌렀으면
        {
            inputIndex++;
            if (inputIndex >= lightSequence.Count)  // 6개의 버튼 모두 맞췄으면
            {
                score++;  // 점수 증가
                scoreText.text = "Score: " + score;  // 점수 화면에 업데이트
                acceptingInput = false;  // 입력을 받지 않도록 설정

                // 일정 시간 후 다음 라운드를 시작하도록 delay를 줄 수 있습니다
                StartCoroutine(NextRoundDelay());
            }
        }
        else  // 틀린 버튼을 눌렀으면
        {
            inputIndex = 0;  // 입력 인덱스를 처음으로 되돌려서 다시 시작
            acceptingInput = false;  // 입력을 받지 않도록 설정

            // 틀린 경우 바로 새로운 라운드를 시작
            StartCoroutine(NextRoundDelay());
        }
    }

    IEnumerator NextRoundDelay()
    {
        // 틀린 경우나 맞춘 경우 모두 잠시 기다렸다가 다음 라운드를 시작
        yield return new WaitForSeconds(1f);  // 1초 대기 후 불빛 시퀀스 시작
        acceptingInput = false;  // 다음 라운드를 위해 입력을 막음
    }

    void ShowResultPanel()
    {
        // 게임 종료 후 결과 패널을 띄우고 최종 점수 표시
        resultPanel.SetActive(true);  // 결과 패널 보이기
        finalScoreText.text = "Final Score: " + score.ToString();  // 최종 점수 표시
    }
}
