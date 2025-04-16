using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightButtonGame : MonoBehaviour
{
    public Button[] buttons; // 9���� TMP ��ư
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public GameObject resultPanel;  // ��� �г� (UI)
    public TextMeshProUGUI finalScoreText;  // ��� �г��� ���� ���� �ؽ�Ʈ

    private List<int> lightSequence = new List<int>(); // �Һ� ������
    private int inputIndex = 0;
    private int score = 0;
    private float timeLeft = 60f;  // Ÿ�̸Ӹ� 60�ʷ� ����
    private bool acceptingInput = false;

    void Start()
    {
        scoreText.text = "Score: 0";
        resultPanel.SetActive(false);  // ���� ���� �� ��� �г� �����
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;  // Ÿ�̸� ����
            timerText.text = "Time: " + timeLeft.ToString("F1");  // Ÿ�̸� ���

            // ����ڰ� �Է��� ���� �ʰ� ���� ���� �Һ� �������� ����
            if (!acceptingInput)
            {
                // �Һ� �������� ���� ����
                yield return StartCoroutine(ShowLights());
            }

            yield return null;
        }

        // Ÿ�̸Ӱ� ���� �� ��� �г��� Ȱ��ȭ�ϰ� ���� ���� ǥ��
        ShowResultPanel();
    }

    IEnumerator ShowLights()
    {
        lightSequence.Clear();  // �� ���带 ���� �������� �ʱ�ȭ

        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 6; i++)  // 6���� �Һ� ������
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, buttons.Length);  // ��ư �� �ϳ� ���� ����
            } while (usedIndexes.Contains(randomIndex));  // �ߺ��� ��ư�� ���ϱ� ����

            usedIndexes.Add(randomIndex);
            lightSequence.Add(randomIndex);  // �Һ� �������� �߰�

            // ��ư�� �Һ� ȿ���� �Ѱ� ���� �κ�
            buttons[randomIndex].GetComponent<ButtonLightEffect>().TurnOnLight();
            yield return new WaitForSeconds(0.5f);
            buttons[randomIndex].GetComponent<ButtonLightEffect>().TurnOffLight();
            yield return new WaitForSeconds(0.2f);
        }

        // ��ư�� Ŭ�� �̺�Ʈ ����
        acceptingInput = true;  // ��ư Ŭ���� ���� �� �ֵ��� ����
        inputIndex = 0;  // ���� ���� �� �Է� �ε����� 0���� �ʱ�ȭ

        // ��ư Ŭ�� �̺�Ʈ ����
        for (int i = 0; i < buttons.Length; i++)
        {
            int btnIndex = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnButtonPressed(btnIndex));
        }
    }

    void OnButtonPressed(int index)
    {
        if (!acceptingInput || timeLeft <= 0f) return;  // ������ �����ų� �Է��� ���� �� ������ �ƹ� ���۵� ���� ����

        if (index == lightSequence[inputIndex])  // �ùٸ� ��ư�� ��������
        {
            inputIndex++;
            if (inputIndex >= lightSequence.Count)  // 6���� ��ư ��� ��������
            {
                score++;  // ���� ����
                scoreText.text = "Score: " + score;  // ���� ȭ�鿡 ������Ʈ
                acceptingInput = false;  // �Է��� ���� �ʵ��� ����

                // ���� �ð� �� ���� ���带 �����ϵ��� delay�� �� �� �ֽ��ϴ�
                StartCoroutine(NextRoundDelay());
            }
        }
        else  // Ʋ�� ��ư�� ��������
        {
            inputIndex = 0;  // �Է� �ε����� ó������ �ǵ����� �ٽ� ����
            acceptingInput = false;  // �Է��� ���� �ʵ��� ����

            // Ʋ�� ��� �ٷ� ���ο� ���带 ����
            StartCoroutine(NextRoundDelay());
        }
    }

    IEnumerator NextRoundDelay()
    {
        // Ʋ�� ��쳪 ���� ��� ��� ��� ��ٷȴٰ� ���� ���带 ����
        yield return new WaitForSeconds(1f);  // 1�� ��� �� �Һ� ������ ����
        acceptingInput = false;  // ���� ���带 ���� �Է��� ����
    }

    void ShowResultPanel()
    {
        // ���� ���� �� ��� �г��� ���� ���� ���� ǥ��
        resultPanel.SetActive(true);  // ��� �г� ���̱�
        finalScoreText.text = "Final Score: " + score.ToString();  // ���� ���� ǥ��
    }
}
