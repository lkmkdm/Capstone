using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    public GameObject questionPrefab; // ���� �г� ������
    public Transform content; // Content (�� Panel)

    public Button nextButton2, prevButton; // ����, ���� ��ư
    private TMP_Text nextButtonText; // "�Ϸ�" ��ư

    public GameObject testPanel1; // ���� & ���� �Է� �г� (TestPanel1)
    public GameObject testPanel2; // ���� �г� (TestPanel2)

    public Slider progressSlider; // ���� ������� ��Ÿ�� �����̴�

    private List<int> questionOrder = new List<int>(); // ���� ���� ����
    private List<GameObject> questionPanels = new List<GameObject>(); // ������ �г� ����Ʈ
    private int currentQuestionIndex = 0; // ���� ���� ��ġ

    // 7�ܰ� ���� ����Ʈ
    private readonly float[] scoreValues = { 0f, 3.3f, 6.6f, 10f, 13.3f, 16.6f, 20f };

    // ���� ����Ʈ
    private string[] questions = {
        "�����ϴ� ���� ��ƴٰ� ������.",
        "������ ������ �������ϴ� ���� ��ƴ�.",
        "���ǰ� �길������ ��찡 ����.",
        "�� ���� �Ͽ� ���� ������ �� ����.",
        "��ȹ�� ����� ���� ��ƴ�.",
        "������ ������ ���� ��ƴ�.",
        "������ ���� �ʴٰ� ������.",
        "���� ������ �Ҿ������.",
        "�ٸ� ����� ���� ��ٰ� �ݹ� �� ������ �Ѵ�.",
        "�����ؾ� �� �� ���� �길������.",
        "��ȹ�� ���� ���� �ؾ������.",
        "���� ü�������� �����ϴ� ���� ��ƴ�.",
        "���� ���� ���� ���ÿ� �ϸ� �Ǽ��� ����.",
        "�ð� ������ �� ���� ���Ѵ�.",
        "�ؾ� �� ���� �̷�� ��찡 ����.",
        "�ڽ��� �� �ൿ�� ���� ��ȸ�Ѵ�.",
        "�ٸ� ������� ���� �⺹�� ���ϴٰ� ������.",
        "�������� �����ϴٰ� ������.",
        "���� ������ ��ƴٰ� �����Ѵ�.",
        "������ �ϴ� ���� ���� �����Ѵ�.",
        "ģ�� ���踦 �����ϴ� ���� ��ƴ�.",
        "��ȸ���� ��Ȳ���� �ҾȰ��� ������.",
        "�ڽŰ��� �����ϴٰ� �����Ѵ�.",
        "���ο� ȯ�濡 �����ϴ� ���� ��ƴ�.",
        "�йڰ��� ���� �� �� �Ǽ��� ���� �Ѵ�."
    };

    private List<int> selectedButtonIndices = new List<int>(); // ���õ� ��ư �ε��� ����

    void Start()
    {
        nextButtonText = nextButton2.GetComponentInChildren<TMP_Text>(); // ��ư ���� �ؽ�Ʈ ��������

        ShuffleQuestions(); // ������ �������� ����
        selectedButtonIndices = new List<int>(new int[questions.Length]); // �ʱⰪ -1�� ����
        for (int i = 0; i < selectedButtonIndices.Count; i++)
            selectedButtonIndices[i] = -1;

        GenerateQuestionPanel();
        UpdateButtons();

        // ��ư Ŭ�� �̺�Ʈ �߰�
        nextButton2.onClick.AddListener(ShowNextQuestion);
        prevButton.onClick.AddListener(ShowPreviousQuestion);

        nextButton2.interactable = false; // ó���� ��Ȱ��ȭ
    }

    // ���� ����Ʈ�� �������� ����
    void ShuffleQuestions()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < questions.Length; i++)
        {
            indexes.Add(i);
        }
        while (indexes.Count > 0)
        {
            int randomIndex = Random.Range(0, indexes.Count);
            questionOrder.Add(indexes[randomIndex]);
            indexes.RemoveAt(randomIndex);
        }
    }

    // ù ��° ������ ���� �� ǥ��
    void GenerateQuestionPanel()
    {
        ShowQuestion(currentQuestionIndex);
    }

    // ���� �г��� �����ϰ� ǥ��
    void ShowQuestion(int index)
    {
        if (questionPanels.Count > 0)
        {
            Destroy(questionPanels[0]); // ���� �г� ����
            questionPanels.Clear();
        }

        int questionIndex = questionOrder[index]; // ���� ���� ����
        GameObject newPanel = Instantiate(questionPrefab, content);
        questionPanels.Add(newPanel);

        TMP_Text questionText = newPanel.transform.Find("Test1")?.GetComponent<TMP_Text>();
        if (questionText != null)
        {
            questionText.text = questions[questionIndex];
        }

        // ��ư �̺�Ʈ �߰�
        Button[] buttons = newPanel.GetComponentsInChildren<Button>();
        for (int j = 0; j < buttons.Length; j++)
        {
            int buttonIndex = j; // Ŭ���� ���� ����
            buttons[j].onClick.RemoveAllListeners();
            buttons[j].onClick.AddListener(() => OnButtonClick(newPanel, buttonIndex));

            // ������ ������ ��ư�� �׵θ� ����
            Outline outline = buttons[j].GetComponent<Outline>();
            if (outline != null)
            {
                if (selectedButtonIndices[currentQuestionIndex] == j)
                {
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(12, 12);
                }
                else
                {
                    outline.effectColor = Color.white;
                    outline.effectDistance = new Vector2(1, 1);
                }
            }
        }

        // ������ ���õ� ��ư�� �ִٸ� "����" ��ư Ȱ��ȭ
        nextButton2.interactable = selectedButtonIndices[currentQuestionIndex] != -1;

        // ������ �����̸� "�Ϸ�"�� ��ư �ؽ�Ʈ ����
        if (currentQuestionIndex == questionOrder.Count - 1)
        {
            nextButtonText.text = "�Ϸ�";
            nextButton2.onClick.RemoveAllListeners();
            nextButton2.onClick.AddListener(FinishTest);
        }
        else
        {
            nextButtonText.text = "����";
            nextButton2.onClick.RemoveAllListeners();
            nextButton2.onClick.AddListener(ShowNextQuestion);
        }

        // �����̴� �� ����
        float progress = (float)(currentQuestionIndex + 1) / questionOrder.Count;
        progressSlider.value = progress;

        UpdateButtons();
    }

    // ��ư Ŭ�� �� ���� ����
    void OnButtonClick(GameObject panel, int index)
    {
        selectedButtonIndices[currentQuestionIndex] = index;
        float selectedScore = scoreValues[index];

        Button[] buttons = panel.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Outline outline = buttons[i].GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = (i == index) ? Color.black : Color.white;
                outline.effectDistance = (i == index) ? new Vector2(12, 12) : new Vector2(1, 1);
            }
        }

        nextButton2.interactable = true; // �����ϸ� "����" ��ư Ȱ��ȭ
    }

    // ���� ���� ǥ��
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < questionOrder.Count - 1)
        {
            currentQuestionIndex++;
            ShowQuestion(currentQuestionIndex);
        }
    }

    // ���� ���� ǥ��
    public void ShowPreviousQuestion()
    {
        if (currentQuestionIndex == 0)
        {
            // "����" ��ư�� ó�� �������� ������ TestPanel1�� ���ư���
            testPanel2.SetActive(false);
            testPanel1.SetActive(true);
        }
        else
        {
            currentQuestionIndex--;
            ShowQuestion(currentQuestionIndex);
        }
    }

    void FinishTest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UserInfo"); // �� �̵�
    }

    // ��ư Ȱ��ȭ/��Ȱ��ȭ ����
    void UpdateButtons()
    {
        prevButton.interactable = true;
    }
}
