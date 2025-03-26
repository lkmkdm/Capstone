using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TestPanelController : MonoBehaviour
{
    public GameObject testPanel1; // TestPanel1 (성별 선택 & 나이 입력)
    public GameObject testPanel2; // TestPanel2 (질문 패널)

    public Button manButton, womanButton; // 성별 선택 버튼
    public TMP_Dropdown ageDropdown; // 나이 선택 드롭다운
    public Button nextButton; // TestPanel1에서 TestPanel2로 이동할 버튼

    private bool isGenderSelected = false;
    private bool isAgeSelected = false;
    private string selectedGender = ""; // 선택된 성별 저장

    void Start()
    {
        // 초기 버튼 상태 설정
        nextButton.interactable = false;

        // 나이 드롭다운 1~120 옵션 설정
        List<TMP_Dropdown.OptionData> ageOptions = new List<TMP_Dropdown.OptionData>();
        ageOptions.Add(new TMP_Dropdown.OptionData("나이")); // 첫 번째 옵션
        for (int i = 1; i <= 120; i++)
        {
            ageOptions.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        ageDropdown.ClearOptions();
        ageDropdown.AddOptions(ageOptions);
        ageDropdown.value = 0; // 기본값 설정

        // 버튼 클릭 이벤트 추가
        manButton.onClick.AddListener(() => SelectGender("남성", manButton, womanButton));
        womanButton.onClick.AddListener(() => SelectGender("여성", womanButton, manButton));

        // 드롭다운 변경 감지
        ageDropdown.onValueChanged.AddListener(delegate { OnAgeSelected(); });

        // "다음" 버튼 클릭 시 TestPanel2로 이동
        nextButton.onClick.AddListener(GoToTestPanel2);
    }

    // 성별 버튼 클릭 시 호출
    void SelectGender(string gender, Button selectedButton, Button otherButton)
    {
        selectedGender = gender;
        isGenderSelected = true;

        // 선택한 버튼 강조 효과 (테두리 굵게)
        Outline selectedOutline = selectedButton.GetComponent<Outline>();
        Outline otherOutline = otherButton.GetComponent<Outline>();

        if (selectedOutline != null) selectedOutline.effectColor = Color.black;
        if (otherOutline != null) otherOutline.effectColor = Color.white;

        CheckNextButton();
    }

    // 나이 선택 시 호출
    void OnAgeSelected()
    {
        // 첫 번째 옵션이 아닌 경우만 선택됨
        isAgeSelected = ageDropdown.value != 0;
        CheckNextButton();
    }

    // "다음" 버튼 활성화 여부 체크
    void CheckNextButton()
    {
        nextButton.interactable = isGenderSelected && isAgeSelected;
    }

    // TestPanel2로 이동
    void GoToTestPanel2()
    {
        testPanel1.SetActive(false);
        testPanel2.SetActive(true);
    }
}
