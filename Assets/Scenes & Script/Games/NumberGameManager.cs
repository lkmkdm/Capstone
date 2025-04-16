using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberGameManager : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text leftNumberText;
    public TMP_Text rightNumberText;
    public Button leftButton;
    public Button rightButton;

    public GameObject resultPanel;
    public TMP_Text finalScoreText;

    private int score = 0;
    private float timeRemaining = 60f;

    private int leftNumber;
    private int rightNumber;

    void Start()
    {
        GenerateNewNumbers();
        UpdateUI();

        leftButton.onClick.AddListener(() => OnNumberClick(leftNumber, rightNumber));
        rightButton.onClick.AddListener(() => OnNumberClick(rightNumber, leftNumber));
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

    void OnNumberClick(int selected, int other)
    {
        if (selected > other)
            score++;

        UpdateUI();
        GenerateNewNumbers();
    }

    void GenerateNewNumbers()
    {
        do
        {
            leftNumber = Random.Range(1, 101);
            rightNumber = Random.Range(1, 101);
        } while (leftNumber == rightNumber);

        leftNumberText.text = leftNumber.ToString();
        rightNumberText.text = rightNumber.ToString();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    void EndGame()
    {
        leftButton.interactable = false;
        rightButton.interactable = false;

        resultPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + score.ToString();
    }
}
