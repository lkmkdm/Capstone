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
    private readonly float[] scoreValues = { 16.67f, 13.90f, 11.12f, 8.34f, 5.56f, 2.78f, 0f };

    // ���� ����Ʈ(����)
    private string[] questions = {
        "�����ϰų� ������ �Ͽ� �����ϱ� �����.",
        "���ɼ��� ���� �Ǽ��� ���� �ϴ� ���̴�.",
        "���ǰ� �길������ ��찡 ����.",
        "�ٸ� ����� ���� ��ٰ� �ݹ� �� ������ �Ѵ�.",
        "�������� �� �ߵ��� ���Ѵ�.",
        "� �Ͽ� �����ϰ� �����Ѵ�.",
        "��Ȳ�� ������� �ʰ� �Ӹ��� �������� ������ �ﰢ������ ���Ѵ�.",
        "���� �浿������ ����.",
        "���� �� �����, ������, �๰����, �浿������ְ� �ִ� ����� �ִ�.",
        "���ʿ��� ������ ����.",
        "�ٸ� ������� ���� �⺹�� ���ϴٰ� ������.",
        "��, ���, ����, ����, ��, ���� � ���� �������.",
        "��ȹ�� ���� �ؾ���� ���� �ִ�.",
        "������ ���� �ʴٰ� ������.",
        "������ ���� �ؾ������.",
        "����̳� �ؾ� �� ���� �ؾ���� ����� �������� �ִ�.",
        "���� � ������ �Ծ����� ����� �� �ȳ���.",
        "������ ������ ���� �ΰų� ��� �ξ����� �� �𸣰ڴ�.",
        "������ ������ �������ϴ� ���� ��ƴ�.",
        "�ð� ������ �� ���� ���Ѵ�.",
        "�ؾ� �� ���� �̷�� ��찡 ����.",
        "���ο� ���� �����ϰ� �غ��ϱ���� ���� �ɸ���.",
        "�����̳� ��� �ð��� ���� �ʴ´�.",
        "���� ���� ���� ���ÿ� �ϸ� �Ǽ��� ����.",
    };

    private List<int> selectedButtonIndices = new List<int>(); // ���õ� ��ư �ε��� ����

    int[][] categories = new int[][] {
    new int[] { 0, 1, 2, 3, 4, 5 },      // original_concentration
    new int[] { 6, 7, 8, 9, 10, 11 },      // original_impulsiveness
    new int[] { 12, 13, 14, 15, 16, 17 }, // original_memory
    new int[] { 18, 19, 20, 21, 22, 23 }, // original_processingspeed
    };

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

        nextButton2.interactable = false;
    }

    // ���� ��ư Ŭ�� �� ȣ��Ǵ� �Լ�
    void SelectGender(string selectedGender)
    {
        gender = selectedGender;
        womenButton.image.color = (gender == "Female") ? Color.gray : Color.white;
        manButton.image.color = (gender == "Male") ? Color.gray : Color.white;
    }

    // ���� ��Ӵٿ� �� ���� �� ȣ��Ǵ� �Լ�
    void UpdateAge()
    {
        age = int.Parse(ageDropdown.options[ageDropdown.value].text);
    }

    // ���� ����Ʈ�� �������� ����
    void ShuffleQuestions()
    {
        questionOrder.Clear(); // ���� ���� ���� �ʱ�ȭ

        // �� ī�װ����� ����
        foreach (int[] category in categories)
        {
            List<int> shuffledCategory = new List<int>(category);

            // �ش� ī�װ� ���ο����� ���� ����
            for (int i = shuffledCategory.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (shuffledCategory[i], shuffledCategory[randomIndex]) = (shuffledCategory[randomIndex], shuffledCategory[i]);
            }

            // ���� �� ���� ���� ������ �߰�
            questionOrder.AddRange(shuffledCategory);
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
        // ���� ���� �г��� �ִٸ� ��� ����
        foreach (GameObject panel in questionPanels)
        {
            Destroy(panel);
        }
        questionPanels.Clear();

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
                    outline.effectDistance = new Vector2(5, 5);
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
        float progress = (float)(currentQuestionIndex) / questionOrder.Count;
        progressSlider.value = progress;

        // �����̴� ȿ�� ����
        int percentValue = Mathf.RoundToInt(progress * 100);
        progressText.text = percentValue + "%";
        progressFill.color = Color.Lerp(Color.gray, Color.cyan, progress);

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
                outline.effectDistance = (i == index) ? new Vector2(5, 5) : new Vector2(1, 1);
            }
        }

        nextButton2.interactable = true; // �����ϸ� "����" ��ư Ȱ��ȭ
    }

    // ���� ���� ǥ��
    public void ShowNextQuestion()
    {
        if (nextButton2.interactable == false) return;  // �̺�Ʈ ������ �ߺ� ȣ�� ����

        if (currentQuestionIndex < questionOrder.Count - 1)
        {
            currentQuestionIndex++;
            ShowQuestion(currentQuestionIndex);
        }
    }

    // ���� ���� ǥ��
    public void ShowPreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--; // ���� �������� �̵�
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            // ù ��° �������� "����"�� ������ TestPanel1�� �̵�
            testPanel2.SetActive(false);
            testPanel1.SetActive(true);
        }
    }

    // ���� �Ϸ� �� ����
    void FinishTest()
    {
        selectedGender = gender;
        selectedAge = age;
        SaveUserData();

        SaveTestResults(() =>
        {
            SceneManager.LoadScene("Analyzing"); // Firestore ���� �� �� �̵�
        });
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
            userid = userID,
            adhd_test = true
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

    void SaveTestResults(System.Action onComplete)
    {
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userID))
        {
            return;
        }

        float original_concentration = 0f;
        float original_impulsiveness = 0f;
        float original_memory = 0f;
        float original_processingspeed = 0f;

        for (int i = 0; i < categories.Length; i++)
        {
            foreach (int questionIndex in categories[i])
            {
                if (selectedButtonIndices[questionIndex] != -1)
                {
                    float selectedScore = scoreValues[selectedButtonIndices[questionIndex]];
                    switch (i)
                    {
                        case 0: original_concentration += selectedScore; break;
                        case 1: original_impulsiveness += selectedScore; break;
                        case 2: original_memory += selectedScore; break;
                        case 3: original_processingspeed += selectedScore; break;
                    }
                }
            }
        }

        DocumentReference abilityDocRef = db
            .Collection("users")
            .Document(userEmail)
            .Collection("personal_information")
            .Document("original_ability");

        // ����� ���: �ɷ�ġ * ���� ���� * ����
        float rawScore = original_concentration + original_impulsiveness + original_memory + original_processingspeed;

        // ���� ���� (20�� ���� 1.0 �� ���� �������� ����)
        float ageModifier = Mathf.Clamp(1.2f - (selectedAge / 100f), 0.8f, 1.2f);
        float randomFactor = Random.Range(0.95f, 1.05f);

        float potentialRaw = rawScore * ageModifier * randomFactor;
        float normalized = Mathf.Clamp01(potentialRaw / 400f); // �ִ� 400 ����
        float finalPotential = 50f + normalized * 50f;

        finalPotential = Mathf.Round(finalPotential * 10f) / 10f;

        var abilityData = new Dictionary<string, object>
    {
        { "original_concentration", Mathf.Round(original_concentration * 10f) / 10f },
        { "original_impulsiveness", Mathf.Round(original_impulsiveness * 10f) / 10f },
        { "original_memory", Mathf.Round(original_memory * 10f) / 10f },
        { "original_processingspeed", Mathf.Round(original_processingspeed * 10f) / 10f },
        { "original_potential", finalPotential }
    };

        abilityDocRef.SetAsync(abilityData).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Firestore�� �ɷ�ġ ������ ���� ����!");
                onComplete?.Invoke(); // ������ �Ϸ�� �� �� ��ȯ
            }
            else
            {
                Debug.LogError("Firestore �ɷ�ġ ������ ���� ����: " + task.Exception);
            }
        });
    }

    // ��ư Ȱ��ȭ/��Ȱ��ȭ ����
    void UpdateButtons()
    {
        prevButton.interactable = true;
    }
}
