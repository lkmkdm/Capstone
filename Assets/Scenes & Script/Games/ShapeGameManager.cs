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

    public Transform buttonParent; // ���� ��ư���� ��� �ִ� �θ� ������Ʈ (��: Horizontal Layout Group)

    private float timeRemaining = 60f;
    private int score = 0;
    private string currentAnswer;

    private List<string> shapeNames = new() { "��", "�׸�", "�ﰢ��", "��" };

    private Dictionary<string, string> questionDisplayNames = new()
    {
        { "��", "���׶��" },
        { "�׸�", "�׸�" },
        { "�ﰢ��", "����" },
        { "��", "��" }
    };

    private List<Button> shapeButtons;

    void Start()
    {
        shapeButtons = new List<Button> { circleButton, squareButton, triangleButton, starButton };

        circleButton.onClick.AddListener(() => CheckAnswer("��"));
        squareButton.onClick.AddListener(() => CheckAnswer("�׸�"));
        triangleButton.onClick.AddListener(() => CheckAnswer("�ﰢ��"));
        starButton.onClick.AddListener(() => CheckAnswer("��"));

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
        // ���� ����
        int randomIndex = Random.Range(0, shapeNames.Count);
        currentAnswer = shapeNames[randomIndex];

        // ���� �ؽ�Ʈ �ѱ� ��Ÿ�Ϸ� ǥ��
        questionText.text = questionDisplayNames[currentAnswer];

        // ��ư ���� ����ȭ
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
        // ��ư ���� ���� ����
        List<Button> shuffled = new List<Button>(shapeButtons);
        for (int i = 0; i < shuffled.Count; i++)
        {
            Button temp = shuffled[i];
            int randIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randIndex];
            shuffled[randIndex] = temp;
        }

        // ��ư �θ𿡼� ��ġ ���ġ
        for (int i = 0; i < shuffled.Count; i++)
        {
            shuffled[i].transform.SetSiblingIndex(i);
        }

        // ���̾ƿ� ���� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)buttonParent);
    }
}
