using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorWordGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public Button leftButton;
    public Button centerButton;
    public Button rightButton;
    public TMP_Text leftText;
    public TMP_Text centerText;
    public TMP_Text rightText;
    public GameObject resultPanel;
    public TMP_Text finalScoreText;

    [Header("Color Data")]
    public List<ColorData> colorList;

    private int score = 0;
    private float timeRemaining = 60f;
    private int correctIndex = 0;

    void Start()
    {
        leftButton.onClick.AddListener(() => CheckAnswer(0));
        centerButton.onClick.AddListener(() => CheckAnswer(1));
        rightButton.onClick.AddListener(() => CheckAnswer(2));
        ShowNextQuestion();
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();
        }
        else
        {
            EndGame();
        }
    }

    void CheckAnswer(int clickedIndex)
    {
        if (clickedIndex == correctIndex)
            score++;

        scoreText.text = "Score: " + score;
        ShowNextQuestion();
    }

    void ShowNextQuestion()
    {
        List<(TMP_Text text, bool isEnglish)> options = new()
        {
            (leftText, true),  // 영어
            (centerText, false), // 한글
            (rightText, false)   // 한글
        };

        // 정답 위치 선택
        correctIndex = Random.Range(0, options.Count);
        ColorData correctColor = colorList[Random.Range(0, colorList.Count)];

        // 정답 텍스트 설정
        var correctOption = options[correctIndex];
        correctOption.text.text = correctOption.isEnglish ? correctColor.colorNameEng : correctColor.colorNameKor;
        correctOption.text.color = correctColor.colorValue;

        // 오답 설정: 텍스트와 색상이 일치하지 않도록
        for (int i = 0; i < options.Count; i++)
        {
            if (i == correctIndex) continue;

            TMP_Text textComp = options[i].text;
            bool isEng = options[i].isEnglish;

            int textIdx, colorIdx;

            // 텍스트 인덱스 선택
            do
            {
                textIdx = Random.Range(0, colorList.Count);
            } while (textIdx == colorList.IndexOf(correctColor)); // 정답과 같은 텍스트 피하기

            // 색상 인덱스 선택 (텍스트와 일치하지 않게)
            do
            {
                colorIdx = Random.Range(0, colorList.Count);
            } while (colorIdx == textIdx || colorIdx == colorList.IndexOf(correctColor));

            textComp.text = isEng ? colorList[textIdx].colorNameEng : colorList[textIdx].colorNameKor;
            textComp.color = colorList[colorIdx].colorValue;
        }

        // 디버깅 로그
        Debug.Log($"정답 위치: {correctIndex}, 텍스트: {correctOption.text.text}, 색상: {correctColor.colorValue}");
    }

    void EndGame()
    {
        leftButton.interactable = false;
        centerButton.interactable = false;
        rightButton.interactable = false;
        resultPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + score;
    }
}

[System.Serializable]
public class ColorData
{
    public string colorNameEng;
    public string colorNameKor;
    public Color colorValue;
}
