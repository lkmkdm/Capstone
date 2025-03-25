using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FirebaseDataToUI : MonoBehaviour
{
    public Text memoryText;
    public Text concentrationText;
    public Text processingSpeedText;
    public Text impulsivenessText;
    public Text accuracyText;

    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadDataFromFirebase();
    }

    void LoadDataFromFirebase()
    {
        db.Collection("example").Document("ability").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DocumentSnapshot snapshot = task.Result;
                Dictionary<string, object> data = snapshot.ToDictionary();

                // Firebase에서 가져온 값을 애니메이션으로 표시
                StartCoroutine(AnimateText(memoryText, "기억력", GetValueFromDict(data, "memory")));
                StartCoroutine(AnimateText(concentrationText, "집중력", GetValueFromDict(data, "concentration")));
                StartCoroutine(AnimateText(processingSpeedText, "처리속도", GetValueFromDict(data, "processing speed")));
                StartCoroutine(AnimateText(impulsivenessText, "충동성", GetValueFromDict(data, "impulsiveness")));
                StartCoroutine(AnimateText(accuracyText, "정확도", GetValueFromDict(data, "accuracy")));
            }
            else
            {
                Debug.LogError("Firebase 데이터 불러오기 실패");
            }
        });
    }

    IEnumerator AnimateText(Text targetText, string label, float targetValue)
    {
        float duration = 1.0f; // 애니메이션 지속 시간
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
}
