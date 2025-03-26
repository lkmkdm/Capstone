using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TestPanelController : MonoBehaviour
{
    public GameObject testPanel1; // TestPanel1 (���� ���� & ���� �Է�)
    public GameObject testPanel2; // TestPanel2 (���� �г�)

    public Button manButton, womanButton; // ���� ���� ��ư
    public TMP_Dropdown ageDropdown; // ���� ���� ��Ӵٿ�
    public Button nextButton; // TestPanel1���� TestPanel2�� �̵��� ��ư

    private bool isGenderSelected = false;
    private bool isAgeSelected = false;
    private string selectedGender = ""; // ���õ� ���� ����

    void Start()
    {
        // �ʱ� ��ư ���� ����
        nextButton.interactable = false;

        // ���� ��Ӵٿ� 1~120 �ɼ� ����
        List<TMP_Dropdown.OptionData> ageOptions = new List<TMP_Dropdown.OptionData>();
        ageOptions.Add(new TMP_Dropdown.OptionData("����")); // ù ��° �ɼ�
        for (int i = 1; i <= 120; i++)
        {
            ageOptions.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        ageDropdown.ClearOptions();
        ageDropdown.AddOptions(ageOptions);
        ageDropdown.value = 0; // �⺻�� ����

        // ��ư Ŭ�� �̺�Ʈ �߰�
        manButton.onClick.AddListener(() => SelectGender("����", manButton, womanButton));
        womanButton.onClick.AddListener(() => SelectGender("����", womanButton, manButton));

        // ��Ӵٿ� ���� ����
        ageDropdown.onValueChanged.AddListener(delegate { OnAgeSelected(); });

        // "����" ��ư Ŭ�� �� TestPanel2�� �̵�
        nextButton.onClick.AddListener(GoToTestPanel2);
    }

    // ���� ��ư Ŭ�� �� ȣ��
    void SelectGender(string gender, Button selectedButton, Button otherButton)
    {
        selectedGender = gender;
        isGenderSelected = true;

        // ������ ��ư ���� ȿ�� (�׵θ� ����)
        Outline selectedOutline = selectedButton.GetComponent<Outline>();
        Outline otherOutline = otherButton.GetComponent<Outline>();

        if (selectedOutline != null) selectedOutline.effectColor = Color.black;
        if (otherOutline != null) otherOutline.effectColor = Color.white;

        CheckNextButton();
    }

    // ���� ���� �� ȣ��
    void OnAgeSelected()
    {
        // ù ��° �ɼ��� �ƴ� ��츸 ���õ�
        isAgeSelected = ageDropdown.value != 0;
        CheckNextButton();
    }

    // "����" ��ư Ȱ��ȭ ���� üũ
    void CheckNextButton()
    {
        nextButton.interactable = isGenderSelected && isAgeSelected;
    }

    // TestPanel2�� �̵�
    void GoToTestPanel2()
    {
        testPanel1.SetActive(false);
        testPanel2.SetActive(true);
    }
}
