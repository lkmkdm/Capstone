using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShapeGameManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text questionText;

    public Button circleButton;
    public Button squareButton;
    public Button triangleButton;
    public Button starButton;

    public GameObject resultPanel;
    public TMP_Text finalScoreText;

    public Transform buttonParent; // 도형 버튼들을 담고 있는 부모 오브젝트 (예: Horizontal Layout Group)

    private float timeRemaining = 60f;
    private int score = 0;
    private string currentAnswer;

    private List<string> shapeNames = new() { "원", "네모", "삼각형", "별" };

    private Dictionary<string, string> questionDisplayNames = new()
    {
        { "원", "동그라미" },
        { "네모", "네모" },
        { "삼각형", "세모" },
        { "별", "별" }
    };

    private List<Button> shapeButtons;

    void Start()
    {
        shapeButtons = new List<Button> { circleButton, squareButton, triangleButton, starButton };

        circleButton.onClick.AddListener(() => CheckAnswer("원"));
        squareButton.onClick.AddListener(() => CheckAnswer("네모"));
        triangleButton.onClick.AddListener(() => CheckAnswer("삼각형"));
        starButton.onClick.AddListener(() => CheckAnswer("별"));

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

    void ShowNextQuestion()
    {
        // 정답 설정
        int randomIndex = Random.Range(0, shapeNames.Count);
        currentAnswer = shapeNames[randomIndex];

        // 질문 텍스트 한글 스타일로 표시
        questionText.text = questionDisplayNames[currentAnswer];

        // 버튼 순서 랜덤화
        ShuffleButtons();
    }

    void CheckAnswer(string selected)
    {
        if (selected == currentAnswer)
            score++;

        scoreText.text = "Score: " + score;
        ShowNextQuestion();
    }

    void EndGame()
    {
        foreach (Button btn in shapeButtons)
        {
            btn.interactable = false;
        }

        resultPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + score;
    }

    void ShuffleButtons()
    {
        // 버튼 순서 랜덤 섞기
        List<Button> shuffled = new List<Button>(shapeButtons);
        for (int i = 0; i < shuffled.Count; i++)
        {
            Button temp = shuffled[i];
            int randIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        // 버튼 부모에서 위치 재배치
        for (int i = 0; i < shuffled.Count; i++)
        {
            shuffled[i].transform.SetSiblingIndex(i);
        }

        // 레이아웃 강제 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)buttonParent);
    }
}
