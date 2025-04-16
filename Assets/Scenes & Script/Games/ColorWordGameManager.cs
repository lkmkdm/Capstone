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
            (leftText, true),  // ����
            (centerText, false), // �ѱ�
            (rightText, false)   // �ѱ�
        };

        // ���� ��ġ ����
        correctIndex = Random.Range(0, options.Count);
        ColorData correctColor = colorList[Random.Range(0, colorList.Count)];

        // ���� �ؽ�Ʈ ����
        var correctOption = options[correctIndex];
        correctOption.text.text = correctOption.isEnglish ? correctColor.colorNameEng : correctColor.colorNameKor;
        correctOption.text.color = correctColor.colorValue;

        // ���� ����: �ؽ�Ʈ�� ������ ��ġ���� �ʵ���
        for (int i = 0; i < options.Count; i++)
        {
            if (i == correctIndex) continue;

            TMP_Text textComp = options[i].text;
            bool isEng = options[i].isEnglish;

            int textIdx, colorIdx;

            // �ؽ�Ʈ �ε��� ����
            do
            {
                textIdx = Random.Range(0, colorList.Count);
            } while (textIdx == colorList.IndexOf(correctColor)); // ����� ���� �ؽ�Ʈ ���ϱ�

            // ���� �ε��� ���� (�ؽ�Ʈ�� ��ġ���� �ʰ�)
            do
            {
                colorIdx = Random.Range(0, colorList.Count);
            } while (colorIdx == textIdx || colorIdx == colorList.IndexOf(correctColor));

            textComp.text = isEng ? colorList[textIdx].colorNameEng : colorList[textIdx].colorNameKor;
            textComp.color = colorList[colorIdx].colorValue;
        }

        // ����� �α�
        Debug.Log($"���� ��ġ: {correctIndex}, �ؽ�Ʈ: {correctOption.text.text}, ����: {correctColor.colorValue}");
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
