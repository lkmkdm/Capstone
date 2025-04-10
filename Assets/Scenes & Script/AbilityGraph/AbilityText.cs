using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Unity.VisualScripting;
using System;

public class FirebaseDataToUI : MonoBehaviour
{
    FirebaseFirestore db;
    FirebaseAuth auth;

    private string userEmail;
    private string userID;
    private string userName;

    public Text memoryText;
    public Text concentrationText;
    public Text processingSpeedText;
    public Text impulsivenessText;
    public Text potentialText;
    public Text sumText;

    public TMPro.TextMeshProUGUI concentrationStateText;
    public TMPro.TextMeshProUGUI impulsivenessStateText;
    public TMPro.TextMeshProUGUI memoryStateText;
    public TMPro.TextMeshProUGUI potentialStateText;
    public TMPro.TextMeshProUGUI processingSpeedStateText;
    public TMPro.TextMeshProUGUI sumStateText;

    void Start()
    {
        // Firebase 초기화
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        //테스트용(테스트 계정 사용 시 바로 밑에 if else 문 주석처리 필수)
        //userEmail = "testuser@example.com";

        //테스트용 사용 시 주석 처리하기
        if (auth.CurrentUser != null)
        {
            userEmail = auth.CurrentUser.Email;
            userID = auth.CurrentUser.UserId;
            userName = auth.CurrentUser.DisplayName;
        }
        else
        {
            Debug.LogError("Firebase Authentication: 로그인된 사용자가 없습니다!");
        }
        //테스트용 사용 시 여기까지 주석 처리

        LoadDataFromFirebase();
    }

    void LoadDataFromFirebase()
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("유저 이메일이 설정되지 않음");
            return;
        }

        db.Collection("users").Document(userEmail)  // 사용자의 이메일이 문서 ID
          .Collection("personal_information").Document("original_ability")
          .GetSnapshotAsync().ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && task.Result.Exists)
              {
                  DocumentSnapshot snapshot = task.Result;
                  Dictionary<string, object> data = snapshot.ToDictionary();

                  // Firebase에서 가져온 값을 애니메이션으로 표시
                  float concentration = GetValueFromDict(data, "original_concentration");
                  float impulsiveness = GetValueFromDict(data, "original_impulsiveness");
                  float memory = GetValueFromDict(data, "original_memory");
                  float potential = GetValueFromDict(data, "original_potential");
                  float processingSpeed = GetValueFromDict(data, "original_processingspeed");

                  // 각각 텍스트 애니메이션
                  StartCoroutine(AnimateText(concentrationText, "집중력", concentration));
                  StartCoroutine(AnimateText(impulsivenessText, "충동성", impulsiveness));
                  StartCoroutine(AnimateText(memoryText, "기억력", memory));
                  StartCoroutine(AnimateText(potentialText, "잠재력", potential));
                  StartCoroutine(AnimateText(processingSpeedText, "처리속도", processingSpeed));

                  // 상태 텍스트 설정
                  SetStatusText(concentrationStateText, concentration);
                  SetStatusText(impulsivenessStateText, impulsiveness);
                  SetStatusText(memoryStateText, memory);
                  SetStatusText(potentialStateText, potential);
                  SetStatusText(processingSpeedStateText, processingSpeed);

                  // 합계 계산 및 표시
                  float sum = concentration + impulsiveness + memory + potential + processingSpeed;
                  StartCoroutine(AnimateSumText(sumText, "총점", sum, 500f));
                  SetSumStatusText(sumStateText, sum);
              }
              else
              {
                  Debug.LogError("Firebase 데이터 불러오기 실패 또는 문서 없음");
              }
          });
    }

    IEnumerator AnimateText(Text targetText, string label, float targetValue)
    {
        float duration = 3.0f; // 애니메이션 지속 시간
        float elapsed = 0f;
        float startValue = 0f; // 0부터 시작

        targetText.alignment = TextAnchor.MiddleCenter; // 중앙 정렬 유지

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            targetText.text = $"{label} \n{Mathf.RoundToInt(currentValue)}"; // 정수로 표시
            yield return null;
        }

        // 최종 값 설정
        targetText.text = $"{label} \n{Mathf.RoundToInt(targetValue)}";
    }

    float GetValueFromDict(Dictionary<string, object> data, string key)
    {
        if (data.ContainsKey(key) && data[key] is long)
        {
            return (long)data[key];
        }
        if (data.ContainsKey(key) && data[key] is double)
        {
            return (float)(double)data[key];
        }
        return 0f;
    }

    IEnumerator AnimateSumText(Text targetText, string label, float targetValue, float maxValue)
    {
        float duration = 3.0f;
        float elapsed = 0f;
        float startValue = 0f;

        targetText.alignment = TextAnchor.MiddleCenter;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsed / duration);
            targetText.text = $"{label}\n{Mathf.RoundToInt(currentValue)} / {Mathf.RoundToInt(maxValue)}";
            yield return null;
        }

        targetText.text = $"{label}\n{Mathf.RoundToInt(targetValue)} / {Mathf.RoundToInt(maxValue)}";
    }

    IEnumerator AnimateStatusText(TMPro.TextMeshProUGUI statusText, string finalStatus, Color finalColor)
    {
        string[] tempStatuses = { "정상", "경계", "주의", "심각" };
        Color[] tempColors = { Color.green, Color.yellow, new Color(1f, 0.5f, 0f), Color.red };

        // 상태 순서 랜덤 섞기
        List<int> indices = new List<int> { 0, 1, 2, 3 };
        System.Random rng = new System.Random();
        for (int i = 0; i < indices.Count; i++)
        {
            int swapIndex = rng.Next(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[swapIndex];
            indices[swapIndex] = temp;
        }

        float duration = 3.0f;
        float interval = 0.3f;
        float elapsed = 0f;
        int index = 0;

        while (elapsed < duration)
        {
            int i = indices[index % indices.Count];  // 랜덤 순서대로
            statusText.text = tempStatuses[i];
            statusText.color = tempColors[i];
            yield return new WaitForSeconds(interval);
            elapsed += interval;
            index++;
        }

        // 최종 상태로 고정
        statusText.text = finalStatus;
        statusText.color = finalColor;
    }

    void SetStatusText(TMPro.TextMeshProUGUI statusText, float value)
    {
        string status = "정상";
        Color color = Color.green;

        if (value < 25f)
        {
            status = "심각";
            color = Color.red;
        }
        else if (value < 50f)
        {
            status = "주의";
            color = new Color(1f, 0.5f, 0f);
        }
        else if (value < 75f)
        {
            status = "경계";
            color = Color.yellow;
        }

        StartCoroutine(AnimateStatusText(statusText, status, color));
    }

    void SetSumStatusText(TMPro.TextMeshProUGUI statusText, float value)
    {
        string status = "정상";
        Color color = Color.green;

        if (value < 125f)
        {
            status = "심각";
            color = Color.red;
        }
        else if (value < 250f)
        {
            status = "경계";
            color = Color.yellow;
        }
        else if (value < 375f)
        {
            status = "주의";
            color = new Color(1f, 0.5f, 0f);
        }

        StartCoroutine(AnimateStatusText(statusText, status, color));
    }

}
