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

    public string selectedGender = "Male"; // 기본값
    public int selectedAge = 0; // 드롭다운에서 선택된 값
    public string difficulty = "normal"; // 기본값

    // 설문 조사 관련 변수들
    public GameObject questionPrefab; // 질문 패널 프리팹
    public Transform content; // Content (빈 Panel)
    public Button nextButton2, prevButton; // 다음, 이전 버튼
    private TMP_Text nextButtonText; // "완료" 버튼
    public GameObject testPanel1; // 성별 & 나이 입력 패널 (TestPanel1)
    public GameObject testPanel2; // 질문 패널 (TestPanel2)
    public Slider progressSlider; // 설문 진행률을 나타낼 슬라이더
    public TMP_Text progressText; // 진행률 퍼센트 텍스트
    public Image progressFill; // 슬라이더 Fill 이미지 (색상 변경용)
    public Button womenButton, manButton; // 성별 버튼
    public TMP_Dropdown ageDropdown; // 나이 선택 드롭다운

    private string gender = ""; // 성별 값 저장
    private int age = 0; // 나이 값 저장

    private List<int> questionOrder = new List<int>(); // 랜덤 순서 저장
    private List<GameObject> questionPanels = new List<GameObject>(); // 생성된 패널 리스트
    private int currentQuestionIndex = 0; // 현재 질문 위치

    // 7단계 점수 리스트
    private readonly float[] scoreValues = { 0f, 2.78f, 5.56f, 8.34f, 11.12f, 13.90f, 16.67f };

    // 질문 리스트
    private string[] questions = {
        "0집중하는 것이 어렵다고 느낀다.",
        "1과제를 끝까지 마무리하는 것이 어렵다.",
        "2주의가 산만해지는 경우가 많다.",
        "3한 가지 일에 오래 집중할 수 없다.",
        "4계획을 세우는 것이 어렵다.",
        "5결정을 내리는 것이 어렵다.",
        "6기억력이 좋지 않다고 느낀다.",
        "7자주 물건을 잃어버린다.",
        "8다른 사람의 말을 듣다가 금방 딴 생각을 한다.",
        "9집중해야 할 때 쉽게 산만해진다.",
        "10계획한 일을 자주 잊어버린다.",
        "11일을 체계적으로 수행하는 것이 어렵다.",
        "12여러 가지 일을 동시에 하면 실수가 많다.",
        "13시간 관리를 잘 하지 못한다.",
        "14해야 할 일을 미루는 경우가 많다.",
        "15자신이 한 행동을 종종 후회한다.",
        "16다른 사람보다 감정 기복이 심하다고 느낀다.",
        "17참을성이 부족하다고 느낀다.",
        "18감정 조절이 어렵다고 생각한다.",
        "19남들이 하는 말을 쉽게 오해한다.",
        "20친구 관계를 유지하는 것이 어렵다.",
        "21사회적인 상황에서 불안감을 느낀다.",
        "22자신감이 부족하다고 생각한다.",
        "23새로운 환경에 적응하는 것이 어렵다.",
    };

    private List<int> selectedButtonIndices = new List<int>(); // 선택된 버튼 인덱스 저장

    int[][] categories = new int[][] {
    new int[] { 0, 1, 2, 3, 4, 5 },      // original_concentration
    new int[] { 6, 7, 8, 9, 10, 11 },      // original_impulsiveness
    new int[] { 12, 13, 14, 15, 16, 17 }, // original_memory
    new int[] { 18, 19, 20, 21, 22, 23 }, // original_processingspeed
    };

    void Start()
    {
        // Firebase 초기화
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // Firebase에서 사용자 정보 가져오기
        if (auth.CurrentUser != null)
        {
            userEmail = auth.CurrentUser.Email;
            userID = auth.CurrentUser.UserId;
            userName = auth.CurrentUser.DisplayName;
        }

        nextButtonText = nextButton2.GetComponentInChildren<TMP_Text>(); // 버튼 내부 텍스트 가져오기

        ShuffleQuestions(); // 질문을 랜덤으로 섞기
        selectedButtonIndices = new List<int>(new int[questions.Length]); // 초기값 -1로 설정
        for (int i = 0; i < selectedButtonIndices.Count; i++)
            selectedButtonIndices[i] = -1;

        GenerateQuestionPanel();
        UpdateButtons();


        // 성별 나이 이벤트 추가
        womenButton.onClick.AddListener(() => SelectGender("Female"));
        manButton.onClick.AddListener(() => SelectGender("Male"));
        ageDropdown.onValueChanged.AddListener(delegate { UpdateAge(); });

        // 버튼 클릭 이벤트 추가
        nextButton2.onClick.AddListener(ShowNextQuestion);
        prevButton.onClick.AddListener(ShowPreviousQuestion);

        nextButton2.interactable = false;
    }

    // 성별 버튼 클릭 시 호출되는 함수
    void SelectGender(string selectedGender)
    {
        gender = selectedGender;
        womenButton.image.color = (gender == "Female") ? Color.gray : Color.white;
        manButton.image.color = (gender == "Male") ? Color.gray : Color.white;
    }

    // 나이 드롭다운 값 변경 시 호출되는 함수
    void UpdateAge()
    {
        age = int.Parse(ageDropdown.options[ageDropdown.value].text);
    }

    // 질문 리스트를 랜덤으로 섞기
    void ShuffleQuestions()
    {
        questionOrder.Clear(); // 기존 질문 순서 초기화

        // 각 카테고리별로 섞기
        foreach (int[] category in categories)
        {
            List<int> shuffledCategory = new List<int>(category);

            // 해당 카테고리 내부에서만 랜덤 섞기
            for (int i = shuffledCategory.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (shuffledCategory[i], shuffledCategory[randomIndex]) = (shuffledCategory[randomIndex], shuffledCategory[i]);
            }

            // 섞은 후 최종 질문 순서에 추가
            questionOrder.AddRange(shuffledCategory);
        }
    }

    // 첫 번째 질문을 생성 및 표시
    void GenerateQuestionPanel()
    {
        ShowQuestion(currentQuestionIndex);
    }

    // 질문 패널을 생성하고 표시
    void ShowQuestion(int index)
    {
        // 기존 질문 패널이 있다면 모두 삭제
        foreach (GameObject panel in questionPanels)
        {
            Destroy(panel);
        }
        questionPanels.Clear();

        int questionIndex = questionOrder[index]; // 랜덤 순서 적용
        GameObject newPanel = Instantiate(questionPrefab, content);
        questionPanels.Add(newPanel);

        TMP_Text questionText = newPanel.transform.Find("Test1")?.GetComponent<TMP_Text>();
        if (questionText != null)
        {
            questionText.text = questions[questionIndex];
        }

        // 버튼 이벤트 추가
        Button[] buttons = newPanel.GetComponentsInChildren<Button>();
        for (int j = 0; j < buttons.Length; j++)
        {
            int buttonIndex = j; // 클로저 문제 방지
            buttons[j].onClick.RemoveAllListeners();
            buttons[j].onClick.AddListener(() => OnButtonClick(newPanel, buttonIndex));

            // 이전에 선택한 버튼의 테두리 유지
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

        // 이전에 선택된 버튼이 있다면 "다음" 버튼 활성화
        nextButton2.interactable = selectedButtonIndices[currentQuestionIndex] != -1;

        // 마지막 질문이면 "완료"로 버튼 텍스트 변경
        if (currentQuestionIndex == questionOrder.Count - 1)
        {
            nextButtonText.text = "완료";
            nextButton2.onClick.RemoveAllListeners();
            nextButton2.onClick.AddListener(FinishTest);
        }
        else
        {
            nextButtonText.text = "다음";
            nextButton2.onClick.RemoveAllListeners();
            nextButton2.onClick.AddListener(ShowNextQuestion);
        }

        // 슬라이더 값 갱신
        float progress = (float)(currentQuestionIndex) / questionOrder.Count;
        progressSlider.value = progress;

        // 슬라이더 효과 관련
        int percentValue = Mathf.RoundToInt(progress * 100);
        progressText.text = percentValue + "%";
        progressFill.color = Color.Lerp(Color.gray, Color.cyan, progress);

        UpdateButtons();
    }

    // 버튼 클릭 시 점수 선택
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

        nextButton2.interactable = true; // 선택하면 "다음" 버튼 활성화
    }

    // 다음 질문 표시
    public void ShowNextQuestion()
    {
        if (nextButton2.interactable == false) return;  // 이벤트 리스너 중복 호출 방지

        if (currentQuestionIndex < questionOrder.Count - 1)
        {
            currentQuestionIndex++;
            ShowQuestion(currentQuestionIndex);
        }
    }

    // 이전 질문 표시
    public void ShowPreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--; // 이전 질문으로 이동
            ShowQuestion(currentQuestionIndex);
        }
        else
        {
            // 첫 번째 질문에서 "이전"을 누르면 TestPanel1로 이동
            testPanel2.SetActive(false);
            testPanel1.SetActive(true);
        }
    }

    // 설문 완료 후 저장
    void FinishTest()
    {
        selectedGender = gender;
        selectedAge = age;
        SaveUserData();

        SaveTestResults(() =>
        {
            SceneManager.LoadScene("Analyzing"); // Firestore 저장 후 씬 이동
        });
    }

    // Firebase에 사용자 데이터 저장
    void SaveUserData()
    {
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userID))
        {
            Debug.LogError("유저 정보가 없습니다. 로그인 상태를 확인하세요.");
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
                Debug.Log("Firestore에 사용자 데이터 저장 성공!");
            }
            else
            {
                Debug.LogError("Firestore 데이터 저장 실패: " + task.Exception);
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

        // 잠재력 계산: 능력치 * 나이 보정 * 랜덤
        float rawScore = original_concentration * original_impulsiveness * original_memory * original_processingspeed;
        float ageModifier = Mathf.Clamp(1.5f - (selectedAge / 100f), 0.8f, 1.5f);
        float randomFactor = Random.Range(0.9f, 1.1f);

        float finalPotential;

        if (rawScore < 25f)
        {
            finalPotential = Random.Range(50f, 65f);
        }
        else
        {
            float potentialRaw = rawScore * ageModifier * randomFactor;
            float normalized = Mathf.Clamp01(potentialRaw / 625f);
            finalPotential = 50f + normalized * 50f;
        }

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
                Debug.Log("Firestore에 능력치 데이터 저장 성공!");
                onComplete?.Invoke(); // 저장이 완료된 후 씬 전환
            }
            else
            {
                Debug.LogError("Firestore 능력치 데이터 저장 실패: " + task.Exception);
            }
        });
    }

    // 버튼 활성화/비활성화 관리
    void UpdateButtons()
    {
        prevButton.interactable = true;
    }
}
