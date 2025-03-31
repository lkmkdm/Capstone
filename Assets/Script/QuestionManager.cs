using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    FirebaseFirestore db;
    FirebaseAuth auth;

    private string userEmail;
    private string userID;
    private string userName;

    public string selectedGender = "Male"; // �⺻��
    public int selectedAge = 0; // ��Ӵٿ�� ���õ� ��
    public string difficulty = "normal"; // �⺻��

    // ���� ���� ���� ������
    public GameObject questionPrefab; // ���� �г� ������
    public Transform content; // Content (�� Panel)
    public Button nextButton2, prevButton; // ����, ���� ��ư
    private TMP_Text nextButtonText; // "�Ϸ�" ��ư
    public GameObject testPanel1; // ���� & ���� �Է� �г� (TestPanel1)
    public GameObject testPanel2; // ���� �г� (TestPanel2)
    public Slider progressSlider; // ���� ������� ��Ÿ�� �����̴�
    public TMP_Text progressText; // ����� �ۼ�Ʈ �ؽ�Ʈ
    public Image progressFill; // �����̴� Fill �̹��� (���� �����)
    public Button womenButton, manButton; // ���� ��ư
    public TMP_Dropdown ageDropdown; // ���� ���� ��Ӵٿ�

    private string gender = ""; // ���� �� ����
    private int age = 0; // ���� �� ����

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
        // Firebase �ʱ�ȭ
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // Firebase���� ����� ���� ��������
        if (auth.CurrentUser != null)
        {
            userEmail = auth.CurrentUser.Email;
            userID = auth.CurrentUser.UserId;
            userName = auth.CurrentUser.DisplayName;
        }

        nextButtonText = nextButton2.GetComponentInChildren<TMP_Text>(); // ��ư ���� �ؽ�Ʈ ��������

        ShuffleQuestions(); // ������ �������� ����
        selectedButtonIndices = new List<int>(new int[questions.Length]); // �ʱⰪ -1�� ����
        for (int i = 0; i < selectedButtonIndices.Count; i++)
            selectedButtonIndices[i] = -1;

        GenerateQuestionPanel();
        UpdateButtons();


        // ���� ���� �̺�Ʈ �߰�
        womenButton.onClick.AddListener(() => SelectGender("Female"));
        manButton.onClick.AddListener(() => SelectGender("Male"));
        ageDropdown.onValueChanged.AddListener(delegate { UpdateAge(); });

        // ��ư Ŭ�� �̺�Ʈ �߰�
        nextButton2.onClick.AddListener(ShowNextQuestion);
        prevButton.onClick.AddListener(ShowPreviousQuestion);

        nextButton2.interactable = false; // ó���� ��Ȱ��ȭ
    }

    // ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    void SelectGender(string selectedGender)
    {
        gender = selectedGender;
        womenButton.image.color = (gender == "Female") ? Color.green : Color.white;
        manButton.image.color = (gender == "Male") ? Color.green : Color.white;
    }

    // ���� ��Ӵٿ� �� ���� �� ȣ��Ǵ� �Լ�
    void UpdateAge()
    {
        age = int.Parse(ageDropdown.options[ageDropdown.value].text);
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

        // �����̴� ȿ�� ����
        int percentValue = Mathf.RoundToInt(progress * 100);
        progressText.text = percentValue + "%";
        progressFill.color = Color.Lerp(Color.red, Color.green, progress);

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

    // ���� �Ϸ� �� ����
    void FinishTest()
    {
        selectedGender = gender;
        selectedAge = age;
        SaveUserData();
        SceneManager.LoadScene("UserInfo");

        SceneManager.LoadScene("UserInfo"); // �� �̵�
    }

    // Firebase�� ����� ������ ����
    void SaveUserData()
    {
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userID))
        {
            Debug.LogError("���� ������ �����ϴ�. �α��� ���¸� Ȯ���ϼ���.");
            return;
        }

        DocumentReference userDocRef = db
            .Collection("users")
            .Document(userEmail)
            .Collection("personal_information")
            .Document("info");

        var userData = new
        {
            age = selectedAge,
            difficulty = difficulty,
            gender = selectedGender,
            name = userName,
            userid = userID
        };

        userDocRef.SetAsync(userData).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Firestore�� ����� ������ ���� ����!");
            }
            else
            {
                Debug.LogError("Firestore ������ ���� ����: " + task.Exception);
            }
        });
    }

    // ��ư Ȱ��ȭ/��Ȱ��ȭ ����
    void UpdateButtons()
    {
        prevButton.interactable = true;
    }
}
